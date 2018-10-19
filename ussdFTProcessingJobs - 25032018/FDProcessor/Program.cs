using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FDProcessor
{
    class Program
    {
        public static void updateFinal(Int32 refid, int code)
        {
            Console.WriteLine("");
            Console.WriteLine("Updating record with refid " + refid.ToString() + " ....");
            string sql = "";
            sql = "update tbl_USSD_FD set statusflag = @code, dateprocessed=getdate() where refid = @id";
            Connect c = new Connect(sql, true);
            c.addparam("@code", code);
            c.addparam("@id", refid);
            int cn = c.query();
            if (cn > 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Record updated successfully for customer with refid " + refid.ToString());
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Record was not updated successfully for customer with refid " + refid.ToString());
            }
        }
        static void Main(string[] args)
        {
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'FDProcessor.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    throw new Exception("An instance of this Application is already Running!!!.This instance will be stopped.");
                    return;
                }
                else
                {
                    
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("");
                        Console.WriteLine("Processing new transactions [statusflag=0]....");
                        ds = getPendingReq();
                        processDS(ds);

                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(20000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                    }
                }
            }
            catch
            {

            }
        }
        static DataSet getPendingReq()
        {
            //get all pending transactions to be treated
            string sql = "SELECT refid,mobile,nuban,pin,sessionid,statusflag FROM tbl_USSD_FD where statusflag = 0 ";
            Connect cn = new Connect(sql, true);
            return cn.query("recs");
        }
        static void processDS(DataSet ds)
        {
            char[] sep = { '*' }; Utility g = new Utility(); string FrmName = ""; string sms = "";
            Int32 refid = 0; string mobile = ""; string nuban = ""; string pin = ""; string sessionid = "";
            int cnt = ds.Tables[0].Rows.Count; EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            EACBS1.banksSoapClient ws1 = new EACBS1.banksSoapClient(); string card_Acct = "";
            string PINfrmAccount = "";
            if (cnt == 0)
            {
                
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    refid = Int32.Parse(dr["refid"].ToString());
                    mobile = dr["mobile"].ToString();
                    nuban = dr["nuban"].ToString();
                    pin = dr["pin"].ToString();
                    sessionid = dr["sessionid"].ToString();

                    //check if the mobile number is tied to the NUBAN supplied
                    DataSet ds2 = ws.getAccountFullInfo(nuban);
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr2 = ds2.Tables[0].Rows[0];
                        FrmName = dr2["CUS_SHO_NAME"].ToString();
                    }

                    bool acctFund = g.GetAcctByMobile(mobile, nuban);
                    if (!acctFund)
                    {
                        //set status to 6 account not found
                        updateFinal(refid, 6);
                        //send SMS to the customer to conclude the transaction
                        sms = "Dear " + FrmName + ", The account (" + nuban + ") you sent for the fixed deposit details is not mapped to your account.";
                        g.insertIntoInfobip(sms, mobile);
                        return;
                    }

                    //authenticate the PIN
                    DataSet ds1 = ws1.GetAccountfromDByAccountNum2(nuban);
                    card_Acct = ds1.Tables[0].Rows[0]["ACCT_NO"].ToString();
                    if (card_Acct == "??")
                    {
                        PINfrmAccount = nuban;
                    }
                    else
                    {
                        card_Acct = card_Acct.Replace("??", "*");
                        string[] bits2 = card_Acct.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        card_Acct = bits2[1];

                        if (card_Acct.Length == 18)
                        {
                            PINfrmAccount = bits2[1];
                        }
                    }
                    bool found = g.CardAuthenticate(PINfrmAccount, int.Parse(pin));
                    if (!found)
                    {
                        //set status to 5 wrong PIN
                        updateFinal(refid, 5);
                        //send SMS to the customer to conclude the transaction
                        sms = "Dear " + FrmName + ",\n\r the Last 4 digits of your card you submitted via USSD is not correct. ";
                        g.insertIntoInfobip(sms, mobile);
                        return;
                    }
                    DateTime ArrangementValueDate; decimal DepositCommitment = 0; decimal AccuredDepositInterest = 0;
                    DateTime ArrangementMaturityDate; double totaldays = 0; string ArrangementStatus = "";
                    //get the details to sent to customer
                    DataSet ds3 = null;
                    try
                    {
                        ds3 = ws.getDepositInfo(nuban);
                    }
                    catch
                    {
                        sms = "Dear " + FrmName + ",\n\r there you current do not have any fixed deposit with us";
                        //g.insertIntoInfobip(sms, mobile);
                        return;
                    }
                    if (ds3.Tables[0].Rows.Count > 0)
                    {

                        DataRow dr3 = ds3.Tables[0].Rows[0];
                        ArrangementValueDate = Convert.ToDateTime(dr3["ArrangementValueDate"].ToString());
                        ArrangementMaturityDate = Convert.ToDateTime(dr3["ArrangementMaturityDate"].ToString());
                        ArrangementStatus = dr3["ArrangementStatus"].ToString();
                        //calculate tenor
                        var tenor = ArrangementMaturityDate - ArrangementValueDate;
                        totaldays = tenor.TotalDays;

                        sms = "Dear " + FrmName + ",\n\r" +
                            " Below are the details of your FD \n\r" +
                            " DateBk: " + ArrangementValueDate.ToString("dd-MMM-yyyy") + " \n\r" +
                            " Amt: " + DepositCommitment.ToString() + "\n\r" +
                            " Rate: " + AccuredDepositInterest.ToString() + "\n\r" +
                            " SDate: " + ArrangementValueDate.ToString("dd-MMM-yyyy") + "\n\r" +
                            " Tenor: " + totaldays.ToString("0") + " days \n\r" +
                            " Edate: " + ArrangementMaturityDate.ToString("dd-MMM-yyyy") + "\n\r" +
                            " Int: " + AccuredDepositInterest.ToString() + "\n\r" +
                            " Status: " + ArrangementStatus;
                        g.insertIntoInfobip(sms, mobile);
                        updateFinal(refid, 1);
                    }
                }
            }
        }
    }
}
