using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.IO;
using System.Threading;
using com.sbpws.utility;



namespace CustomerAccount
{
    public class CustomerAccount_DAL
    {
        public static SqlConnection GetAppConnection()
        {
            string configCon = ConfigurationManager.AppSettings["mssqlconn"];
            return new SqlConnection(configCon);
        }

        public static string getAccounts( AccountLookupInfo info)
        {
            string[] exclude_ledcode = new string[] {"1282","4018" };
            string[] savings_ledcode = new string[] { "84", "73", "51", "95", "65", "59","95" };
            string[] current_ledcode = new string[] { "1", "5051", "5052", "5054", "9", "5079", "5082", "5060" };
            string logFormat = "session:#SI \r\nAccountName:#AN \r\nAccount Nos:#ACT \r\nMobileNo:#MN \r\nCustomerNumber:#CN \r\nDateRequest:#DR";
            // File.ReadAllText("LogTemplate.txt");

            string resp = "";
            try
            {
                
                    svcBanks.ServiceSoapClient sc = new svcBanks.ServiceSoapClient();
                    DataSet ds = sc.GetAccountsByMobileNo(info.ClientMobileNo);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                var dt_distinct = (from dr in ds.Tables[0].AsEnumerable()
                                                   select new
                                                   {
                                                       MOB_NUM = dr.Field<object>("MOB_NUM")
                                                       ,
                                                       TEL_NUM = dr.Field<object>("TEL_NUM")
                                                       ,
                                                       TEL_NUM_2 = dr.Field<object>("TEL_NUM_2")

                                                      , BRA_CODE = dr.Field<object>("BRA_CODE")
                                                       ,
                                                       CUS_NUM = dr.Field<object>("CUS_NUM")
                                                        ,
                                                       CUR_CODE = dr.Field<object>("CUR_CODE")
                                                        ,
                                                       LED_CODE = dr.Field<object>("LED_CODE")
                                                        ,
                                                       SUB_ACCT_CODE = dr.Field<object>("SUB_ACCT_CODE")
                                                        ,
                                                       CUS_SHO_NAME = dr.Field<object>("CUS_SHO_NAME")
                                                        ,
                                                       MAP_ACC_NO = dr.Field<object>("MAP_ACC_NO")
                                                   }).Distinct().ToList();


                                DataTable dtx = CustomerAccount_Util.ToDataTable(dt_distinct);
                                string txt = "";

                                foreach (DataRow dr in dtx.Rows)
                                {
                                    if (txt.Length >= 130)
                                    { 

                                    }
                                    else
                                    {
                                        //if (!exclude_ledcode.Any(x => x == dr["LED_CODE"].ToString()))
                                        //{
                                        //    txt = txt + "" + dr["MAP_ACC_NO"].ToString() + ";";
                                        //}

                                        if (dr["CUR_CODE"].ToString() == "1")
                                        {
                                            if (current_ledcode.Any(x => x == dr["LED_CODE"].ToString()))
                                            {
                                                txt = txt + "(CRNT)" + dr["MAP_ACC_NO"].ToString() + ";";
                                            }

                                            if (savings_ledcode.Any(x => x == dr["LED_CODE"].ToString()))
                                            {
                                                txt = txt + "(SAV)" + dr["MAP_ACC_NO"].ToString() + ";";
                                            }
                                        }

                                        info.CustomerId = dr["CUS_NUM"].ToString();
                                        info.CustomerName = dr["CUS_SHO_NAME"].ToString();                                        
                                    }

                                    #region MyRegion
                                    //string prodCode = dr["LED_CODE"].ToString();
                                    //string ledlist = "00,01,02,04";
                                    //var v = ledlist.Where(x => x.Equals(""));

                                    //try
                                    //{
                                    //    SqlParameter[] parameters = {
                                    //   new SqlParameter("@CustomerId",info.ClientMobileNo)
                                    //  ,new SqlParameter("@CustomerName",info.ClientMobileNo)
                                    //  ,new SqlParameter("@MobileNo",info.ClientMobileNo)
                                    //  ,new SqlParameter("@AccountType",info.ClientMobileNo)
                                    //  ,new SqlParameter("@Dateopen",info.ClientMobileNo)
                                    //  ,new SqlParameter("@AccountNumber",info.ClientMobileNo)
                                    //  ,new SqlParameter("@SessionId",info.ClientMobileNo)
                                    //  ,new SqlParameter("@ClientMobileNo",info.ClientMobileNo)
                                    //   ,new SqlParameter("@DateRequest",info.ClientMobileNo)
                                    //  ,new SqlParameter("@RequestStatue",info.ClientMobileNo)
                                    //  ,new SqlParameter("@RequestMessage",info.ClientMobileNo)
                                    //  ,new SqlParameter("@DateRespond",info.ClientMobileNo)

                                    //};
                                    //    string sql = " INSERT INTO [dbo].[USSD_Account_Lookup] ([CustomerId],[CustomerName],[MobileNo],[AccountType],[Dateopen],[AccountNumber],[SessionId],[ClientMobileNo],[DateRequest],[RequestStatue],[RequestMessage],[DateRespond]) VALUES (@CustomerId,@CustomerName,@MobileNo,@AccountType,@Dateopen,@AccountNumber,@SessionId,@ClientMobileNo,@DateRequest,@RequestStatue,@RequestMessage ,@DateRespond)";

                                    //    int val = (int)SqlHelper.ExecuteScalar(GetAppConnection(), CommandType.Text, sql, parameters);
                                    //  procLog = string.Format("Request for BVN:{0} with Account:{1} respond with:{2}", info.ClientMobileNo, dr["MAP_ACC_NO"], val);

                                    //}
                                    //catch (SqlException ex)
                                    //{

                                    //    //procLog = string.Format("Request for BVN:{0} with Account:{1} respond with:{2}", info.ClientMobileNo, dr["MAP_ACC_NO"], ex.Message);
                                    //    //ApplicationLog_ log = new ApplicationLog_(procLog, "bvnlog", info.ClientMobileNo);
                                    //} 
                                    #endregion
                                }

                                logFormat = logFormat.Replace("#SI", info.SessionId);
                                logFormat = logFormat.Replace("#AN", info.CustomerName );
                                logFormat = logFormat.Replace("#ACT", txt);
                                logFormat = logFormat.Replace("#MN", info.ClientMobileNo);
                                logFormat = logFormat.Replace("#CN", info.CustomerId);
                               // logFormat = logFormat.Replace("#BC", info.BranchCode);
                              //  logFormat = logFormat.Replace("#CC", info.CurrencyCode);
                              // logFormat = logFormat.Replace("#PC", info.ProductCode);
                               // logFormat = logFormat.Replace("#AS", info.ProductSequence);
                                logFormat = logFormat.Replace("#DR", DateTime.Now.ToString());
                                resp = string.Format("Your NGN Accounts are:{0}", txt);
                                ApplicationLog_ log = new ApplicationLog_(logFormat, "bvnlog", info.ClientMobileNo);
                            }
                            else
                            {
                                //No record found
                                resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
                            }
                        }
                        else
                        {
                            //service error
                            resp = "All connections are currently in use!%0APlease try again. 1";
                        }
                    }
                    else
                    {
                        //Service connection error
                        resp = "All connections are currently in use!%0APlease try again. 2";
                    }
            }
            catch(Exception ex)
            {
                ApplicationLog_ log = new ApplicationLog_(ex.Message, "bvnlog", info.ClientMobileNo);
                resp = "All connections are currently in use!%0APlease try again. 3";
            }
            return resp;
        }

        public static string getAccountsByCustomerId(AccountLookupInfo info)
        {
            string mob_234 = info.ClientMobileNo;
            string mob_080 = info.ClientMobileNo;
            CustomerAccount_Util.formatMobile_to_080(ref mob_080);
            CustomerAccount_Util.formatMobile_to_234(ref mob_234);
            string resp = "";
            try
            {
                svcBanks.ServiceSoapClient sc = new svcBanks.ServiceSoapClient();
                DataSet ds = sc.GetBankCustByMobile(mob_234, mob_080);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var dt_distinct = (from dr in ds.Tables[0].AsEnumerable()
                                               select new
                                               {
                                                   FIRST_NAME = dr.Field<object>("FIRST_NAME")
                                                   ,
                                                   EMAIL = dr.Field<object>("EMAIL")
                                                    ,
                                                   MOBILE_PHONE = dr.Field<object>("MOBILE_PHONE")
                                                    ,
                                                   BIRTH_DATE = dr.Field<object>("BIRTH_DATE")
                                                    ,
                                                   DATE_OPEN = dr.Field<object>("DATE_OPEN")
                                                    ,
                                                   CUS_NUM = dr.Field<object>("CUS_NUM")
                                                    ,
                                                   CUS_SHO_NAME = dr.Field<object>("CUS_SHO_NAME")
                                                    ,
                                                   MAP_ACC_NO = dr.Field<object>("MAP_ACC_NO")
                                               }).Distinct().ToList();


                            DataTable dtx = CustomerAccount_Util.ToDataTable(dt_distinct);
                            //string txt = "Your Acct. Nos are:";
                            string txt = "";

                            foreach (DataRow dr in dtx.Rows)
                            {

                                txt = txt + "" + dr["MAP_ACC_NO"].ToString() + ";";

                                //try
                                //{
                                //    SqlParameter[] parameters = {
                                //   new SqlParameter("@BVN",info.ClientMobileNo)
                                //  ,new SqlParameter("@AccountNo",dr["MAP_ACC_NO"])
                                //  ,new SqlParameter("@AccountType","BANK")
                                //  ,new SqlParameter("@PhoneNo",dr["MOBILE_PHONE"])
                                //  ,new SqlParameter("@DateofBirth",dr["BIRTH_DATE"])
                                //  ,new SqlParameter("@BatchID",info.ClientMobileNo)
                                //  ,new SqlParameter("@RequestFlag","New")
                                //  ,new SqlParameter("@Source","USSD")
                                //};

                                //    string sql = " INSERT INTO tbl_stagingArea (BVN,AccountNo,AccountType,PhoneNo,DateofBirth,DateUploaded,BatchID,RequestFlag,Source) VALUES (@BVN,@AccountNo,@AccountType,@PhoneNo,@DateofBirth,GETDATE(),@BatchID,@RequestFlag,@Source) select @@rowcount";
                                //    int val = (int)SqlHelper.ExecuteScalar(GetAppConnection(), CommandType.Text, sql, parameters);
                                //    procLog = string.Format("Request for BVN:{0} with Account:{1} respond with:{2}", info.ClientMobileNo, dr["MAP_ACC_NO"], val);

                                //}
                                //catch (SqlException ex)
                                //{
                                //    procLog = string.Format("Request for BVN:{0} with Account:{1} respond with:{2}", info.ClientMobileNo, dr["MAP_ACC_NO"], ex.Message);
                                //    ApplicationLog_ log = new ApplicationLog_(procLog, "bvnlog", info.ClientMobileNo);
                                //}
                            }

                            resp = string.Format("Your Accts are:{0}", txt);
                        }
                        else
                        {
                            //No record found
                            resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
                        }
                    }
                    else
                    {
                        //service error
                        resp = "All connections are currently in use!%0APlease try again. 4";
                    }
                }
                else
                {
                    //Service connection error
                    resp = "All connections are currently in use!%0APlease try again. 5";
                }
            }
            catch (Exception ex)
            {
                ApplicationLog_ log = new ApplicationLog_(ex.Message, "bvnlog", info.ClientMobileNo);
                resp = "All connections are currently in use!%0APlease try again. 6";
            }
            return resp;
        }

        private static List<AccountInfo> GetAccountsByMobile(string mobile)
        {
            string mob_234 = mobile;
            string mob_080 = mobile;
            CustomerAccount_Util.formatMobile_to_080(ref mob_080);
            CustomerAccount_Util.formatMobile_to_234(ref mob_234);
            svcBanks.ServiceSoapClient sc = new svcBanks.ServiceSoapClient();
            DataSet ds = sc.GetBankCustByMobile(mob_234, mob_080);

            if (CustomerAccount_Util.CountRows(ds) <= 0) return null;
            var ls = new List<AccountInfo>();
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                var ai = new AccountInfo();
                ai.Set(dr);
                ls.Add(ai);
            }
            return ls;
        }


        public static string getAccountBalance(AccountLookupInfo inf)
        {
            //get account info here
            var acctbal = GetAccountInfo(inf.AccountNumber);
            if (acctbal == null) return "ERROR: CAB01 - Invalid account%0AKindly visit any of our branches to update your account." +
                 "%0ACALL US - (+234) 01-4484481-5";


            var accList = GetAccountsByMobile(inf.ClientMobileNo);
            if (accList == null) return "ERROR: CAB02  - Invalid mobile%0AKindly visit any of our branches to update your account." +
                "%0ACALL US - (+234) 01-4484481-5";
            
            var acct = accList.Where(x => x.AccountNumber == inf.AccountNumber).FirstOrDefault();
            if (acct == null) return "ERROR: CAB03 - Invalid customer account%0AKindly visit any of our branches to update your account." +
                "%0ACALL US - (+234) 01-4484481-5"; 

            return SendBalanceSMS(acct, acctbal);           
        }

        private static Account GetAccountInfo(string p)
        { 
            var ibskey = new IbsKey();
            string msg = XMLTool.GetHeader(DateTime.Now.ToString("yyMMddHHmmss"), "706");
            msg += XMLTool.AddTagWithValue("NUBAN", p);
            msg += XMLTool.GetFooter();
            WSPlumber ws = new WSPlumber();
            string x = (string)ws.CallIBS(msg, ibskey.Key, ibskey.Vector, ibskey.AppId);
            Response r = new Response();
            r.ID = XMLTool.GetNodeData(x, "ReferenceID");
            r.Type = XMLTool.GetNodeData(x, "RequestType");
            r.Code = XMLTool.GetNodeData(x, "ResponseCode");
            r.Text = XMLTool.GetNodeData(x, "ResponseText");
            if (r.Code != "00") return null;
            var acctBal = (Account)JSONize.DeserializeFromString(r.Text, typeof(Account));
            return acctBal;
        }

        private static string SendBalanceSMS(AccountInfo acct, Account acctBal)
        {
            string fmt = @"Your account ({0} - {1}) balance is 
Avl: {2} {3}
Bal: {4} {5}";
            var cur = GetCurrency(acctBal.CurCode);
            var message = string.Format(fmt, acct.AccountNumber, acctBal.AccountName + " " + acctBal.LedgerName, cur, acctBal.AvailBalance.ToString("#,##0.00"), cur, acctBal.CurrentBalance.ToString("#,##0.00"));
            SaveBilling(acct.AccountNumber, "USSD to " + acct.Mobile, 1, message);
            return message;

            //msg = XMLTool.GetHeader("1", "217");
            //msg += XMLTool.AddTagWithValue("Msg", message);
            //msg += XMLTool.AddTagWithValue("gsm", acct.Mobile);
            //msg += XMLTool.GetFooter();
            //ws = new WSPlumber();
            //x = (string)ws.CallIBS(msg, ibskey.Key, ibskey.Vector, ibskey.AppId);
            //r = new Response();
            //r.ID = XMLTool.GetNodeData(x, "ReferenceID");
            //r.Type = XMLTool.GetNodeData(x, "RequestType");
            //r.Code = XMLTool.GetNodeData(x, "ResponseCode");
            //r.Text = XMLTool.GetNodeData(x, "ResponseText");
            //if (r.Code == "00")
            //{
                
            //}
            //else
            //{
            //    SaveBilling(acct.AccountNumber, r.Text, 1, "");
            //}
        }

        private static void SaveBilling(string acct, string info, int servtype, string SMS)
        {
            int nx = 0;
            if(SMS.Length > 0)
            {
                nx = Convert.ToInt32(Math.Ceiling(SMS.Length / 160.0));
            }
            string sql = @"insert into tbl_USSD_Billing (servicetypeid,billaccount,billquantity,billdate,billinfo)
values (@servicetypeid,@billaccount,@billquantity,@billdate,@billinfo)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@servicetypeid", servtype);
            cn.AddParam("@billaccount", acct);
            cn.AddParam("@billquantity", nx);
            cn.AddParam("@billdate", DateTime.Now);
            cn.AddParam("@billinfo", info);
            cn.Update();
        }

        private static string GetCurrency(string p)
        {
            string res = "";
            switch(p)
            {
                case "1": res = "NGN"; break;
                case "2": res = "USD"; break;
                case "3": res = "GBP"; break;
                case "46": res = "NGN"; break;
            }
            return res;
        }
    }
}
