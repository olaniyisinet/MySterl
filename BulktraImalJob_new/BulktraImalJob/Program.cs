using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using BulktraImalJob.ImalService;
using System.Management;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BulktraImalJob
{
    class Program
    {
        static int count = 0;
        static void Main(string[] args)
        {
            Mylogger.Info("Input");
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'BulktraImalJob.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    Console.WriteLine("Only one instance of job can run!!!");
                    Mylogger.Info("Only one instance of job can run!!!");
                    return;
                }
                else
                {
                    while (true)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("Processing Transactions...");
                        Console.WriteLine("");

                        postTrnx();

                        Console.WriteLine("Process End Time is:" + DateTime.Now);
                        Console.WriteLine("Press Enter to Exit");
                        Console.ReadLine();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Mylogger.Error(ex);
            }
        }

        static DataSet getPendingImalTxn()
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetProcedure("spd_imalbulktrajob");
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet getBatchIDForPending()
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetProcedure("[spd_pendingBatchID]");
            DataSet ds = c.Select("rec");
            DataTable dt = ds.Tables[0];

            count = dt.Rows.Count;
            return ds;
        }

        static int totalBatchCount()
        {
            return count;
        }

        static DataSet getAcctNoOfBatchSuspense(string batchid)
        {

            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetProcedure("spd_batchsuspense");
            c.AddParam("@bid", batchid);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet getAcctNosOfAcctCred(string batchid)
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetProcedure("spd_batchcred");
            c.AddParam("@bid", batchid);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static Boolean checkBatchAccsTrtd(string bid)
        {
            bool outcome = false;
            int cnt = 0;
            bool ck = false;

            DataSet ds = getAcctNosOfAcctCred(bid);
            DataTable dt = ds.Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string jay = Convert.ToString(dt.Rows[i]["sub_acct_code"]);
                ck = checkStatusTreated(jay, bid);
                if (ck == true)
                {
                    cnt++;
                }
            }
            if (cnt == dt.Rows.Count)
            {
                outcome = true;
            }
            else
            {
                outcome = false;
            }

            return outcome;
        }

        static DataSet setBatchStatusPartTreated(string batchid)
        {
            string sql = "update batches set status=@status where batch_id=@batch_id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@status", "Partially Treated");
            c.AddParam("@batch_id", batchid);
            DataSet ds = c.Select("rec");

            return ds;
        }

        static DataSet setBatchStatusFailed(string batchid)
        {
            string sql = "update batches set status=@status where batch_id=@batch_id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@status", "Failed");
            c.AddParam("@batch_id", batchid);
            DataSet ds = c.Select("rec");

            return ds;
        }

        static DataSet setBatchStatusTreated(string batchid)
        {
            string sql = "update batches set status=@status where batch_id=@batch_id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@status", "Treated");
            c.AddParam("@batch_id", batchid);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet setAcctStatusTreated(string acct, string batchid)
        {
            string sql = "update trans set Status=@status where sub_acct_code=@sub_acct_code and batch_id=@batch_id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@sub_acct_code", acct);
            c.AddParam("@status", "Treated");
            c.AddParam("@batch_id", batchid);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet getuserName(string batchid)
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetProcedure("spd_getUserMails");
            c.AddParam("@bid", batchid);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static String getUserMail(string name)
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetProcedure("spd_getMailAddress");
            c.AddParam("@nameMail", name);
            DataSet ds = c.Select("rec");
            DataTable dt = ds.Tables[0];
            String n = Convert.ToString(dt.Rows[0]["email"]);
            return n;
        }

        static void sendFailedTrnx(string batchid)
        {
            
            EWSService.ServiceSoapClient mailsender = new EWSService.ServiceSoapClient();
            DataSet ds = getuserName(batchid);
            DataTable dt = ds.Tables[0];

            string authBy = Convert.ToString(dt.Rows[0]["Authorized_By"]);
            string uploadedBy = Convert.ToString(dt.Rows[0]["Uploaded_By"]);
            string totalAmt = Convert.ToString(dt.Rows[0]["Total_Amt"]);
            string status = Convert.ToString(dt.Rows[0]["Status"]);
            string bid = batchid;
            string authMail = getUserMail(authBy);
            string uplMail = getUserMail(uploadedBy);
            string authDate = Convert.ToString(dt.Rows[0]["Authorized_Date"]);
            string sourceMail = "bulktra@sterlingbankng.com";
            string body = "Bulk Transaction with Total Amount of " + totalAmt + " " + status + " for Batch ID: " + bid + "; uploaded by " + uploadedBy + " and Authorized by" + authBy + "; Authorized on " + authDate + "." + "\n" + "\n" + "\n" + "Thank you";
            string subject = "Batch Transaction " + bid + " Report.";
            try
            {
                mailsender.SendMail(authMail, sourceMail, body, subject);
                mailsender.SendMail(uplMail, sourceMail, body, subject);
            }
            catch (Exception e)
            {
                Mylogger.Error(e);
            }

        }


        static void sendPassedTrnx(string batchid)
        {
            
            EWSService.ServiceSoapClient mailsender = new EWSService.ServiceSoapClient();
            DataSet ds = getuserName(batchid);
            DataTable dt = ds.Tables[0];

            string authBy = Convert.ToString(dt.Rows[0]["Authorized_By"]);
            string uploadedBy = Convert.ToString(dt.Rows[0]["Uploaded_By"]);
            string totalAmt = Convert.ToString(dt.Rows[0]["Total_Amt"]);
            string status = Convert.ToString(dt.Rows[0]["Status"]);
            string bid = batchid;
            string authMail = getUserMail(authBy);
            string uplMail = getUserMail(uploadedBy);
            string authDate = Convert.ToString(dt.Rows[0]["Authorized_Date"]);
            string sourceMail = "bulktra@sterlingbankng.com";
            //string body = "Bulk Transaction with Total Amount of " + totalAmt + " " + status + " for Batch ID: " + bid + "; uploaded by " + uploadedBy + " and Authorized by" + authBy + "; Authorized on " + authDate + ".";
            string body = "Bulk Transaction with Total Amount of " + totalAmt + " " + status + " for Batch ID: " + bid + "; uploaded by " + uploadedBy + " and Authorized by" + authBy + "; Authorized on " + authDate + "." + "\n" + "\n" + "\n" + "Thank you";
            string subject = "Batch Transaction " + bid + " Report.";
            try
            {
                
                mailsender.SendMail(authMail, sourceMail, body, subject);
                mailsender.SendMail(uplMail, sourceMail, body, subject);
            }
            catch (Exception e)
            {
                Mylogger.Error(e);
            }
        }

        static Boolean checkStatusTreated(string acct, string batchid)
        {
            bool status = false;
            
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetProcedure("spd_checkTreatedStatus");
                c.AddParam("@sub_acct_code", acct);
                c.AddParam("@batch_id", batchid);
                DataSet ds = c.Select("rec");

                DataTable dt = ds.Tables[0];
                string chck = null;

                chck = Convert.ToString(dt.Rows[0]["status"]);
                if (chck == "Treated")
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Account Status couldn't be checked for: " + acct);
                Mylogger.Info("Account Status couldn't be checked for: " + acct + "\n" + "Error Message: " + ex);
            }
            return status;
        }

        static String postTrnx()
        {
            
            Console.WriteLine("Process Start Time is:" + DateTime.Now);
            Mylogger.Info("Process Start Time is:" + DateTime.Now);
            ImalService.ServicesSoapClient vtel = new ImalService.ServicesSoapClient();
            
            string k = null;
            DataSet dt = null;
           
            DataSet ds = null;
            try
            {
                dt = getBatchIDForPending();
                ds = getPendingImalTxn();
            }
            catch(Exception e)
            {
                Console.WriteLine("Database conection is not available");
                Mylogger.Error("Database conection is not available " + "\n" + e);
            }
            DataTable dat = ds.Tables[0];

            int tb = totalBatchCount();
            decimal suspAmt = 0;
            string frmAcct = null;
            string toAcct = null;
            string desc = null;
            decimal accCred = 0;
            int succCount = 0;
            bool daad = false;
            bool chstat = false;
            int c = 0;

            string resp = null;

            if (tb == 0)
            {
                Console.WriteLine("No Records To Post");
                Mylogger.Info("No Records To Post");
            }

            for (int i = 0; i < tb; i++)
            {
                k = Convert.ToString(dat.Rows[i]["Batch_Id"]);
                DataSet dd = null;
                DataSet dj = null;
                try
                {
                    dj = getAcctNosOfAcctCred(k);
                    dd = getAcctNoOfBatchSuspense(k);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Database conection is not available");
                    Mylogger.Error("Database conection is not available " + "\n" + e);
                }
                DataTable db = dd.Tables[0];
                
                DataTable doob = dj.Tables[0];

                if(db.Rows.Count == 0 && doob.Rows.Count == 0)
                {
                    Console.WriteLine("Batch ID: " + k + "is not an IMAL related batch ID");
                    Mylogger.Info("Batch ID: " + k + " is not an IMAL related batch ID");
                    continue;
                }
                if (doob.Rows.Count == 0)
                {
                    Console.WriteLine("Batch ID: " + k + "does not have any to-Account");
                    Mylogger.Info("Batch ID: " + k + "does not have any to-Account");
                    continue;
                }

                int z = 0;

                while (z < db.Rows.Count)
                {
                    frmAcct = Convert.ToString(db.Rows[z]["sub_acct_code"]);
                    suspAmt = Convert.ToDecimal(db.Rows[z]["tra_amt"]);

                    z++;
                    for (int j = 0; j < doob.Rows.Count; j++)
                    {

                        toAcct = Convert.ToString(doob.Rows[j]["sub_acct_code"]);
                        accCred = Convert.ToDecimal(doob.Rows[j]["tra_amt"]);
                        desc = Convert.ToString(doob.Rows[j]["remarks"]);

                        if (suspAmt < accCred)
                        {

                            string q = Convert.ToString(doob.Rows[0]["batch_id"]);
                            Console.WriteLine("Failed for Account " + toAcct + " of Batch ID: " + q + ". The FROM ACCOUNT's balance is lower than the Transaction Amount.");
                            Mylogger.Info("Failed for Account " + toAcct + " of Batch ID: " + q + ". The FROM ACCOUNT's balance is lower than the Transaction Amount.");
                            continue;

                        }

                        daad = checkStatusTreated(toAcct, k);

                        c++;

                        if (suspAmt >= accCred && daad == false)
                        {
                            try
                            {
                                string q = Convert.ToString(doob.Rows[0]["batch_id"]);
                                resp = vtel.FundstransFer(frmAcct, toAcct, accCred, desc);
                                if (resp.Contains("00*Successful"))
                                {
                                    suspAmt -= accCred;
                                    succCount++;
                                    Console.WriteLine("Treated for Account " + toAcct + " of Batch ID: " + q);
                                    Mylogger.Info("Treated for Account " + toAcct + " of Batch ID: " + q);
                                    setAcctStatusTreated(toAcct, k);
                                }

                                else
                                {
                                    Console.WriteLine("Transaction was not posted for " + toAcct + " Error code is: " + resp);
                                    Mylogger.Error("Transaction was not posted for " + toAcct + " Error is: " + resp);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Cannot connet access IMAL FT module.");
                                Mylogger.Error("Cannot connet access IMAL FT module" + "\n" + "Error Message: " + e.Message.ToString(), e);
                            }
                        }

                    }

                }

                chstat = checkBatchAccsTrtd(k);
                if (chstat == true)
                {
                    setBatchStatusTreated(k);
                    Mylogger.Info("Treated Completely for Batch ID: " + Convert.ToString(db.Rows[0]["batch_id"]));
                    try
                    {
                        sendPassedTrnx(k);
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error(e);
                    }
                    Console.WriteLine();
                    Console.WriteLine("Treated Completely for Batch ID: " + Convert.ToString(db.Rows[0]["batch_id"]));
                    Console.WriteLine();
                    Console.WriteLine();
                }
                else if (succCount > 0 && chstat == false)
                {
                    setBatchStatusPartTreated(k);
                    Mylogger.Info("Not Treated completely for Batch ID: " + Convert.ToString(db.Rows[0]["batch_id"]));
                    try
                    {
                        sendFailedTrnx(k);
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error(e);
                    }
                    Console.WriteLine();
                    Console.WriteLine("Not Treated completely for Batch ID: " + Convert.ToString(db.Rows[0]["batch_id"]));
                    Console.WriteLine();
                    Console.WriteLine();
                }
                else if (succCount == 0  && db.Rows.Count > 0)
                {

                    setBatchStatusFailed(k);

                    Mylogger.Error("Failed completely for Batch ID: " + Convert.ToString(db.Rows[0]["batch_id"]));
                    try
                    {
                        sendFailedTrnx(k);
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error(e);

                    }
                    Console.WriteLine();
                    Console.WriteLine("Failed completely for Batch ID: " + Convert.ToString(db.Rows[0]["batch_id"]));
                    Console.WriteLine();
                    Console.WriteLine();
                }

            }
            
            Mylogger.Info("Process End Time is:" + DateTime.Now);
            return "Done";
        }
    }
}
