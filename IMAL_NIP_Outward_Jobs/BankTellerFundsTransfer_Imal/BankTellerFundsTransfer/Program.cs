using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Threading;
using System.Net;
using System.Management;
using Tj.Cryptography;
namespace BankTellerFundsTransfer_lagos3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'BankTellerFundsTransfer_imal.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    //throw new Exception("An instance of this Application is already Running!!!.This instance will be stopped.");
                    return;
                }
                else
                {
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("Processing new transactions [status=0]....");
                        ds = getTransactions("21");
                        transferDS(ds);
                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        //Thread.Sleep(20000);
                        Thread.Sleep(1000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                        //ds = getTransactions("99");
                        //transferDS1(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
            }
        }

        static void JobProcess(object r)
        {
            Trnx t = (Trnx)r;
            NFPWS ws = new NFPWS();
            DateTime dt = DateTime.Now;
            string todaysDate = dt.ToString("dd-MMM-yy");
            if (todaysDate.ToUpper() == t.tra_date.ToUpper())
            {
                updateStatus(t.ne_session_id, 99);
                string txtresp = ws.sendTrnx(t);
                char[] sep = { ':' };
                string[] arr = txtresp.Split(sep, StringSplitOptions.None);
                int resp = 0;
                try
                {
                    resp = Convert.ToInt32(arr[0]);
                }
                catch { }
                t.paymentRef = Convert.ToInt32(arr[1]);
                Console.WriteLine("Finished processing " + t.ne_session_id + " for " + t.sender_name +
                    ", " + resp.ToString());
                new ErrorLog("Finished processing " + t.ne_session_id + ", " + t.sender_acct + ", " + resp.ToString());

                Alert al = new Alert();

                switch (resp)
                {
                    case 1: //Name Enquiry failed, retry 5 times
                        t.retries++;
                        updatePayRef(t.ne_session_id, t.paymentRef);
                        if (t.retries == 5)
                        {
                            updateStatus(t.ne_session_id, 2);
                            al.sendFailure(t, "");
                        }
                        else
                        {
                            updateRetries(t.ne_session_id, t.retries);
                        }
                        break;
                    case 2: //vTeller Failed
                        updateStatus(t.ne_session_id, 2);
                        updatePayRef(t.ne_session_id, t.paymentRef);
                        al.sendFailure(t, "");
                        break;
                    case 3: //trnx was successful
                        updateStatus(t.ne_session_id, 1);
                        updatePayRef(t.ne_session_id, t.paymentRef);
                        al.sendSuccess(t);
                        break;
                    case 4: //FT failed, auto reversal has been done
                        updateStatus(t.ne_session_id, 2);
                        updatePayRef(t.ne_session_id, t.paymentRef);
                        al.sendFailure(t, "");
                        break;
                    case 5: //FT failed , but no auto reversal
                        updateStatus(t.ne_session_id, 88);
                        updatePayRef(t.ne_session_id, t.paymentRef);
                        al.sendWaiting(t);
                        break;
                    case 6: //duplicate name enquiry session id
                        updateStatus(t.ne_session_id, 3);
                        break;
                    case 99:
                        updateStatus(t.ne_session_id, 99);
                        al.sendWaiting(t);
                        break;
                }
            }
            else
            {
                new ErrorLog("Transaction does not carry todays date: " + t.ne_session_id);
                MarkLateTrans(t.ne_session_id);
            }
        }
        public static string resp_val = "";
        static bool IsSessiononNEontable(string SessionNE)
        {
            bool found = false;
            string sql = "select sessionidNE,response from tbl_nibssmobile where sessionidNE = @se";
            Connect c = new Connect(sql, true);
            c.addparam("@se", SessionNE);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                resp_val = dr["response"].ToString();
                found = true;
            }
            else
            {
                found = false;
            }
            return found;
        }

        //static bool IsSessiononNEontable(string SessionNE)
        //{
        //    bool found = false;
        //    string sql = "select sessionidNE from tbl_nibssmobile where sessionidNE = @se";
        //    Connect c = new Connect(sql, true);
        //    c.addparam("@se", SessionNE);
        //    DataSet ds = c.query("rec");
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        found = true;
        //    }
        //    else
        //    {
        //        found = false;
        //    }
        //    return found;
        //}

        static void transferDS1(DataSet ds)
        {
            int cnt = ds.Tables[0].Rows.Count;
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                NFPWS ws = new NFPWS();
                for (int i = 0; i < cnt; i++)
                {
                    Trnx t = new Trnx();
                    t.setTrnx(ds.Tables[0].Rows[i]);
                    //check if this Nameenquiry sessionid exist on the table
                    bool found = IsSessiononNEontable(t.ne_session_id);
                    if (found)
                    {

                        if (resp_val == "00")
                        {
                            updateStatus(t.ne_session_id, 1);
                        }
                        else
                        {

                            if (resp_val == "1x")
                            {
                                updateStatus(t.ne_session_id, 13);
                            }
                            else if (resp_val == "11x")
                            {
                                updateStatus(t.ne_session_id, 13);
                            }
                            else
                            {
                                updateStatus(t.ne_session_id, int.Parse(resp_val));
                            }
                        }
                        new ErrorLog("The transaction with sessiond " + t.ne_session_id + " has already been treated");
                    }
                    else
                    {

                    }
                }
            }
        }

        static void transferDS(DataSet ds)
        {
            int cnt = ds.Tables[0].Rows.Count;
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                NFPWS ws = new NFPWS();
                for (int i = 0; i < cnt; i++)
                {
                    Trnx t = new Trnx();
                    t.setTrnx(ds.Tables[0].Rows[i]);
                    //check if this Nameenquiry sessionid exist on the table
                    bool found = IsSessiononNEontable(t.ne_session_id);
                        new ErrorLog("The transaction with sessiond " + t.ne_session_id + " has response val " + resp_val);

                    if (found)
                    {

                        if (resp_val == "00")
                        {

                            updateStatus(t.ne_session_id, 1);
                        }
                        else
                        {

                            if (resp_val == "1x")
                            {
                                updateStatus(t.ne_session_id, 13);
                            }
                            else if (resp_val == "11x")
                            {
                                updateStatus(t.ne_session_id, 13);
                            }
                         
                            else
                            {
                               
                                updateStatus(t.ne_session_id, int.Parse(resp_val));
                            }
                        }
                        new ErrorLog("The transaction with sessiond " + t.ne_session_id + " has already been treated");
                    }
                    else
                    {
                        DateTime dt = DateTime.Now;
                        string todaysDate = dt.ToString("dd-MMM-yy");
                        if (todaysDate.ToUpper() == t.tra_date.ToUpper())
                        {
                            updateStatus(t.ne_session_id, 77);
                            string txtresp = ws.sendTrnx(t);
                            char[] sep = { ':' };
                            string[] arr = txtresp.Split(sep, StringSplitOptions.None);
                            int resp = 0;
                            try
                            {
                                resp = Convert.ToInt32(arr[0]);
                            }
                            catch { }
                            try
                            {
                                t.paymentRef = Convert.ToInt32(arr[1]);
                            }
                            catch (Exception ex)
                            {
                                new ErrorLog("Error Occured.  arg returned " + arr[1].ToString() + " " + t.ne_session_id+". exceptoon is "+ex.ToString());
                            }
                            Console.WriteLine("Finished processing " + t.ne_session_id + " for " + t.sender_name +
                                ", " + resp.ToString());
                            new ErrorLog("Finished processing " + t.ne_session_id + ", " + t.sender_acct + ", and response gotten is " + resp.ToString());

                            Alert al = new Alert();

                            switch (resp)
                            {
                                case 1: //Name Enquiry failed, retry 5 times
                                    t.retries++;
                                    updatePayRef(t.ne_session_id, t.paymentRef);
                                    if (t.retries == 5)
                                    {
                                        updateStatus(t.ne_session_id, 2);
                                        al.sendFailure(t, "");
                                    }
                                    else
                                    {
                                        updateRetries(t.ne_session_id, t.retries);
                                    }
                                    break;
                                case 2: //vTeller Failed
                                    updateStatus(t.ne_session_id, 2);
                                    updatePayRef(t.ne_session_id, t.paymentRef);
                                    al.sendFailure(t, "");
                                    break;
                                case 3: //trnx was successful
                                    updateStatus(t.ne_session_id, 1);
                                    updatePayRef(t.ne_session_id, t.paymentRef);
                                    al.sendSuccess(t);
                                    break;
                                case 4: //FT failed, auto reversal has been done
                                    updateStatus(t.ne_session_id, 2);
                                    updatePayRef(t.ne_session_id, t.paymentRef);
                                    al.sendFailure(t, "");
                                    break;
                                case 5: //FT failed , but no auto reversal
                                    updateStatus(t.ne_session_id, 88);
                                    updatePayRef(t.ne_session_id, t.paymentRef);
                                    al.sendWaiting(t);
                                    break;
                                case 6: //duplicate name enquiry session id
                                    updateStatus(t.ne_session_id, 3);
                                    break;
                                case 12: //duplicate name enquiry session id
                                    updateStatus(t.ne_session_id, 12);
                                    updatePayRef(t.ne_session_id, t.paymentRef);
                                    break;
                                case 99:
                                    updateStatus(t.ne_session_id, 99);
                                    al.sendWaiting(t);
                                    break;
                                case 51:
                                    updateStatus(t.ne_session_id, 51);
                                    al.sendWaiting(t);
                                    break;
                            }
                        }
                        else
                        {
                            new ErrorLog("Transaction does not carry todays date: " + t.ne_session_id);
                            MarkLateTrans(t.ne_session_id);
                        }
                    }
                }
            }
        }

        static void requeryDS(DataSet ds)
        {
            int cnt = ds.Tables[0].Rows.Count;
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                NFPWS ws = new NFPWS();
                for (int i = 0; i < cnt; i++)
                {
                    Trnx t = new Trnx();
                    t.setTrnx(ds.Tables[0].Rows[i]);

                    //check if the customer has a valid mobile number
                    //if (t.sender_mobile == "")
                    //{
                    //    updateStatus(t.ne_session_id, 3);
                    //    continue;
                    //}

                    int[] resp = ws.requery(t.ne_session_id);
                    Alert al = new Alert();
                    switch (resp[1])
                    {
                        case 0:
                            updateStatus(t.ne_session_id, 0);
                            break;
                        case 3:
                            updateStatus(t.ne_session_id, 1);
                            al.sendSuccess(t);
                            break;
                        case 4:
                            updateStatus(t.ne_session_id, 2);
                            al.sendFailure(t, "");
                            break;
                        case 5:
                            updateStatus(t.ne_session_id, 88);

                            break;
                        case 99:

                            break;
                    }
                }
            }
        }

        static void requeryFinalDS(DataSet ds)
        {
            int cnt = ds.Tables[0].Rows.Count;
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                NFPWS ws = new NFPWS();
                for (int i = 0; i < cnt; i++)
                {
                    Trnx t = new Trnx();
                    t.setTrnx(ds.Tables[0].Rows[i]);
                    int[] resp = ws.requery(t.ne_session_id);
                    Alert al = new Alert();
                    switch (resp[2])
                    {
                        case 3:
                            updateStatus(t.ne_session_id, 1);
                            al.sendSuccess(t);
                            break;
                        case 4:
                            updateStatus(t.ne_session_id, 2);
                            al.sendFailure(t, "");
                            break;
                        case 5:
                            updateStatus(t.ne_session_id, 2);
                            al.sendFailure(t, "");
                            break;
                        case 99:
                            //updateStatus(t.ne_session_id, 88);
                            //al.sendWaiting(t.ne_session_id);
                            break;
                    }
                }
            }
        }

        static void MarkLateTrans(string sessionid)
        {
            //mark this transaction as late transaction
            string sql = "update tbl_nibbstrans set Approvevalue=9 where sessionid = @ne";
            Connect cn = new Connect(sql, true);
            cn.addparam("@ne", sessionid);
            cn.query();
        }

        static void updateStatus(string sessionid, int statusflag)
        {
            string sql = "update tbl_nibbstrans set statusflag = @sf " +
                " where sessionid = @ne";
            Connect cn = new Connect(sql, true);
            cn.addparam("@sf", statusflag);
            cn.addparam("@ne", sessionid);
            cn.query();
        }

        static void updateRetries(string sessionid, int retries)
        {
            string sql = "update tbl_nibbstrans set retries = @rt " +
                " where sessionid = @ne";
            Connect cn = new Connect(sql, true);
            cn.addparam("@rt", retries);
            cn.addparam("@ne", sessionid);
            cn.query();
        }

        static void updatePayRef(string sessionid, int paymentRef)
        {
            string sql = "update tbl_nibbstrans set paymentRef = @pr " +
                " where sessionid = @ne";
            Connect cn = new Connect(sql, true);
            cn.addparam("@pr", paymentRef.ToString());
            cn.addparam("@ne", sessionid);
            cn.query();
        }

        static DataSet getTransactions(string statuscode)
        {
            string sql = @"select * from tbl_nibbstrans where neresponse ='00'  and statusflag = @sf and retries is not null and Approvevalue = 1 and approvedby is not null and CONVERT(varchar(50), inputdate,112) = CONVERT(varchar(50), GETDATE(),112) " +
                " and mime='2' order by approveddate desc";
            //string sql = @"select * from tbl_nibbstrans_batch where neresponse ='00'  and retries is not null and Approvevalue = 1 and approvedby is not null  " +
            //   " and mime='2' and refid='2217975' order by approveddate desc";
            Connect cn = new Connect(sql, true);
            cn.addparam("@sf", statuscode);
            return cn.query("recs");
        }
    }
    class ErrorLog
    {
        public ErrorLog(Exception ex)
        {
            string pth = "E:\\AppLogs\\BankTellerFTLog\\BankTellerFTLogImal\\";
            string err = ex.ToString();
            //string err = ex.Message;

            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            if (!File.Exists(pth))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(pth))
                    {
                        sw.WriteLine(dt.ToString() + " : " + err);
                        sw.WriteLine(" ");
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(pth))
                    {
                        sw.WriteLine(dt.ToString() + " : " + err);
                        sw.WriteLine(" ");
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch { }
            }

        }
        public ErrorLog(string ex)
        {
            string pth = "E:\\AppLogs\\BankTellerFTLog\\BankTellerFTLogImal\\";
            string err = ex;
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            if (!File.Exists(pth))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(pth))
                    {
                        sw.WriteLine(dt.ToString() + " : " + err);
                        sw.WriteLine(" ");
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch { }
            }
            else
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(pth))
                    {
                        sw.WriteLine(dt.ToString() + " : " + err);
                        sw.WriteLine(" ");
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch { }
            }

        }
    }
    class Alert
    {
        public void sendSuccess(Trnx t)
        {
            string msg = "Your NGN " + t.amt.ToString("#,##0.00") + " was successful. " +
                "REF: netTransfer/" + t.bra_code + "/" + t.sender_name + "/" +
                t.destbankcode + "/" + t.bene_name + "/" + t.paymentRef.ToString();
            postMsg(msg, t.sender_mobile);
        }

        public void sendFailure(Trnx t, string reason)
        {
            string msg = "Your NGN " + t.amt.ToString("#,##0.00") + " was not successful. " +
                "";// "Remark: " + reason;
            postMsg(msg, t.sender_mobile);
        }

        public void sendWaiting(Trnx t)
        {
            //
        }

        protected void postMsg(string msg, string mobile)
        {
            if (mobile != "")
            {
                try
                {
                    msgBuilder m = new msgBuilder();
                    m.mobile = mobile;
                    m.message = msg;
                    string txt = m.buildRequest();
                    Gadget1 g = new Gadget1();
                    txt = g.enkrypt(txt);
                    BankTellerFundsTransfer_imal.SMS.smsSender sms = new BankTellerFundsTransfer_imal.SMS.smsSender();
                    //NetworkCredential nc = new NetworkCredential("spservice", "Kinder$$098", "sterlingbank");
                    //ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //WebProxy pr = new WebProxy("10.0.0.120", 80);
                    //sms.Proxy = pr;
                    //sms.Proxy.Credentials = nc;
                    sms.sendMessageLive(txt, "BankTellerFT");
                    new ErrorLog(mobile + ": " + msg);
                }
                catch (Exception ex)
                {
                    new ErrorLog("Error occured sending sms " + ex);
                }
            }
            else
            {
                //do nothing
            }
        }
    }
    class Trnx
    {
        public string ne_session_id;
        public string destbankcode;
        public string bra_code;
        public string cus_num;
        public string cur_code;
        public string led_code;
        public string sub_acct_code;
        public double amt;
        public string bene_name;
        public string bene_acct;
        public int paymentRef;
        public string remarks;
        public string tra_date;
        public int statusflag;
        public int retries;
        public decimal fee;

        public string sender_name;
        public string sender_mobile;
        public string sender_email;
        public string sender_acct;
        public string tellerid;
        public string OriginatorAccountNumber;
        public string nuban;
        public string bvn;
        public int cur_code1 = 0;
        public void setTrnx(DataRow dr)
        {
            GetActualBankCode abc = new GetActualBankCode();
            TransactionServices tsv = new TransactionServices();
            tsv.getCUrrentFee();
            //fee = tsv.currentFee;
            fee = 1500;
            try
            {
                ne_session_id = dr["sessionid"].ToString();
                Console.WriteLine("Processing " + ne_session_id);
                destbankcode = dr["Benebankcode"].ToString();
                nuban = dr["nuban"].ToString();
                //destbankcode = abc.getNewBankCode(destbankcode);
                //bra_code = dr["bra_code"].ToString();
                bra_code = dr["Orig_bra_code"].ToString();
                cus_num = dr["cus_num"].ToString();
                cur_code1 = int.Parse(dr["cur_code"].ToString());
                if (cur_code1 == 566)
                {
                    cur_code = "NGN";
                }
                led_code = dr["led_code"].ToString();
                sub_acct_code = dr["sub_acct_code"].ToString();
                sender_acct = bra_code + "-" + cus_num + "-" + cur_code + "-" + led_code + "-" + sub_acct_code;
                amt = Convert.ToDouble(dr["amt"]);
                bene_name = dr["Benename"].ToString();
                bene_acct = dr["Beneaccount"].ToString();
                DateTime dt = new DateTime();
                dt = Convert.ToDateTime(dr["inputdate"]);
                tra_date = dt.ToString("dd-MMM-yy");
                remarks = dr["Remark"].ToString();
                remarks = remarks.Replace("& #40;", "(");
                remarks = remarks.Replace("& #41;", ")");
                remarks = remarks.Replace("& #40", "(");
                remarks = remarks.Replace("& #41", ")");
                remarks = remarks.Replace("&", " ");

                statusflag = Convert.ToInt32(dr["statusflag"]);
                retries = Convert.ToInt32(dr["retries"]);
            }
            catch (Exception ex)
            {
                new ErrorLog(ex + "Error occured " + ne_session_id);
            }
            //fee = 100;

            try
            {
                BankTellerFundsTransfer_imal.EACBS.banks bk = new BankTellerFundsTransfer_imal.EACBS.banks();
                BankTellerFundsTransfer_imal.ImalAcctinfo.Service imal = new BankTellerFundsTransfer_imal.ImalAcctinfo.Service();
                //DataSet ds = bk.CusInfo(bra_code + cus_num + cur_code + led_code + sub_acct_code);
                DataSet ds = new DataSet();
                ds = imal.GetAccountByAccountNumber(nuban);
                if (ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count > 0)
                {
                    sender_email = "EMAIL";
                    sender_mobile = "MOBILE";
                    sender_name = ds.Tables[0].Rows[0]["CUSTOMERNAME"].ToString();
                    //bvn =  ds.Tables[0].Rows[0]["bvn"].ToString();
                    bvn = "BVN";
                    OriginatorAccountNumber = ds.Tables[0].Rows[0]["NUBAN"].ToString();
                }
            }
            catch(Exception ex)
            {
                new ErrorLog(ex + " Error for " + ne_session_id);
            }
        }
    }
    class msgBuilder
    {
        public string mobile;
        public string message;
        public string response;
        private StringBuilder xml = new StringBuilder();


        public string buildRequest()
        {
            xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>");
            xml.Append("<txtmessage>");
            xml.Append("<txtmobile>" + mobile + "</txtmobile>");
            xml.Append("<txtcontent>" + cleanString(message) + "</txtcontent>");
            xml.Append("</txtmessage>");
            return xml.ToString();
        }

        protected void readXML(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            mobile = xmlDoc.GetElementsByTagName("txtmobile").Item(0).InnerText;
            message = xmlDoc.GetElementsByTagName("txtcontent").Item(0).InnerText;
            message = dirtyString(message);
        }

        protected string cleanString(string str)
        {
            str = str.Replace("&", "&amp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("'", "&apos;");
            return str;
        }

        protected string dirtyString(string str)
        {
            str = str.Replace("&amp;", "&");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&apos;", "'");
            return str;
        }
    }

    class Gadget1
    {

        //to encrpt
        public string enkrypt(string strEnk)
        {
            SymCryptography cryptic = new SymCryptography();
            cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";
            return cryptic.Encrypt(strEnk);
        }
        //to decrypt
        public string dekrypt(string strDek)
        {
            SymCryptography cryptic = new SymCryptography();
            cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";
            return cryptic.Decrypt(strDek);
        }
    }
    class NFPWS
    {
        public int getVar(string respCode)
        {
            int n = 0;
            switch (respCode)
            {
                case "00":
                case "v1":
                    n = 3;
                    break;
                case "96":
                case "91":
                case "09":
                case "21":
                case "1x":
                case "v3":
                    n = 5;
                    break;
                case "":
                case "v0":
                    n = 0;
                    break;
                case "v2":
                default:
                    n = 4;
                    break;
            }
            return n;
        }

        public int[] requery(string ne_session_id)
        {
            int[] n = { 99, 99, 99, 99, 99 };
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                BankTellerFundsTransfer_imal.BankTeller.bankTeller ws = new BankTellerFundsTransfer_imal.BankTeller.bankTeller();
                NetworkCredential nc = new NetworkCredential("spservice", "Kinder$$098", "sterlingbank");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                WebProxy pr = new WebProxy("10.0.0.120", 80);
                ws.Proxy = pr;
                ws.Proxy.Credentials = nc;
                
                //WebProxy pr = new WebProxy("10.0.0.120", 80);
                //ws.Proxy = pr;
                string str = ws.requeryTrnx(ne_session_id);

                char[] sep = { ',' };
                string[] k = str.Split(sep, StringSplitOptions.None);
                //n = new int[4];
                n = new int[5];
                n[0] = getVar(k[0]);
                n[1] = getVar(k[1]);
                n[2] = getVar(k[2]);
                n[3] = getVar(k[3]);
                n[4] = getVar(k[4]);
            }
            catch (Exception ex)
            {
                ErrorLog err = new ErrorLog(ex);
            }
            return n;
        }

        public bool ConfirmSessionidonNIBSSMobile(string NSID)
        {
            bool found = false;
            string sql = "select sessionid from tbl_nibssmobile " +
                " where sessionidNE=@se " +
                " union " +
                " select sessionid from tbl_nibssmobile_batch " +
                " where sessionidNE=@se ";
            Connect c = new Connect(sql, true);
            c.addparam("@se", NSID);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                found = true;
            }
            else
            {
                found = false;
            }
            return found;
        }
        static void updateStatus(string sessionid, int statusflag)
        {
            try
            {
                string sql = "update tbl_nibbstrans set statusflag = @sf " +
                    " where sessionid = @ne";
                Connect cn = new Connect(sql, true);
                cn.addparam("@sf", statusflag);
                cn.addparam("@ne", sessionid);
                cn.query();
            }
            catch (Exception ex)
            {
                new ErrorLog("Error occured trying to update tbl_nibbstrans for Name enquiry sessionid " + sessionid + " as " + ex);
            }
        }
        public string sendTrnx(Trnx t)
        {
            string resp = "99:0";
            try
            {
                //check if the Name enquiry session id already exist in tbl_nibssmobile, if it exist then update the nibss trans with 1
                bool ifExist = ConfirmSessionidonNIBSSMobile(t.ne_session_id);
                if (ifExist)
                {
                    //uppdate the statusflag of tbl_nibsstrans to avoid retpeating the transaction
                    updateStatus(t.ne_session_id, 55);
                    new ErrorLog("This transaction has already been processed before on tbl_nibssmobile" + t.ne_session_id);
                }
                else
                {
                    //get the tellerid of the approver
                    TransactionServices tsv = new TransactionServices();
                    tsv.getApprovebySessionid(t.ne_session_id);
                    string approvedby = tsv.approvedby;

                    PowerUserService pus = new PowerUserService();
                    PowerUser pu = pus.getUser(approvedby);
                    t.tellerid = pu.tellerId;

                    //BankTellerFundsTransfer_imal.BankTeller.bankTeller ws = new BankTellerFundsTransfer_imal.BankTeller.bankTeller();
                    BankTellerFundsTransfer_imal.BankTellerV2.bankTeller ws = new BankTellerFundsTransfer_imal.BankTellerV2.bankTeller();
                    //ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //NetworkCredential nc = new NetworkCredential("spservice", "Kinder$$098", "sterlingbank");
                    //ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //WebProxy pr = new WebProxy("10.0.0.120", 80);
                    //ws.Proxy = pr;
                    //ws.Proxy.Credentials = nc;
                    //WebProxy pr = new WebProxy("10.0.0.120", 80);
                    //ws.Proxy = pr;







                    t.sender_name = t.sender_name.Replace("& ", "&amp;");
                    t.sender_name = t.sender_name.Replace("'", "&apos;");
                    t.sender_name = t.sender_name.Replace("\"", "&quot;");
                    t.sender_name = t.sender_name.Replace("(SAL AC", "");

                    t.bene_name = t.bene_name.Replace("& ", "&amp;");
                    t.bene_name = t.bene_name.Replace("'", "&apos;");
                    t.bene_name = t.bene_name.Replace("\"", "&quot;");

                    t.remarks = t.remarks.Replace("& ", "&amp;");
                    t.remarks = t.remarks.Replace("'", "&apos;");
                    t.remarks = t.remarks.Replace("\"", "&quot;");

                    //ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

                    //WebProxy p = new WebProxy("10.0.0.120", 80);
                    //ws.Proxy = pr;

                    //go and get other details using the sessionid
                    string BeneficiaryBankVerificationNumber = ""; string BeneficiaryKYCLevel = ""; string OriginatorKYCLevel = "";
                    string OriginatorBankVerificationNumber = ""; string TransactionLocation = ""; string OriginatorAccountNumber = "";
                    //Gadget g = new Gadget();
                    Gizmo g = new Gizmo();
                    GetNameEnqDetails gn = new GetNameEnqDetails();
                    gn.getdetailsbySID(t.ne_session_id);
                    BeneficiaryBankVerificationNumber = gn.BankVerificationNumber;
                    BeneficiaryKYCLevel = gn.KYCLevel;

                    bool kyc = g.findKyc(Int32.Parse(t.led_code));
                    if (kyc)
                    {
                        OriginatorKYCLevel = "1";
                    }
                    else
                    {
                        OriginatorKYCLevel = "3";
                    }

                    OriginatorAccountNumber = t.OriginatorAccountNumber;
                    OriginatorBankVerificationNumber = t.bvn;
                    TransactionLocation = "";
                    var loggingString = string.Format("about to call bankteller ws for transaction with sessiond {0}, originator account nmber {1},amount {2} and beneficiary account number {3}", t.ne_session_id, t.OriginatorAccountNumber, t.amt.ToString(),t.bene_acct);
                    new ErrorLog(loggingString);

                    resp = ws.DebitandSendtoNIBSS(t.ne_session_id, t.bra_code, t.cus_num, t.cur_code, t.led_code,
                        t.sub_acct_code, t.amt.ToString(), t.fee.ToString(), t.sender_name, t.destbankcode, "1",
                        t.bene_name, t.bene_acct, t.remarks, t.sender_name, t.tellerid, BeneficiaryBankVerificationNumber,
                        BeneficiaryKYCLevel, OriginatorAccountNumber, OriginatorBankVerificationNumber, OriginatorKYCLevel, TransactionLocation, t.ne_session_id);
                    var loggingStr = string.Format("control is back from bankteller ws call to bankteller ws for transaction with sessiond {0}, originator account nmber {1},amount {2} and beneficiary account number {3}. response is {4}", t.ne_session_id, t.OriginatorAccountNumber, t.amt.ToString(), t.bene_acct,resp);
                    new ErrorLog(loggingStr);


                }
            }
            catch(Exception ex)
            {
                new ErrorLog("Error Occured for session " + t.ne_session_id + " " + ex);
            }
            return resp;
        }

        private string DebitAndSendToNibb(string sessionid, string bracode, string cusnum, string curcode, string ledcode, string subacctcode, string amt, string fee, string sendername, string destBankCode, string channelCode, string beneName, string beneAcct, string remark, string sender_name, string tellerid, string BeneficiaryBankVerificationNumber, string BeneficiaryKYCLevel, string OriginatorAccountNumber, string OriginatorBankVerificationNumber, string OriginatorKYCLevel, string TransactionLocation, string p17)
        {
            string TSSAcct = "";
            string resp = "";
            //imalVteller.Services vtel = new imalVteller.Services();
            //resp = vtel.FundstransFer(beneAcct, TSSAcct,Convert.ToDecimal(amt), remark);

            return TSSAcct;
        }


    }
}
