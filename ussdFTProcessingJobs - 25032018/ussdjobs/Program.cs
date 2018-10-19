using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ussdjobs.Models;

namespace ussdjobs
{
    class Program
    {
        static void Main(string[] args)
        {
            //modified on 27 Feb 2017
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'ussdjobs.exe'";
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
                        ds = getTransactions();
                        processDS(ds);

                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(1000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                        //treat 
                        ds = getTransactions2();
                        processDS(ds);
                    }
                }
            }
            catch
            {

            }
        }
        public static string getRealMoney(decimal amt)
        {
            string amtval = "";
            amtval = amt.ToString("#,##.00");
            return amtval;
        }
        public static void updaterec(Int32 refid)
        {
            Console.WriteLine("");
            Console.WriteLine("Updating record with refid " + refid.ToString() + " ....");
            string sql = "";
            sql = "update tbl_USSD_transfers set statusflag = 9 where refid = @id";
            Connect c = new Connect(sql, true);
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
        public static void updateFinal(Int32 refid, int code)
        {
            Console.WriteLine("");
            Console.WriteLine("Updating record with refid " + refid.ToString() + " ....");
            string sql = "";
            sql = "update tbl_USSD_transfers set statusflag = @code, dateprocessed=getdate() where refid = @id";
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
        static DataSet getTransactions()
        {
            //get all pending transactions to be treated
            //string sql = "select * from tbl_USSD_transfers where statusflag = 0 " +
            //    " and convert(varchar(50),dateadded,112)  = convert(varchar(50),GETDATE(),112) and trans_type in (1,4) order by dateadded asc";
            string sql = "select * from tbl_USSD_transfers where statusflag = 0 " +
               " and convert(varchar(50),dateadded,112)  = convert(varchar(50),GETDATE(),112) and trans_type in (1,4) order by dateadded desc";
            Connect cn = new Connect(sql, true);
            return cn.query("recs");
        }
        static DataSet getTransactions2()
        {
            //get all pending transactions to be treated
            string sql = "select * from tbl_USSD_transfers where statusflag in (98,42,46) " +
                " and convert(varchar(50),dateadded,112)  = convert(varchar(50),GETDATE(),112) and trans_type in (1,4) order by dateadded asc";
            Connect cn = new Connect(sql, true);
            return cn.query("recs");
        }
        public static string getFromAccount(string acctid, string sid)
        {
            string nuban = "";
            try
            {
                string sql = "select nuban from tbl_USSD_account_id where acctid=@acctid and sessionid =@sid";
                Connect c = new Connect(sql, true);
                c.addparam("@acctid", acctid);
                c.addparam("@sid", sid);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    nuban = dr["nuban"].ToString();
                }
                else
                {
                    nuban = "";
                }
            }
            catch (Exception ex)
            {
                new Errorlog("An error occured " + ex);
            }
            return nuban;
        }
        public static string getCustomerDetails(string nuban)
        {
            string val = "";
            if (nuban.StartsWith("05"))
            {
                imalServ.ServiceSoapClient ws1 = new imalServ.ServiceSoapClient();
                DataSet ds1 = ws1.GetAccountByAccountNumber(nuban);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    string stacode = "";
                    DataRow dr = ds1.Tables[0].Rows[0];
                    stacode = dr["STA_CODE"].ToString();
                    if (stacode == "A")
                    {
                        stacode = "ACTIVE";
                    }
                    val = dr["CUSTOMERNAME"].ToString() + "*" + stacode;
                }
            }
            else
            {
                EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                DataSet ds = ws.getAccountFullInfo(nuban);
                if (ds != null)
                {
                    try
                    {
                        val = ds.Tables[0].Rows[0]["CUS_SHO_NAME"].ToString() + "*" + ds.Tables[0].Rows[0]["STA_CODE"].ToString();
                    }
                    catch
                    {
                        //go to the OverallMapperAcctReplica table and cofirm the record exist.
                        //set staus as Active by default to enable the tranaction to proceed
                        //if account is really not active then T24 will fail the transaciton
                        string sql = "";
                        sql = "Select CUS_SHOW_NAME from OverallMapperAcctReplica where NUBAN = @acct";
                        Connect c = new Connect(sql, true);
                        c.addparam("@acct", nuban);
                        DataSet dsN = c.query("rec");
                        if (dsN.Tables[0].Rows.Count > 0)
                        {
                            val = dsN.Tables[0].Rows[0]["CUS_SHO_NAME"].ToString() + "*" + "ACTIVE";
                        }
                        else
                        {
                            val = "";
                        }
                    }
                }
                else
                {
                    //go to the OverallMapperAcctReplica table and cofirm the record exist.
                    //set staus as Active by default to enable the tranaction to proceed
                    //if account is really not active then T24 will fail the transaciton
                    string sql = "";
                    sql = "Select CUS_SHOW_NAME from OverallMapperAcctReplica where NUBAN = @acct";
                    Connect c = new Connect(sql, true);
                    c.addparam("@acct", nuban);
                    DataSet dsN = c.query("rec");
                    if (dsN.Tables[0].Rows.Count > 0)
                    {
                        val = dsN.Tables[0].Rows[0]["CUS_SHO_NAME"].ToString() + "*" + "ACTIVE";
                    }
                    else
                    {
                        val = "";
                    }
                }
            }
            return val;
        }
        static void processDS(DataSet ds)
        {
            Gadget gg = new Gadget();
            EACBS.banksSoapClient ws1 = new EACBS.banksSoapClient(); char[] sep = { '*' };
            ibs.BSServicesSoapClient ws = new ibs.BSServicesSoapClient();
            StringBuilder rqt = new StringBuilder(); Utility g = new Utility(); string PINfrmAccount = "";
            StringBuilder rsp = new StringBuilder(); string frm_Account = ""; string card_Acct = "";
            Int32 refid = 0; string SessionID = ""; int trans_type = 0; string mobile = "";
            string frmAccount = ""; string toAccount = ""; decimal amt = 0; string custauthid = "";
            string resp = ""; string responseCode = ""; string responseText = ""; string FrmName = "";
            string FrmStatus = ""; string ToName = ""; string ToStatus = ""; string sms = "";
            int cnt = ds.Tables[0].Rows.Count; string BenefiName = ""; string NEResponse = "";
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    //get records into variable
                    Console.WriteLine("");
                    Console.WriteLine("Assigning records to variables....");
                    DataRow dr = ds.Tables[0].Rows[i];
                    refid = Int32.Parse(dr["refid"].ToString()); SessionID = dr["sessionid"].ToString();
                    trans_type = int.Parse(dr["trans_type"].ToString()); mobile = dr["mobile"].ToString();
                    frmAccount = dr["frmAccount"].ToString(); toAccount = dr["toAccount"].ToString();
                    //frm_Account = frmAccount;


                    try
                    {
                        amt = decimal.Parse(dr["amt"].ToString());
                    }
                    catch (Exception ex)
                    {
                        sms = "Dear Valued Customer, your transaction will not be processed because you did not supply any amount.  Kindly retry again and enter amount.";
                        g.insertIntoInfobip(sms, mobile);
                        updateFinal(refid, 50);//Amt was not supplied
                        new Errorlog("Error occured processing transaction with referenceid as no amount was supplied" + refid.ToString() + ex.ToString());
                    }

                    try
                    {
                        custauthid = dr["custauthid"].ToString();
                    }
                    catch (Exception ex)
                    {
                        sms = "Dear Valued Customer, your transaction will not be processed because the last 4 digit you supplied is not correct";
                        g.insertIntoInfobip(sms, mobile);
                        updateFinal(refid, 51);//no last 4 digits supplied
                        new Errorlog("Error occured processing transaction with referenceid as no last 4 digits " + refid.ToString() + ex.ToString());
                    }

                    //get the account to debit
                    //frmAccount = getFromAccount(frmAccount, SessionID);
                    if (string.IsNullOrEmpty(frmAccount))
                    {
                        frmAccount = g.getNubanByMobile(mobile);
                    }
                    if (frmAccount == "")
                    {
                        sms = "Sorry, you will not be able to perform this transaction because you are yet to register for the service.  Kindly dial *822*1*NUBAN#";
                        g.insertIntoInfobip(sms, mobile);
                        new Errorlog("Account not found in Registered account table for MSDN " + mobile);
                        updateFinal(refid, 41);//no account registered
                        continue;
                    }

                    if (frmAccount.StartsWith("05"))
                    {
                        var isMobValid = g.validateMobileAndNuban(frmAccount, mobile);
                        if (!isMobValid)
                        {
                            updateFinal(refid, -15);//mark the record as -15 mobile not on core
                            continue;//by continue means pick the next record
                        }

                        ImalDetails imalInfo = new ImalDetails();
                        imalInfo = g.GetImalDetailsByNuban(frmAccount);
                        FrmName = imalInfo.CustomerName;

                        if (imalInfo.Status != 1)
                        {
                            new Errorlog("Account details not found from Imal DB " + frmAccount);
                            sms = "An error occurred at this time. Kindly try again later.";
                            g.insertIntoInfobip(sms, mobile);
                            updateFinal(refid, -50);//unable to get customer status
                            continue;
                        }

                        //check if PIN trials have been exceeded
                        int pintrialcnt = g.getPinTrails(mobile);
                        if (pintrialcnt == 5)
                        {
                            updateFinal(refid, 6); //pin trials
                            sms = "You have exceeded the allowed number of PIN TRIALS for USSD transaction.  Kindly contact Customer service for assistance. ";
                            g.insertIntoInfobip(sms, mobile);
                            continue;
                        }

                        //authenticate with card table
                        bool found = g.CardAuthenticate(PINfrmAccount, custauthid, frmAccount);
                        if (!found)
                        {
                            //set status to 5
                            updateFinal(refid, 5);
                            g.InsertPINTrails(mobile);
                            //send SMS to the customer to conclude the transaction
                            sms = "Dear " + FrmName + ", your USSD PIN submitted is not correct. ";
                            g.insertIntoInfobip(sms, mobile);
                            continue;
                        }

                        //check if the transaction type is S2S or inter-bank
                        //1 == Imal to Sterling. 4== Merchant (sterling to sterling at the end) 2 == Interbank transfer
                        string BaseUrl = ConfigurationManager.AppSettings["ImalBaseUrl"].ToString();
                        string jsoncontent = "";
                        string referenceCode = g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss");
                        if (trans_type == 1 || trans_type == 4)
                        {
                            string Details2 = "";
                            updateFinal(refid, 10);
                            Details2 = getCustomerDetails(toAccount);
                            string[] bits1 = null;
                            try
                            {
                                bits1 = Details2.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while splitting the details since no record was found for to account " + toAccount);
                                updateFinal(refid, 111);
                                continue;
                            }


                            ToName = bits1[0];
                            BenefiName = bits1[0];
                            ToStatus = bits1[1];
                            if (ToStatus != "ACTIVE")
                            {
                                //set status to 4
                                updateFinal(refid, 4);
                                //send SMS to the customer to conclude the transaction
                                sms = "Dear " + FrmName + ", the Credit account number submitted " + ToStatus + " via USSD is currently not active. ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }
                            if (ToName == "")
                            {
                                //set status to 5
                                updateFinal(refid, 5);
                                //send SMS to the customer to conclude the transaction
                                sms = "Dear " + FrmName + ", the to account " + toAccount + " supplied is not a valid sterling account with via ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }
                            //Sterling to Sterling
                            Console.WriteLine("");
                            Console.WriteLine("Processing Imal(Sterling) to Sterling transaction with Refid " + refid.ToString());
                            //update the record first
                            try
                            {
                                updaterec(refid);
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while processing transaction with refid " + refid.ToString() + " " + ex.ToString());
                                continue;
                            }
                            if (frmAccount == toAccount)
                            {
                                //update statusflag to 49 for same account
                                updateFinal(refid, 49);
                                //send SMS to the customer to conclude the transaction
                                sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful due to same account transaction ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }

                            //**** check to ensure that the

                            decimal maxPerTrans = 0; decimal maxPerday = 0;

                            //1= Trnx Limit for nuban 
                            int trnxLimit = g.GetTrnxLimitStatus(mobile, frmAccount);
                            if (trnxLimit != 0)
                            {
                                DataSet dsLimit = g.GetLimitMINMAXamt();
                                if (dsLimit.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drLimit = dsLimit.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drLimit["minamt"].ToString());
                                    maxPerday = decimal.Parse(drLimit["maxamt"].ToString());
                                }
                            }
                            else
                            {
                                DataSet dsl = g.getMINMAXamt();
                                if (dsl.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drl = dsl.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                                }
                            }

                            //DataSet dsl = g.getMINMAXamt();
                            //if (dsl.Tables[0].Rows.Count > 0)
                            //{
                            //    DataRow drl = dsl.Tables[0].Rows[0];
                            //    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                            //    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                            //}

                            try
                            {
                                if (trnxLimit == 1)
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                    g.getTotalAirtimeDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                                else
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while getting total transactions done per day for customer with mobile " + mobile + " From acct " + frmAccount + " " + ex.ToString());
                                continue;
                            }
                            if (amt > maxPerTrans)
                            {
                                //amt per trans is higher.
                                updateFinal(refid, 13);
                                sms = "Dear " + FrmName + ", your transfer of " + amt + " to " + ToName + " via USSD was not successful due to the limit per transfer. You can only transfer " + maxPerTrans.ToString("#,##.00") + " maximum per transaction ";
                                //sms = "Dear " + FrmName + ",\n\r Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful\n\r due to the amount per transfer.  You can only transfer 5,000 maximum per transfer ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }

                            if (trnxLimit == 1 && amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum amount for today via USSD. You can only use a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }

                            if (amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum transfer for today via USSD. You can only transfer a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }


                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(BaseUrl);
                                string apipath = "api/Imal/LocalFT";
                                Imal_LocalFT_Req req = new Imal_LocalFT_Req();
                                req.amount = amt.ToString();
                                req.toAccount = toAccount;
                                req.fromAccount = frmAccount;
                                req.requestCode = "1";//Not really needed
                                req.principalIdentifier = "";//Not really needed
                                req.referenceCode = referenceCode;
                                req.beneficiaryName = ToName;

                                string input = JsonConvert.SerializeObject(req);
                                var imalref = g.InsertImalRequest(SessionID, input, "LocalFT(Intra-bank)");
                                var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");

                                //var result = client.(apipath, cont).Result;
                                var result = client.PostAsync(apipath, cont).Result;
                                //var result = await client.PostAsync(apipath, cont);
                                //jsoncontent = result.Content.ToString();
                                jsoncontent = result.Content.ReadAsStringAsync().Result;
                                //jsoncontent = await result.Content.ReadAsStringAsync();
                                jsoncontent = jsoncontent.Replace(@"\", "");
                                jsoncontent = jsoncontent.Replace("\"{", "{");
                                jsoncontent = jsoncontent.Replace("}\"", "}");
                                var imalResp = new Imal_LocalFT_Resp();
                                try
                                {
                                     imalResp = JsonConvert.DeserializeObject<Imal_LocalFT_Resp>(jsoncontent);
                                }
                                catch (Exception ex)
                                {
                                    new Errorlog("Exception getting results for localFT " + ex);                                   
                                }
                                g.UpdateImalWithResponse(imalref, jsoncontent, imalResp.responseCode);
                                if (imalResp.responseCode == "0")
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("Transaction completed successfully for Refid " + refid.ToString());
                                    //update statusflag to 1
                                    updateFinal(refid, 1);
                                    //send SMS to the customer to conclude the transaction
                                    //sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was successful ";
                                    //g.insertIntoInfobip(sms, mobile); " + FrmName + " to " + ToName + " via USSD was not successful.";
                                    //g.insertIntoInfobip(sms,
                                }
                                else
                                {
                                    if (imalResp.responseCode == "-100030")
                                    {
                                        updateFinal(refid, 2);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful.";

                                    }
                                    else
                                    {
                                        updateFinal(refid, 2);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                }
                            }
                        }
                        else if (trans_type == 2)
                        {
                            updateFinal(refid, 10);
                            string BANKCODE = "";
                            BANKCODE = g.getBankcode(SessionID);

                            //Do interbank name enquiry for imal
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(BaseUrl);
                                string apipath = "api/Imal/NIPNameEnquiry";
                                Imal_NIP_NameEnqReq req = new Imal_NIP_NameEnqReq();
                                req.requestCode = "228";
                                req.principalIdentifier = "002";
                                req.referenceCode = referenceCode;
                                req.destinationBankCode = BANKCODE;
                                req.channelCode = "2";
                                req.account = toAccount;

                                string input = JsonConvert.SerializeObject(req);
                                var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
                                var imalref = g.InsertImalRequest(SessionID, input, "Inter-bank(Name enquiry)");

                                var result = client.PostAsync(apipath, cont).Result;
                                //var result = await client.PostAsync(apipath, cont);
                                jsoncontent = result.Content.ToString();
                                //jsoncontent = await result.Content.ReadAsStringAsync();
                                jsoncontent = jsoncontent.Replace(@"\", "");
                                jsoncontent = jsoncontent.Replace("\"{", "{");
                                jsoncontent = jsoncontent.Replace("}\"", "}");
                                var imalResp = new Imal_NIP_NameEnqResp();
                                try
                                {
                                    imalResp = JsonConvert.DeserializeObject<Imal_NIP_NameEnqResp>(jsoncontent);
                                }
                                catch (Exception ex)
                                {
                                    new Errorlog("Exception occurred getting resp for Name enq: " + ex);
                                }
                                g.UpdateImalWithResponse(imalref, jsoncontent, imalResp.responseCode);

                                if (imalResp.responseCode == "0")
                                {
                                    ToName = imalResp.nameDetails;
                                    BenefiName = imalResp.nameDetails;
                                }
                                else
                                {
                                    updateFinal(refid, 98);//error occured during Name Enquiry
                                                           //sms = "Dear " + FrmName + ",\n\r An error occured during Name Equiry Via USSD.  Kindly retry ";
                                                           //g.insertIntoInfobip(sms, mobile);
                                    new Errorlog("Error occured during name enquiry " + imalResp.errorCode);
                                }
                            }

                            //**** check to ensure that the customer isn't over the transaction limit

                            decimal maxPerTrans = 0; decimal maxPerday = 0;

                            //1= Trnx Limit for nuban 
                            int trnxLimit = g.GetTrnxLimitStatus(mobile, frmAccount);
                            if (trnxLimit != 0)
                            {
                                DataSet dsLimit = g.GetLimitMINMAXamt();
                                if (dsLimit.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drLimit = dsLimit.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drLimit["minamt"].ToString());
                                    maxPerday = decimal.Parse(drLimit["maxamt"].ToString());
                                }
                            }
                            else
                            {
                                DataSet dsl = g.getMINMAXamt();
                                if (dsl.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drl = dsl.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                                }
                            }

                            try
                            {
                                if (trnxLimit == 1)
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                    g.getTotalAirtimeDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                                else
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while getting total transactions done per day for customer with mobile " + mobile + " From acct " + frmAccount + " " + ex.ToString());
                                continue;
                            }
                            if (amt > maxPerTrans)
                            {
                                //amt per trans is higher.
                                updateFinal(refid, 13);
                                sms = "Dear " + FrmName + ", your transfer of " + amt + " to " + ToName + " via USSD was not successful due to the limit per transfer. You can only transfer " + maxPerTrans.ToString("#,##.00") + " maximum per transaction ";
                                //sms = "Dear " + FrmName + ",\n\r Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful\n\r due to the amount per transfer.  You can only transfer 5,000 maximum per transfer ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }

                            if (trnxLimit == 1 && amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum amount for today via USSD. You can only use a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }

                            if (amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum transfer for today via USSD. You can only transfer a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }

                            string paymtref = "Transfer of " + amt + " from " + FrmName.Trim() + " to " + ToName + ". USSD ref: " + referenceCode;
                            if (paymtref.Length > 100)
                            {
                                paymtref = paymtref.Substring(1, 100);
                            }
                            using (var client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(BaseUrl);
                                string apipath = "api/Imal/NIPFundsTransfer";
                                Imal_NIP_FtReq req = new Imal_NIP_FtReq();
                                req.amount = amt.ToString();
                                req.toAccount = toAccount;
                                req.fromAccount = frmAccount;
                                req.requestCode = "1";
                                req.principalIdentifier = "002";
                                req.referenceCode = referenceCode;
                                req.beneficiaryName = ToName;
                                req.channelCode = "2";
                                req.customerShowName = FrmName;
                                req.beneBVN = "";
                                req.paymentReference = paymtref;

                                string input = JsonConvert.SerializeObject(req);
                                var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
                                var imalref = g.InsertImalRequest(SessionID, input, "Inter-bank Transfer");

                                //var result = await client.PostAsync(apipath, cont);
                                //jsoncontent = await result.Content.ReadAsStringAsync();
                                var result = client.PostAsync(apipath, cont).Result;
                                jsoncontent = result.Content.ToString();
                                var imalResp = new Imal_NIP_FtResp();
                                try
                                {
                                imalResp = JsonConvert.DeserializeObject<Imal_NIP_FtResp>(jsoncontent);
                                }
                                catch (Exception ex)
                                {
                                    new Errorlog("Exception occurred reading" + ex);
                                }
                                g.UpdateImalWithResponse(imalref, jsoncontent, imalResp.responseCode);
                                if (imalResp.responseCode == "0")
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("Transaction completed successfully for Refid " + refid.ToString());
                                    //update statusflag to 1
                                    updateFinal(refid, 1);
                                    //send SMS to the customer to conclude the transaction
                                    sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was successful ";
                                    g.insertIntoInfobip(sms, mobile);
                                }
                                else
                                {
                                    if (imalResp.responseCode == "-100030")
                                    {
                                        updateFinal(refid, 2);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else
                                    {
                                        updateFinal(refid, 2);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                }
                            }


                        }

                    }
                    else
                    {
                        //check if the mobile is the same as what we have in T24
                        string t24Mob = ""; string Ledcodeval = "";
                        DataSet dea = ws1.getAccountFullInfo(frmAccount);
                        if (dea.Tables[0].Rows.Count > 0)
                        {
                            DataRow dre = dea.Tables[0].Rows[0];
                            t24Mob = dre["MOB_NUM"].ToString();
                            t24Mob = gg.ConvertMobile234(t24Mob);
                            Ledcodeval = dre["T24_LED_CODE"].ToString();
                        }
                        else
                        {
                            t24Mob = "";
                        }

                        if (t24Mob.Trim() != mobile)
                        {
                            updateFinal(refid, -15);//mark the record as -15 mobile not on core
                            continue;//by continue means pick the next record
                        }
                        if (Ledcodeval == "1200" || Ledcodeval == "1201" || Ledcodeval == "1202" || Ledcodeval == "1203")
                        {
                            updateFinal(refid, -16);//Corporate customer is not allowed to use this Service
                            continue;//by continue means pick the next record
                        }
                        //check if customer is found in blacklist table
                        bool isCusBlacklisted = gg.isFoundInBLackList(mobile);
                        if (isCusBlacklisted)
                        {
                            updateFinal(refid, -14);//mark the record as -14 as blacklist
                            continue;//by continue means pick the next record
                        }



                        //******************* This is not needed again since we will be getting cardauth from the registration table ********
                        //get card account number from EACBS
                        //DataSet ds1 = null;
                        //ds1 = ws1.GetAccountfromDByAccountNum2(frmAccount);
                        //try
                        //{
                        //    if (ds1.Tables[0].Rows.Count > 0)
                        //    {
                        //        card_Acct = ds1.Tables[0].Rows[0]["ACCT_NO"].ToString();
                        //        card_Acct = card_Acct.Replace("??", "*");
                        //        string[] bits2 = card_Acct.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        //        try
                        //        {
                        //            card_Acct = bits2[1];
                        //        }
                        //        catch
                        //        {
                        //            card_Acct = frmAccount;
                        //        }

                        //        if (card_Acct.Length == 18)
                        //        {
                        //            PINfrmAccount = bits2[1];
                        //        }
                        //    }
                        //    else
                        //    {
                        //        new Errorlog("Account details not found from EACBS GetAccountfromDByAccountNum2 for frmAccount " + frmAccount);
                        //        updateFinal(refid, 46);//no account registered
                        //        continue;
                        //    }
                        //}
                        //catch(Exception ex)
                        //{
                        //    new Errorlog("Error occured " + ex);
                        //    updateFinal(refid, 42);//issue occured
                        //    continue;
                        //}
                        //*****************************************************************************************************************


                        //check if PIN trials have been exceeded
                        int pintrialcnt = g.getPinTrails(mobile);
                        if (pintrialcnt == 5)
                        {
                            updateFinal(refid, 6); //pin trials
                            sms = "You have exceeded the allowed number of PIN TRIALS for USSD transaction.  Kindly contact Customer service for assistance. ";
                            g.insertIntoInfobip(sms, mobile);
                            continue;
                        }

                        //get the from name and to name
                        string Details1 = getCustomerDetails(frmAccount);
                        if (Details1 == "") //if the detail is empty meaning i
                        {
                            new Errorlog("Account details not found from EACBS GetAccountfull info " + frmAccount);
                            updateFinal(refid, 46);//no account registered
                            continue;
                        }
                        string[] bits = Details1.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        FrmName = bits[0]; //customer name
                        try
                        {
                            FrmStatus = bits[1];//customer account status
                        }
                        catch
                        {
                            new Errorlog("Account details not found from EACBS GetAccountfull info " + frmAccount);
                            sms = "An error occurred at this time. Kindly try again later.";
                            g.insertIntoInfobip(sms, mobile);
                            updateFinal(refid, -50);//unable to get customer status
                            continue;
                        }
                        //authenticate with card table
                        bool found = g.CardAuthenticate(PINfrmAccount, custauthid, frmAccount);
                        if (!found)
                        {
                            //set status to 5
                            updateFinal(refid, 5);
                            g.InsertPINTrails(mobile);
                            //send SMS to the customer to conclude the transaction
                            sms = "Dear " + FrmName + ", your USSD PIN submitted is not correct. ";
                            g.insertIntoInfobip(sms, mobile);
                            continue;
                        }

                        //check if the fromName is empty
                        if (FrmStatus != "ACTIVE")
                        {
                            //set status to 3
                            updateFinal(refid, 3);
                            //send SMS to the customer to conclude the transaction
                            sms = "Dear " + FrmName + ", the debit account number submitted via USSD is currently not active. ";
                            g.insertIntoInfobip(sms, mobile);
                            continue;
                        }
                        //check if the transaction type is S2S or inter-bank
                        //1 == Sterling to Sterling. 4== Merchant (sterling to sterling at the end) 2 == Interbank transfer
                        if (trans_type == 1 || trans_type == 4)
                        {
                            string Details2 = "";
                            updateFinal(refid, 10);
                            Details2 = getCustomerDetails(toAccount);
                            string[] bits1 = null;
                            try
                            {
                                bits1 = Details2.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while splitting the details since no record was found for to account " + toAccount);
                                updateFinal(refid, 111);
                                continue;
                            }


                            ToName = bits1[0];
                            ToStatus = bits1[1];
                            if (ToStatus != "ACTIVE")
                            {
                                //set status to 4
                                updateFinal(refid, 4);
                                //send SMS to the customer to conclude the transaction
                                sms = "Dear " + FrmName + ", the Credit account number submitted " + ToStatus + " via USSD is currently not active. ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }
                            if (ToName == "")
                            {
                                //set status to 5
                                updateFinal(refid, 5);
                                //send SMS to the customer to conclude the transaction
                                sms = "Dear " + FrmName + ", the to account " + toAccount + " supplied is not a valid sterling account with via ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }
                            //Sterling to Sterling
                            Console.WriteLine("");
                            Console.WriteLine("Processing Sterling to Sterling transaction with Refid " + refid.ToString());
                            //update the record first
                            try
                            {
                                updaterec(refid);
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while processing transaction with refid " + refid.ToString() + " " + ex.ToString());
                                continue;
                            }
                            if (frmAccount == toAccount)
                            {
                                //update statusflag to 49 for same account
                                updateFinal(refid, 49);
                                //send SMS to the customer to conclude the transaction
                                sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful due to same account transaction ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }

                            //**** check to ensure that the

                            decimal maxPerTrans = 0; decimal maxPerday = 0;

                            //1= Trnx Limit for nuban 
                            int trnxLimit = g.GetTrnxLimitStatus(mobile, frmAccount);
                            if (trnxLimit != 0)
                            {
                                DataSet dsLimit = g.GetLimitMINMAXamt();
                                if (dsLimit.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drLimit = dsLimit.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drLimit["minamt"].ToString());
                                    maxPerday = decimal.Parse(drLimit["maxamt"].ToString());
                                }
                            }
                            else
                            {
                                DataSet dsl = g.getMINMAXamt();
                                if (dsl.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drl = dsl.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                                }
                            }

                            //DataSet dsl = g.getMINMAXamt();
                            //if (dsl.Tables[0].Rows.Count > 0)
                            //{
                            //    DataRow drl = dsl.Tables[0].Rows[0];
                            //    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                            //    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                            //}

                            try
                            {
                                if (trnxLimit == 1)
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                    g.getTotalAirtimeDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                                else
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("An error occured while getting total transactions done per day for customer with mobile " + mobile + " From acct " + frmAccount + " " + ex.ToString());
                                continue;
                            }
                            if (amt > maxPerTrans)
                            {
                                //amt per trans is higher.
                                updateFinal(refid, 13);
                                sms = "Dear " + FrmName + ", your transfer of " + amt + " to " + ToName + " via USSD was not successful due to the limit per transfer. You can only transfer " + maxPerTrans.ToString("#,##.00") + " maximum per transaction ";
                                //sms = "Dear " + FrmName + ",\n\r Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful\n\r due to the amount per transfer.  You can only transfer 5,000 maximum per transfer ";
                                g.insertIntoInfobip(sms, mobile);
                                continue;
                            }

                            if (trnxLimit == 1 && amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum amount for today via USSD. You can only use a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }

                            if (amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum transfer for today via USSD. You can only transfer a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }
                            string USSDRef = "";
                            USSDRef = g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss");
                            //proceed once update is successful
                            rqt.Clear();
                            rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                            rqt.Append("<IBSRequest>");
                            rqt.Append("<ReferenceID>" + USSDRef + "</ReferenceID>");
                            rqt.Append("<RequestType>" + "102" + "</RequestType>");
                            rqt.Append("<FromAccount>" + frmAccount + "</FromAccount>");
                            rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                            rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                            rqt.Append("<PaymentReference>" + "Transfer of " + amt + " from " + FrmName + " to " + ToName + "USSD Ref " + USSDRef + "</PaymentReference>");
                            rqt.Append("</IBSRequest>");
                            string str = "";
                            str = rqt.ToString();
                            //str = g.Encrypt(str, 26);

                            //send to web service for processing
                            Console.WriteLine("");
                            Console.WriteLine("Sending to IBS service Refid " + refid.ToString());

                            try

                            {
                                //resp = ws.IBSBridge(str, 26);
                                resp = ws.IBSBridgeNE(str, 26);
                                //resp = g.Decrypt(resp, 26);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(resp);
                                responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                                responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                                if (responseCode == "00")
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("Transaction completed successfully for Refid " + refid.ToString());
                                    //update statusflag to 1
                                    updateFinal(refid, 1);
                                    //send SMS to the customer to conclude the transaction
                                    //sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was successful ";
                                    //g.insertIntoInfobip(sms, mobile);
                                }
                                else
                                {
                                    if (responseCode == "x1000")
                                    {
                                        //update statusflag 1000. Post no debit
                                        updateFinal(refid, 1000);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account has POST No Debit restriction.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else if (responseCode == "x1001")
                                    {
                                        //update statusflag to 1001. Incomplete Documentation
                                        updateFinal(refid, 1001);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account has Incomplete Documentation.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else if (responseCode == "x1002")
                                    {
                                        //update statusflag to 1002. BVN Restriction
                                        updateFinal(refid, 1002);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account has BVN Restriction.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else if (responseCode == "x1003")
                                    {
                                        //update statusflag to 1003. Dormant Account Restriction
                                        updateFinal(refid, 1003);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account is Dormant.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else if (responseCode == "x1004")
                                    {
                                        //update statusflag to 1004. Failed Address Verification
                                        updateFinal(refid, 1004);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account has Failed Address Verification Restriction.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else if (responseCode == "x1005")
                                    {
                                        //update statusflag to 1005. Unauthorised overdraf
                                        updateFinal(refid, 1005);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account has Unauthorised overdraft Restriction.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else if (responseCode == "x1006")
                                    {
                                        //update statusflag to 1006. Inactive Account Restriction
                                        updateFinal(refid, 1006);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful because your account has Inactive Account Restriction.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                    else
                                    {
                                        updateFinal(refid, 2);
                                        //failed
                                        sms = "Dear " + FrmName + ", Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful.";
                                        g.insertIntoInfobip(sms, mobile);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("Error occured while processing the transaction with refid " + refid.ToString() + " for account " + frmAccount + " " + ex.ToString());
                                return;
                            }
                        }
                        else if (trans_type == 2)
                        {
                            //mark as sent for processing
                            updateFinal(refid, 10);

                            //**** check to ensure that the

                            decimal maxPerTrans = 0; decimal maxPerday = 0;

                            //1= Trnx Limit for nuban 
                            int trnxLimit = g.GetTrnxLimitStatus(mobile, frmAccount);
                            if (trnxLimit != 0)
                            {
                                DataSet dsLimit = g.GetLimitMINMAXamt();
                                if (dsLimit.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drLimit = dsLimit.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drLimit["minamt"].ToString());
                                    maxPerday = decimal.Parse(drLimit["maxamt"].ToString());
                                }
                            }
                            else
                            {
                                DataSet dsl = g.getMINMAXamt();
                                if (dsl.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drl = dsl.Tables[0].Rows[0];
                                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                                }
                            }

                            try
                            {
                                if (trnxLimit == 1)
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                    g.getTotalAirtimeDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                                else
                                {
                                    g.getTotalTransDonePerday(amt, mobile, SessionID, maxPerday);
                                }
                            }
                            catch (Exception ex)
                            {
                                new Errorlog("Error occured while fetching total transaction done perday for interbank for account " + frmAccount + " " + ex.ToString());
                            }
                            if (amt > maxPerTrans)
                            {
                                //amt per trans is higher.
                                updateFinal(refid, 13);
                                //sms = "Dear " + FrmName + ",\n\r Transfer of " + amt + " from " + FrmName + " to " + ToName + " via USSD was not successful\n\r due to the amoun per transfer.  You can only transfer 5,000 maximum per transfer ";
                                sms = "Dear " + FrmName + ", your transfer of " + amt + " to " + ToName + " via USSD was not successful due to the limit per transfer. You can only transfer " + maxPerTrans.ToString("#,##.00") + " maximum per transaction ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }

                            if (trnxLimit == 1 && amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum amount for today via USSD. You can only use a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }

                            if (amt + g.Totaldone > maxPerday)
                            {
                                updateFinal(refid, 61);
                                sms = "Dear " + FrmName + ", you have exceeded your maximum transfer for today via USSD. You can only transfer a total of " + maxPerday.ToString("#,##.00") + " maximum per day ";
                                g.insertIntoInfobip(sms, mobile);
                                return;
                            }
                            //Inter-bank 
                            string BANKCODE = "";
                            Console.WriteLine("");
                            Console.WriteLine("Processing Inter-bank(NE) transaction with Refid " + refid.ToString());
                            //pass the sessionid to get the bankcode
                            BANKCODE = g.getBankcode(SessionID);
                            string str = "";
                            //first do name enquiry
                            rqt.Clear();
                            rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                            rqt.Append("<IBSRequest>");
                            rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                            rqt.Append("<RequestType>" + "105" + "</RequestType>");
                            rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                            rqt.Append("<DestinationBankCode>" + BANKCODE + "</DestinationBankCode>");
                            rqt.Append("</IBSRequest>");

                            str = rqt.ToString();
                            str = g.Encrypt(str, 26);

                            XmlDocument xmlDoc = new XmlDocument();
                            try
                            {
                                resp = ws.IBSBridge(str, 26);
                                resp = g.Decrypt(resp, 26);
                                xmlDoc.LoadXml(resp);
                                SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
                                BenefiName = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;
                                NEResponse = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                            }
                            catch (Exception ex)
                            {
                                updateFinal(refid, 98);//error occured during Name Enquiry
                                                       //sms = "Dear " + FrmName + ",\n\r An error occured during Name Equiry Via USSD.  Kindly retry ";
                                                       //g.insertIntoInfobip(sms, mobile);
                                new Errorlog("Error occured during name enquiry " + ex);
                            }
                            Console.WriteLine("Processing Inter-bank(NE) transaction with Refid Completed" + refid.ToString());
                            resp = ""; string paymtref = "";
                            //if NE is successful
                            if (NEResponse == "00")
                            {
                                Console.WriteLine("Processing Inter-bank(FT) transaction with Refid " + refid.ToString());
                                paymtref = "Trf " + amt + " frm " + FrmName.Trim() + " to " + BenefiName;
                                if (paymtref.Length > 100)
                                {
                                    paymtref = paymtref.Substring(1, 100);
                                }
                                //SessionID = g.newSessionId(BANKCODE);
                                NEResponse = "00";
                                rqt.Clear();
                                rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                                rqt.Append("<IBSRequest>");
                                rqt.Append("<SessionID>" + SessionID + "</SessionID>");
                                rqt.Append("<ReferenceID>" + g.GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                                rqt.Append("<RequestType>" + "101" + "</RequestType>");
                                rqt.Append("<FromAccount>" + frmAccount + "</FromAccount>");
                                rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                                rqt.Append("<DestinationBankCode>" + BANKCODE + "</DestinationBankCode>");
                                rqt.Append("<Amount>" + amt + "</Amount>");
                                rqt.Append("<NEResponse>" + NEResponse + "</NEResponse>");
                                rqt.Append("<BenefiName>" + BenefiName + "</BenefiName>");
                                rqt.Append("<PaymentReference>" + paymtref + "</PaymentReference>");
                                rqt.Append("</IBSRequest>");

                                str = rqt.ToString();
                                str = g.Encrypt(str, 26);
                                try
                                {
                                    resp = ws.IBSBridge(str, 26);
                                    resp = g.Decrypt(resp, 26);
                                }
                                catch
                                {
                                    Console.WriteLine("Processing Inter-bank(FT) transaction with Refid Completed with error" + refid.ToString());
                                    new Errorlog("Inter-bank Response returned for account " + frmAccount + " with sessionid " + SessionID + " is " + resp);
                                    updateFinal(refid, -1); //success
                                                            //send SMS to the customer to conclude the transaction
                                    sms = "Dear " + FrmName + ", Inter-bank Transfer of " + amt + " from " + FrmName + " to " + BenefiName + " via USSD encountered an error while processing.  Kindly check your balance before carrying out another transaction ";
                                    g.insertIntoInfobip(sms, mobile);
                                    continue;
                                }


                                //string[] bits1 = resp.Split(':');
                                //if (bits1[0] == "00")
                                //{
                                //resp = bits1[1];
                                try
                                {
                                    xmlDoc.LoadXml(resp);
                                    responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                                    responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                                    if (responseCode == "00")
                                    {
                                        Console.WriteLine("Processing Inter-bank(FT) transaction with Refid Completed" + refid.ToString());
                                        updateFinal(refid, 1); //success
                                                               //send SMS to the customer to conclude the transaction
                                                               //sms = "Dear " + FrmName + ", Inter-bank Transfer of " + amt + " from " + FrmName + " to " + BenefiName + " via USSD was successful ";
                                                               //g.insertIntoInfobip(sms, mobile);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Console.WriteLine("Processing Inter-bank(FT) transaction with Refid Completed Not successful " + refid.ToString());
                                            new Errorlog("Inter bank response from NIBSS " + responseCode + " for account " + frmAccount);
                                            updateFinal(refid, 2);
                                            sms = "Dear " + FrmName + ", Inter-bank Transfer of " + amt + " from " + FrmName + " to " + BenefiName + " via USSD was not successful ";
                                            g.insertIntoInfobip(sms, mobile);
                                        }
                                        catch (Exception ex)
                                        {
                                            new Errorlog("Error==>Inter bank response from NIBSS " + responseCode + " for account " + frmAccount);
                                            continue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Processing Inter-bank(FT) transaction with Refid Completed Not successful " + refid.ToString());
                                    sms = "Dear " + FrmName + ", Inter-bank error has occured.  Kindly check your account balance before you try again";
                                    g.insertIntoInfobip(sms, mobile);
                                    updateFinal(refid, 22);
                                    //new Errorlog("Error occured for Inter-bank FT transfer " + ex);
                                    new Errorlog("Error occured for Inter-bank FT transfer " + ex + " for account " + frmAccount + " with sessionid " + SessionID + " is " + resp);
                                    continue;
                                }
                                //}
                                // else
                                //{

                                // }//end of bits1
                            }
                            else
                            {
                                sms = "Dear " + FrmName + ", Inter-bank error has occured.  Kindly check your account balance before you try again";
                                g.insertIntoInfobip(sms, mobile);
                                updateFinal(refid, 22);
                                new Errorlog("NIBSS Response for Interbank " + NEResponse);
                                updateFinal(refid, int.Parse(NEResponse));
                            }
                        }
                    }
                }
            }
        }
    }
}
