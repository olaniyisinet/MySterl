using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TakeUssdCharges
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'TakeUssdCharges.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    throw new Exception("An instance of this Application is already Running!!!.This instance will be stopped.");
                }
                else
                {
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("");
                        Console.WriteLine("Processing new transactions [statusflag=0]....");
                        ds = getTransactions();
                        processDS(ds);

                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(1000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                    }
                }
            }
            catch(Exception ex) { }
        }
        static DataSet getTransactions()
        {
            //get all pending transactions to be treated
            string sql = "select refid,sessionid,debitAccount,debitamt,transtypeid, statusflag from tbl_USSD_trnx_chrgTaken where statusflag in (0,3)";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            return ds;
        }
        static void processDS(DataSet ds)
        {
            //loop through the items and deal
            StringBuilder rqt = new StringBuilder(); 
            StringBuilder rsp = new StringBuilder(); Gadget g = new Gadget();
            int cnt = ds.Tables[0].Rows.Count; string sid = ""; int Transtatus = 0; Int32 refid = 0;
            string debitaccount = "";string toAccount = ""; decimal amt = 0;string resp = "";
            string responseCode = ""; string responseText = ""; int transtypeid = 0; int statusflag = 0;
            decimal sterling_amt = 0; decimal sterling_vat = 0; decimal Balance_amt = 0; decimal Balance_vat = 0;
            decimal Acct_Check_amt = 0; decimal Acct_Check_vat = 0;
            string toAccountVat = ""; decimal vat = 0; int Last4 = 0;
            string VAT_bra_code = ConfigurationManager.AppSettings["VAT_bra_code"].ToString();
            string VAT_cus_num = ConfigurationManager.AppSettings["VAT_cus_num"].ToString();
            string VAT_cur_code = ConfigurationManager.AppSettings["VAT_cur_code"].ToString();
            string VAT_led_code = ConfigurationManager.AppSettings["VAT_led_code"].ToString();

            Last4 = int.Parse(VAT_bra_code.Substring(6, 3)) + 2000;
            toAccountVat = "NGN" + VAT_led_code + "0001" + Last4.ToString();


            sterling_amt = decimal.Parse(ConfigurationManager.AppSettings["Sterling_amt"].ToString());
            sterling_vat = decimal.Parse(ConfigurationManager.AppSettings["sterling_vat"].ToString());

            Balance_amt = decimal.Parse(ConfigurationManager.AppSettings["Balance_amt"].ToString());
            Balance_vat = decimal.Parse(ConfigurationManager.AppSettings["Balance_vat"].ToString());

            Acct_Check_amt = decimal.Parse(ConfigurationManager.AppSettings["Acct_Check_amt"].ToString());
            Acct_Check_vat = decimal.Parse(ConfigurationManager.AppSettings["Acct_Check_vat"].ToString());

            if (cnt == 0)
            {
                //return;
            }
            else
            {
                toAccount = ConfigurationManager.AppSettings["toAccount"].ToString();
                for (int i = 0; i < cnt; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    Console.WriteLine("");
                    Console.WriteLine("Running item " + i.ToString() + " of " + cnt.ToString());
                    refid = Int32.Parse(dr["refid"].ToString());
                    sid = dr["sessionid"].ToString();
                    debitaccount = dr["debitAccount"].ToString();
                    amt = sterling_amt;//decimal.Parse(dr["debitamt"].ToString());
                    transtypeid = int.Parse(dr["transtypeid"].ToString());
                    statusflag = int.Parse(dr["statusflag"].ToString());
                    if (transtypeid == 1) //Transtype 1 is sterling to sterling transfer
                    {

                        Transtatus = getTxnStatus(sid);
                        if(Transtatus == 0)
                        {
                            continue;
                        }
                        IBS.BSServicesSoapClient ws = new IBS.BSServicesSoapClient();
                        if (Transtatus == 1 && transtypeid == 1)
                        {
                            //mark the record for treating
                            UpdateRecord(88, refid);
                            //proceed to debit
                            
                            rqt.Clear();
                            rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                            rqt.Append("<IBSRequest>");
                            rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                            rqt.Append("<RequestType>" + "102" + "</RequestType>");
                            rqt.Append("<FromAccount>" + debitaccount + "</FromAccount>");
                            rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                            rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                            rqt.Append("<PaymentReference>" + "Charge taken for ussd transaction for sid " + sid + "</PaymentReference>");
                            rqt.Append("</IBSRequest>");
                            string str = "";
                            str = rqt.ToString();
                            str = g.Encrypt(str, 26);

                            try
                            {
                                resp = ws.IBSBridge(str, 26);
                            }
                            catch
                            {
                                //timeout occured
                                UpdateRecord(81, refid);
                            }
                            try
                            {
                                resp = g.Decrypt(resp, 26);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(resp);
                                responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                                responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                                if (responseCode == "00")//successful
                                {
                                    //take the vat
                                    amt = sterling_vat;
                                    //update statusflag to 1
                                    UpdateRecord(1, refid);
                                    UpdateIncome(refid, 1, responseCode, responseText);
                                    //after the update for income then vat should be taken
                                    rqt.Clear();
                                    rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                                    rqt.Append("<IBSRequest>");
                                    rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                                    rqt.Append("<RequestType>" + "102" + "</RequestType>");
                                    rqt.Append("<FromAccount>" + debitaccount + "</FromAccount>");
                                    rqt.Append("<ToAccount>" + toAccountVat + "</ToAccount>");
                                    rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                                    rqt.Append("<PaymentReference>" + "vat taken for ussd transaction for sid " + sid + "</PaymentReference>");
                                    rqt.Append("</IBSRequest>");
                                    string str1 = "";
                                    str1 = rqt.ToString();
                                    str1 = g.Encrypt(str1, 26);
                                    resp = "";
                                    try
                                    {
                                        resp = ws.IBSBridge(str1, 26);
                                        resp = g.Decrypt(resp, 26);
                                        XmlDocument xmlDoc1 = new XmlDocument();
                                        xmlDoc1.LoadXml(resp);
                                        responseCode = xmlDoc1.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                                        responseText = xmlDoc1.GetElementsByTagName("ResponseText").Item(0).InnerText;
                                        if(responseCode == "00")
                                        {
                                            UpdateVat(refid, 1, responseCode, responseText);
                                        }
                                        else
                                        {
                                            UpdateVat(refid, -1, responseCode, responseText);
                                        }
                                    }
                                    catch
                                    {
                                        //timeout occured
                                        UpdateRecord(81, refid);
                                    }
                                }
                                else
                                {
                                    //update statusflag to 2
                                    UpdateRecord(2, refid);
                                    UpdateIncome(refid, -1, responseCode, responseText);
                                }
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("Error occured while processing the transaction with refid " + refid.ToString());
                                return;
                            }
                        }
                    }
                    else if (transtypeid == 2) //balance enquiry
                    {
                        amt = Balance_amt;
                        //mark the record for treating
                        UpdateRecord(88, refid);
                        //proceed to debit
                        IBS.BSServicesSoapClient ws = new IBS.BSServicesSoapClient();
                        rqt.Clear();
                        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        rqt.Append("<IBSRequest>");
                        rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                        rqt.Append("<RequestType>" + "102" + "</RequestType>");
                        rqt.Append("<FromAccount>" + debitaccount + "</FromAccount>");
                        rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                        rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                        rqt.Append("<PaymentReference>" + "Charge taken for ussd Balance enquiry with sid " + sid + "</PaymentReference>");
                        rqt.Append("</IBSRequest>");
                        string str = "";
                        str = rqt.ToString();
                        str = g.Encrypt(str, 26);
                        try
                        {
                            resp = ws.IBSBridge(str, 26);
                        }
                        catch(Exception ex)
                        {
                            //Timeout
                            UpdateRecord(81, refid);
                            continue;
                        }
                        try
                        {
                            resp = g.Decrypt(resp, 26);
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(resp);
                            responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                            responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                            if (responseCode == "00")
                            {
                                amt = Balance_vat;
                                //update statusflag to 1
                                UpdateRecord(1, refid);
                                UpdateIncome(refid, 1, responseCode, responseText);
                                //after the update for income then vat should be taken
                                rqt.Clear();
                                rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                                rqt.Append("<IBSRequest>");
                                rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                                rqt.Append("<RequestType>" + "102" + "</RequestType>");
                                rqt.Append("<FromAccount>" + debitaccount + "</FromAccount>");
                                rqt.Append("<ToAccount>" + toAccountVat + "</ToAccount>");
                                rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                                rqt.Append("<PaymentReference>" + "vat taken for Balance enquiry for sid " + sid + "</PaymentReference>");
                                rqt.Append("</IBSRequest>");
                                string str1 = "";
                                str1 = rqt.ToString();
                                str1 = g.Encrypt(str1, 26);
                                resp = "";
                                try
                                {
                                    resp = ws.IBSBridge(str1, 26);
                                    resp = g.Decrypt(resp, 26);
                                    XmlDocument xmlDoc1 = new XmlDocument();
                                    xmlDoc1.LoadXml(resp);
                                    responseCode = xmlDoc1.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                                    responseText = xmlDoc1.GetElementsByTagName("ResponseText").Item(0).InnerText;
                                    if (responseCode == "00")
                                    {
                                        UpdateVat(refid, 1, responseCode, responseText);
                                    }
                                    else
                                    {
                                        UpdateVat(refid, -1, responseCode, responseText);
                                    }
                                }
                                catch
                                {
                                    //timeout occured
                                    UpdateRecord(81, refid);
                                }
                            }
                            else
                            {
                                //update statusflag to 2
                                UpdateRecord(2, refid);
                            }
                        }
                        catch (Exception ex)
                        {
                            new Errorlog("Error occured while processing the transaction with refid " + refid.ToString());
                            return;
                        }
                    }
                    else if (transtypeid == 3) //Account checck
                    {
                        amt = Acct_Check_amt;
                        //mark the record for treating
                        UpdateRecord(88, refid);
                        //proceed to debit
                        IBS.BSServicesSoapClient ws = new IBS.BSServicesSoapClient();
                        rqt.Clear();
                        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        rqt.Append("<IBSRequest>");
                        rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                        rqt.Append("<RequestType>" + "102" + "</RequestType>");
                        rqt.Append("<FromAccount>" + debitaccount + "</FromAccount>");
                        rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                        rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                        rqt.Append("<PaymentReference>" + "Charge taken for ussd Account enquiry with sid " + sid + "</PaymentReference>");
                        rqt.Append("</IBSRequest>");
                        string str = "";
                        str = rqt.ToString();
                        str = g.Encrypt(str, 26);
                        try
                        {
                            resp = ws.IBSBridge(str, 26);
                        }
                        catch (Exception ex)
                        {
                            //Timeout
                            UpdateRecord(81, refid);
                            continue;
                        }
                        try
                        {
                            resp = g.Decrypt(resp, 26);
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(resp);
                            responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                            responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                            if (responseCode == "00")
                            {
                                amt = Acct_Check_vat;
                                //update statusflag to 1
                                UpdateRecord(1, refid);
                                UpdateIncome(refid, 1, responseCode, responseText);
                                //after the update for income then vat should be taken
                                rqt.Clear();
                                rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                                rqt.Append("<IBSRequest>");
                                rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                                rqt.Append("<RequestType>" + "102" + "</RequestType>");
                                rqt.Append("<FromAccount>" + debitaccount + "</FromAccount>");
                                rqt.Append("<ToAccount>" + toAccountVat + "</ToAccount>");
                                rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                                rqt.Append("<PaymentReference>" + "vat taken for Account enquiry for sid " + sid + "</PaymentReference>");
                                rqt.Append("</IBSRequest>");
                                string str1 = "";
                                str1 = rqt.ToString();
                                str1 = g.Encrypt(str1, 26);
                                resp = "";
                                try
                                {
                                    resp = ws.IBSBridge(str1, 26);
                                    resp = g.Decrypt(resp, 26);
                                    XmlDocument xmlDoc1 = new XmlDocument();
                                    xmlDoc1.LoadXml(resp);
                                    responseCode = xmlDoc1.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                                    responseText = xmlDoc1.GetElementsByTagName("ResponseText").Item(0).InnerText;
                                    if (responseCode == "00")
                                    {
                                        UpdateVat(refid, 1, responseCode, responseText);
                                    }
                                    else
                                    {
                                        UpdateVat(refid, -1, responseCode, responseText);
                                    }
                                }
                                catch
                                {
                                    //timeout occured
                                    UpdateRecord(81, refid);
                                }
                            }
                            else
                            {
                                //update statusflag to 2
                                UpdateRecord(2, refid);
                            }
                        }
                        catch (Exception ex)
                        {
                            new Errorlog("Error occured while processing the transaction with refid " + refid.ToString());
                            return;
                        }
                    }
                    else
                    {
                        UpdateRecord(3, refid);
                    }
                }
            }
        }//end for processDS

        public static void UpdateRecord(int status, Int32 id)
        {
            string sql = "update tbl_USSD_trnx_chrgTaken set statusflag =@status,datetreated=getdate() where refid=@id ";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetSQL(sql);
            c.AddParam("@status", status);
            c.AddParam("@id", id);
            c.Execute();
        }

        public static void UpdateIncome(Int32 id, int vatStatus, string vtellerIncomResponseCode, string vtellerIncomemsg)
        {
            string sql = "update tbl_USSD_trnx_chrgTaken set incomeTaken =@st," +
                " datetreated=getdate(),vattaken = @st,vtellerIncomResponseCode = @Inc,vtellerIncomemsg =@Incmsg where refid=@id ";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetSQL(sql);
            c.AddParam("@id", id);
            c.AddParam("@st", vatStatus);
            c.AddParam("@Inc", vtellerIncomResponseCode);
            c.AddParam("@Incmsg", vtellerIncomemsg);
            c.Execute();
        }
        public static void UpdateVat(Int32 id, int vatStatus, string vtellerVatResponseCode, string vtellerVatmsg)
        {
            string sql = "update tbl_USSD_trnx_chrgTaken set " +
                " datetreated=getdate(),vattaken = @st,vtellerVatResponseCode = @vtr,vtellerVatmsg =@vmsg where refid=@id ";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetSQL(sql);
            c.AddParam("@id", id);
            c.AddParam("@st", vatStatus);
            c.AddParam("@vtr", vtellerVatResponseCode);
            c.AddParam("@vmsg", vtellerVatmsg);
            c.Execute();
        }
        public static int getTxnStatus(string sid)
        {
            int resp = 0;
            //string sql = "select * from tbl_USSD_transfers " +
              //  " where sessionid =@sid";
            string sql = " select * from tbl_USSD_transfers " +
                " where sessionid =@sid" +
                " union  " +
                " select * from tbl_USSD_transfers_batch " +
                " where sessionid =@sid";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            //string sql = "select * from tbl_USSD_transfers " +
            //    " where sessionid =@sid and " +
            //    " convert(varchar(50),dateadded,112)  = convert(varchar(50),GETDATE(),112) ";
            //Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetSQL(sql);
            c.AddParam("@sid", sid);
            DataSet ds = c.Select("rec");
            if(ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                resp = int.Parse(dr["statusflag"].ToString());
            }
            return resp;
        }
    }
}
