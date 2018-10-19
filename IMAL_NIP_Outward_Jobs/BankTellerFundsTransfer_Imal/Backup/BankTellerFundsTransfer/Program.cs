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
namespace BankTellerFundsTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'BankTellerFundsTransfer.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    throw new Exception("An instance of this Application is already Running!!!.This instance will be stopped.");
                    //return;
                }
                else
                {
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("");
                        Console.WriteLine("Requerying sent transactions [status=99]....");
                        ds = getTransactions("99");
                        requeryDS(ds);
                        Console.WriteLine("");
                        Console.WriteLine("Requerying failed(09,21,91,96) transactions [status=88]....");
                        ds = getTransactions("88");
                        requeryFinalDS(ds);
                        Console.WriteLine("");
                        Console.WriteLine("Processing new transactions [status=0]....");
                        ds = getTransactions("0");
                        transferDS(ds);

                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(20000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
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
                    DateTime dt = DateTime.Now;
                    string todaysDate = dt.ToString("dd-MMM-yy");
                    if (todaysDate.ToUpper() == t.tra_date.ToUpper())
                    {
                        updateStatus(t.ne_session_id, 99);
                        string txtresp = ws.sendTrnx(t);
                        char[] sep ={ ':' };
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
                            new ErrorLog("Error Occured.  arg returned " + arr[1].ToString() + " " + t.ne_session_id);
                        }
                        Console.WriteLine("Finished processing " + t.ne_session_id + " for " + t.sender_name +
                            ", " + resp.ToString());
                        new ErrorLog(t.ne_session_id + ", " + t.sender_acct + ", " + resp.ToString());

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
            string sql = "select * from tbl_nibbstrans where neresponse ='00' " +
                " and statusflag = @sf and retries < 5 and Approvevalue = 1 and approvedby is not null and CONVERT(varchar(50), inputdate,112) = CONVERT(varchar(50), GETDATE(),112)";
            Connect cn = new Connect(sql, true);
            cn.addparam("@sf", statuscode);
            return cn.query("recs");
        }
    }
    class ErrorLog
    {
        public ErrorLog(Exception ex)
        {
            string pth = "D:\\AppLogs\\BankTellerFTLog\\";
            string err = ex.ToString();
            //string err = ex.Message;

            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            if (!File.Exists(pth))
            {
                using (StreamWriter sw = File.CreateText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }

            }

        }
        public ErrorLog(string ex)
        {
            string pth = "D:\\AppLogs\\BankTellerFTLog\\";
            string err = ex;
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            if (!File.Exists(pth))
            {
                using (StreamWriter sw = File.CreateText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }

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
                msgBuilder m = new msgBuilder();
                m.mobile = mobile;
                m.message = msg;
                string txt = m.buildRequest();
                Gadget g = new Gadget();
                txt = g.enkrypt(txt);
                SMS.smsSender sms = new SMS.smsSender();
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                WebProxy pr = new WebProxy("10.0.0.120", 80);
                sms.Proxy = pr;
                sms.sendMessageLive(txt, "BankTellerFT");
                new ErrorLog(mobile + ": " + msg);
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

        public void setTrnx(DataRow dr)
        {
            TransactionServices tsv = new TransactionServices();
            tsv.getCUrrentFee();
            //fee = tsv.currentFee;
            fee = 1500;
            try
            {
                ne_session_id = dr["sessionid"].ToString();
                Console.WriteLine("Processing " + ne_session_id);
                destbankcode = dr["Benebankcode"].ToString();
                bra_code = dr["bra_code"].ToString();
                cus_num = dr["cus_num"].ToString();
                cur_code = dr["cur_code"].ToString();
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
                Bank.bank bk = new Bank.bank();
                //DataSet ds = bk.CusInfo(bra_code + cus_num + cur_code + led_code + sub_acct_code);
                DataSet ds = bk.CusInfo2(bra_code, cus_num , cur_code , led_code , sub_acct_code);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sender_email = ds.Tables[0].Rows[0]["em"].ToString();
                    sender_mobile = ds.Tables[0].Rows[0]["mb"].ToString();
                    sender_name = ds.Tables[0].Rows[0]["fn"].ToString();
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

    class Gadget
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
                BankTeller.bankTeller ws = new BankTeller.bankTeller();
                
                
                WebProxy pr = new WebProxy("10.0.0.120", 80);
                ws.Proxy = pr;
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

        public string sendTrnx(Trnx t)
        {
            string resp = "99:0";
            try
            {
                //get the tellerid of the approver
                TransactionServices tsv = new TransactionServices();
                tsv.getApprovebySessionid(t.ne_session_id);
                string approvedby = tsv.approvedby;

                PowerUserService pus = new PowerUserService();
                PowerUser pu = pus.getUser(approvedby);
                t.tellerid = pu.tellerId;

                BankTeller.bankTeller ws = new BankTeller.bankTeller();
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                WebProxy pr = new WebProxy("10.0.0.120", 80);
                ws.Proxy = pr;

            t.sender_name = t.sender_name.Replace("& ", "&amp;");
            t.sender_name = t.sender_name.Replace("'", "&apos;");
            t.sender_name = t.sender_name.Replace("\"", "&quot;");

            t.bene_name = t.bene_name.Replace("& ", "&amp;");
            t.bene_name = t.bene_name.Replace("'", "&apos;");
            t.bene_name = t.bene_name.Replace("\"", "&quot;");

            t.remarks = t.remarks.Replace("& ", "&amp;");
            t.remarks = t.remarks.Replace("'", "&apos;");
            t.remarks = t.remarks.Replace("\"", "&quot;");

            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            
            WebProxy p = new WebProxy("10.0.0.120", 80);
            ws.Proxy = p;
                resp = ws.DebitandSendtoNIBSS(t.ne_session_id, t.bra_code, t.cus_num, t.cur_code, t.led_code,
                    t.sub_acct_code, t.amt.ToString(), t.fee.ToString(), t.sender_name, t.destbankcode, "1",
                    t.bene_name, t.bene_acct, t.remarks, t.sender_name,t.tellerid);
            }
            catch
            {

            }
            return resp;
        }
    }
}
