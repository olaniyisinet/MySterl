using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTU.DAL;
using VTU.UTILITIES;
using VTU.ObjectInfo;
using System.Data;


namespace VTU.BLL
{
    class USSD_BLL
    {
        public static string Activate(string nuban, string mobile)
        {
            string resp = "";
            DataSet ds = USSD_db.getBankDetail(nuban);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            string mob = dr["MOBILE_PHONE"].ToString();
                            Common.formatMobile(ref mob);
                            Common.formatMobile(ref mobile);
                            string curCode = dr["CUR_CODE"].ToString();
                            if (mobile == mob)
                            {
                                if (curCode == "1")
                                {
                                    Go_Registered_Account g = new Go_Registered_Account();
                                    g.Mobile = mobile;
                                    g.NUBAN = nuban;
                                    g.StatusFlag = Constants.Status_Green.ToString();
                                    g.TransactionRefID = DateTime.Now.Ticks.ToString();
                                    g.RefID = "1987";
                                    int retval = USSD_db.Activate(g);
                                    switch (retval)
                                    {
                                        case -99:
                                            resp = "All connections are currently in use%0A Kindly try again!";
                                            break;
                                        case -1:
                                            resp = "You have already been activated for this service%0ADial *822*Amount# to recharge your phone.%0Ae.g *822*500# ";
                                            break;
                                        case 1:
                                            resp = "Congratulations!%0AYou can now dial *822*Amount# to top-up your mobile phone.%0Ae.g *822*500#";
                                            break;
                                    }
                                }
                                else // Non Naira Account 
                                {
                                    // Non Naira account is not allowed for this service
                                    resp = "The specified account cannot be use for this service.%0AEnter a valid naira account!";
                                }
                            }
                            else
                            {
                                // Phone number not match with the NUBAN supplied
                                resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
                            }
                        }
                        else
                        {
                            //Unusal record set returned
                            resp = "All connections are currently in use, Please try again.";
                        }
                    }
                    else
                    {
                        //When no record exist on banks
                        resp = "This service applies to sterling customers only.%0AKindly approach the nearest sterling bank to open account with us.Thank you.";
                    }
                }
                else
                {
                    //When no record exist on banks
                    resp = "This service applies to sterling customers only.%0AKindly approach the nearest sterling bank to open account with us.Thank you.";
                }
            }
            else
            {
                //Exception in disguise
                resp = "All connections are currently in use, Please try again.";
            }
            return resp;
        }

        /// <summary>
        /// This method call the Aitime request for both Mobile money and bank customers
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="Amount"></param>
        /// <param name="network"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static string Buy_Self(string mobile, string Amount, string network, string sessionId, string frmNuban)
        {
            string resp = ""; int trnxlimit = 0;
            //Activation validation
            Common.formatMobile(ref mobile);
            DataTable dtRegAcct = USSD_db.getRegisteredAcct(mobile);

            if (dtRegAcct != null)
            {
                if (dtRegAcct.Rows.Count > 0)
                {
                    string statusFlag = dtRegAcct.Rows[0]["StatusFlag"].ToString();
                    int enrollType = int.Parse(dtRegAcct.Rows[0]["EnrollType"].ToString());
                    trnxlimit = int.Parse(dtRegAcct.Rows[0]["TrnxLimit"].ToString());
                    if (statusFlag == "0") //Account is inactive
                    {
                        resp = "Your account has been deactivated from this service!.%0AKindly approach the nearest sterling bank to re-activate.";
                    }
                    else //Active account ....
                    {
                        string Nuban = "";
                        if (frmNuban == "")
                        {
                            Nuban = dtRegAcct.Rows[0]["NUBAN"].ToString();
                        }
                        else
                        {
                            Nuban = frmNuban;
                        }
                        if (Nuban.StartsWith("05"))
                        {
                            //proceed
                        }
                        else
                        {
                            //go to EACBS and confirm if the ledcode 
                            string Ledcode = "";
                            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                            DataSet ds = ws.getAccountFullInfo(Nuban);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow dr = ds.Tables[0].Rows[0];
                                Ledcode = dr["T24_LED_CODE"].ToString();
                            }
                            else
                            {
                                resp = "Unable to locate record for this service.";
                                return resp;
                            }
                            if (Ledcode == "1400" || Ledcode == "1704")
                            {
                                resp = "Transaction is not allowed on this account";
                                return resp;
                            }
                        }

                        DataTable dtRegProf = USSD_db.getRegisteredProfile(mobile, Nuban);
                        //Customer enrolled with BVN
                        trnxlimit = int.Parse(dtRegProf.Rows[0]["TrnxLimit"].ToString());

                        //get Today txn sum for mobile
                        DataTable dtDailySum = USSD_db.getDailyTxnSum(mobile, Constants.RequestType_Self);
                        var Go_account = new { Minimum = "", Maximum = "", Max_per_day = "" };
                        if (dtDailySum != null)
                        {
                            //check daily max ...
                            DataTable dtConfig = USSD_db.getConfigAccount();
                            if (enrollType == 1 && trnxlimit != 1)
                            {
                                Go_account = (from p in dtConfig.AsEnumerable()
                                              where p.Field<int>("code") == 700
                                              select new
                                              {
                                                  Minimum = p.Field<string>("var1"),//Minimum
                                                  Maximum = p.Field<string>("var2"),//Maximum
                                                  Max_per_day = p.Field<string>("var3")//Max_per_day
                                              }).FirstOrDefault();
                            }
                            else
                            {
                                Go_account = (from p in dtConfig.AsEnumerable()
                                              where p.Field<int>("code") == 600
                                              select new
                                              {
                                                  Minimum = p.Field<string>("var1"),//Minimum
                                                  Maximum = p.Field<string>("var2"),//Maximum
                                                  Max_per_day = p.Field<string>("var3")//Max_per_day
                                              }).FirstOrDefault();
                            }


                            decimal amt = Common.GetDecimalValue(Amount);
                            decimal todayTxn = Common.GetDecimalValue(dtDailySum.Rows[0][0]);
                            decimal Minimum = Common.GetDecimalValue(Go_account.Minimum);
                            decimal Maximum = Common.GetDecimalValue(Go_account.Maximum);
                            decimal Max_per_day = Common.GetDecimalValue(Go_account.Max_per_day);

                            if (trnxlimit != 0)
                            {
                                DataTable dtRegLimit = USSD_db.GetLimitMINMAXamt();
                                Maximum = decimal.Parse(dtRegLimit.Rows[0]["minamt"].ToString());
                                Max_per_day = decimal.Parse(dtRegLimit.Rows[0]["maxamt"].ToString());

                                DataTable dtTotalTrnxToday = USSD_db.getTotalTransDonePerday(amt, mobile, sessionId, Max_per_day);
                                todayTxn += decimal.Parse(dtTotalTrnxToday.Rows[0]["totalTOday"].ToString());

                            }
                            if (amt >= Minimum)
                            {
                                if (Maximum >= amt)
                                {
                                    if (Max_per_day >= (todayTxn + amt))
                                    {
                                        Go_Request req = new Go_Request();
                                        req.Mobile = mobile;
                                        req.NUBAN = Nuban;
                                        req.RequestRef = Common.GetRef(mobile);
                                        req.RequestStatus = Constants.Status_Yellow.ToString();
                                        req.RequestString = "";
                                        req.RequestType = Constants.RequestType_Self.ToString();
                                        req.Amount = Amount;
                                        req.Beneficiary = mobile;
                                        req.NetworkID = network;
                                        req.sessionId = sessionId;
                                        req.ChannelID = "1";
                                        int retVal = USSD_db.LogRequest(req);
                                        switch (retVal)
                                        {
                                            case -99:
                                                resp = "All connections are currently in use. Please try again!";
                                                break;
                                            case -1:
                                                resp = "All connections are currently in use. Please try again!";
                                                break;
                                            case 1:
                                                //resp = "Your airtime request of NGN" + req.Amount + " is being processed. You can now recharge your phone for up to N2000/request & N5000/day.%0AThank you for banking with us.";
                                                resp = "Your airtime request of NGN" + req.Amount + " is being processed.%0AThank you for banking with us.";
                                                break;
                                            default:
                                                resp = "All connections are currently in use. Please try again!";
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        //Maximum daily transaction exceeded
                                        if (trnxlimit == 1)
                                        {
                                            resp = "Your airtime request of N" + Amount + " was not successful because you have exceeded your maximum allowed amount on USSD today.";
                                        }
                                        else
                                        {
                                            resp = "Your airtime request of N" + Amount + " was not successful as you have exceeded the per transaction limit for airtime purchase.";
                                        }

                                        //resp = "Maximum daily transaction limit is NGN" + Max_per_day + "!";
                                    }
                                }
                                else
                                {
                                    if (enrollType == 1)
                                    {
                                        //Maximum allowd is Maximum
                                        resp = "Sorry, your maximum VTU allowed is N" + Maximum.ToString() + ".%0ADial *822*1*Acct# to upgrade your limit.";
                                    }
                                    else
                                    {
                                        //Maximum allowd is Maximum
                                        resp = "Maximum VTU allowed is NGN" + Maximum.ToString();
                                    }
                                }
                            }
                            else
                            {
                                //Minimum allowed is Minimum ...
                                resp = "Minimum VTU allowed is NGN" + Minimum.ToString();
                            }
                        }
                        else
                        {
                            //database error ...
                            resp = "Connection error...!";
                        }


                    }

                }
                else  //Do Mfino Channel (2)..
                {
                    int isBlacklist = USSD_db.ValidateBlacklist(mobile, mobile);
                    if (isBlacklist == 0)
                    {
                        string stat = "";
                        if (ValidateMfion(mobile, ref stat))
                        {
                            //log request .. ..

                            DataTable dtDailySum = USSD_db.getDailyTxnSum(mobile);
                            if (dtDailySum != null)
                            {
                                //check daily max ...
                                DataTable dtConfig = USSD_db.getConfigAccount();
                                var Go_account = (from p in dtConfig.AsEnumerable()
                                                  where p.Field<int>("code") == 600
                                                  select new
                                                  {
                                                      Minimum = p.Field<string>("var1"),//Minimum
                                                      Maximum = p.Field<string>("var2"),//Maximum
                                                      Max_per_day = p.Field<string>("var3")//Max_per_day
                                                  }).FirstOrDefault();

                                decimal amt = Common.GetDecimalValue(Amount);
                                decimal todayTxn = Common.GetDecimalValue(dtDailySum.Rows[0][0]);
                                decimal Minimum = Common.GetDecimalValue(Go_account.Minimum);
                                decimal Maximum = Common.GetDecimalValue(Go_account.Maximum);
                                decimal Max_per_day = Common.GetDecimalValue(Go_account.Max_per_day);

                                if (amt >= Minimum)
                                {
                                    if (Maximum >= amt)
                                    {
                                        if (Max_per_day >= (todayTxn + amt))
                                        {
                                            Go_Request req = new Go_Request();
                                            req.Mobile = mobile;
                                            req.NUBAN = mobile;
                                            req.RequestRef = Common.GetRef(mobile);
                                            req.RequestStatus = Constants.Status_Yellow.ToString();
                                            req.RequestString = "";
                                            req.RequestType = Constants.RequestType_Self.ToString();
                                            req.Amount = Amount;
                                            req.Beneficiary = mobile;
                                            req.NetworkID = network;
                                            req.sessionId = sessionId;
                                            req.ChannelID = "2";
                                            int retVal = USSD_db.LogRequest(req);
                                            switch (retVal)
                                            {
                                                case -99:
                                                    resp = "All connections are currently in use. Please try again!";
                                                    break;
                                                case -1:
                                                    resp = "All connections are currently in use. Please try again!";
                                                    break;
                                                case 1:
                                                    resp = "Your airtime request of NGN" + req.Amount + " is being processed.%0AThank you for banking with us.";

                                                    //resp = "Your airtime request of NGN" + req.Amount + " is being processed. You can now recharge your phone for up to N2000/request & N5000/day.%0AThank you for banking with us.";
                                                    //resp = "Sterling Bank is currently processing your NGN" + req.Amount + " VTU request.%0AThank you for banking with us.";
                                                    // resp = "You can now topup for friends and family by dialing *822*amount*friend mobile number# e.g *822*500*08012345678#.%0AThank you for banking with us.";
                                                    break;
                                                default:
                                                    resp = "All connections are currently in use. Please try again!";
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            //Maximum daily transaction exceeded
                                            resp = "Your airtime request of N" + Amount + " was not successful as you have exceeded the per trnx limit of N2000 Or daily limit of N5000. Kindly try again.";
                                            //resp = "Maximum daily transaction limit is NGN" + Max_per_day + "!";
                                        }
                                    }
                                    else
                                    {
                                        //Maximum allowd is Maximum
                                        resp = "Maximum VTU allowed is NGN" + Maximum.ToString();
                                    }
                                }
                                else
                                {
                                    //Minimum allowed is Minimum ...
                                    resp = "Minimum VTU allowed is NGN" + Minimum.ToString();
                                }
                            }
                        }
                        else //Not Registered ..
                        {
                            resp = "Kindly dial *822*1*10-digit account number to activate this service.%0Aeg *822*1*0123456789#";
                        }
                    }
                    else if (isBlacklist == 1) //
                    {
                        resp = "Your account has been deactivated from this service!.%0AKindly approach the nearest sterling bank to reactivate.";
                    }
                    else if (isBlacklist == -1) //database error
                    {
                        resp = "All connections are currently in use. Please try again!";
                    }
                    else if (isBlacklist == -99) //exception error
                    {
                        resp = "All connections are currently in use. Please try again!";
                    }
                }
            }
            else
            {
                //error occurred
                resp = "All connections are currently in use.Please try again!";
            }
            return resp;
        }


        public static string Buy_Friend(string mobile_friend, string mobile, string Amount, string network, string sessionId, string authkey, string frmNuban)
        {
            string resp = "";
            DataTable dtRegAcct = USSD_db.getRegisteredAcct(mobile);
            string Ledcode = "";
            string Nuban = "";
            if (frmNuban == "")
            {
                Nuban = dtRegAcct.Rows[0]["NUBAN"].ToString();
            }
            else
            {
                Nuban = frmNuban;
            }
            if (Nuban.StartsWith("05"))
            {
                //proceed
            }
            else
            {
                EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                DataSet ds = ws.getAccountFullInfo(Nuban);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    Ledcode = dr["T24_LED_CODE"].ToString();
                }
                else
                {
                    resp = "Unable to locate record for this service.";
                    return resp;
                }
                if (Ledcode == "1400" || Ledcode == "1704")
                {
                    resp = "Transaction is not allowed on this account";
                    return resp;
                }
            }
            //Activation validation
            Common.formatMobile(ref mobile);
            int isBlacklist = USSD_db.ValidateBlacklist(mobile, mobile);
            if (isBlacklist == 0)
            {
                //log request .. ..
                DataTable dtDailySum = USSD_db.getDailyTxnSum(mobile, Constants.RequestType_Other);
                if (dtDailySum != null)
                {
                    //check daily max ...
                    DataTable dtConfig = USSD_db.getConfigAccount();
                    var Go_account = (from p in dtConfig.AsEnumerable()
                                      where p.Field<int>("code") == 600
                                      select new
                                      {
                                          Minimum = p.Field<string>("var1"),//Minimum
                                          Maximum = p.Field<string>("var2"),//Maximum
                                          Max_per_day = p.Field<string>("var3")//Max_per_day
                                      }).FirstOrDefault();

                    decimal amt = Common.GetDecimalValue(Amount);
                    decimal todayTxn = Common.GetDecimalValue(dtDailySum.Rows[0][0]);
                    decimal Minimum = Common.GetDecimalValue(Go_account.Minimum);
                    decimal Maximum = Common.GetDecimalValue(Go_account.Maximum);
                    decimal Max_per_day = Common.GetDecimalValue(Go_account.Max_per_day);

                    DataTable dtRegProf = USSD_db.getRegisteredProfile(mobile, Nuban);
                    //Customer enrolled with BVN
                    if (dtRegProf.Rows[0]["TrnxLimit"].ToString() == "1")
                    {
                        DataTable dtRegLimit = USSD_db.GetLimitMINMAXamt();
                        Maximum = decimal.Parse(dtRegLimit.Rows[0]["minamt"].ToString());
                        Max_per_day = decimal.Parse(dtRegLimit.Rows[0]["maxamt"].ToString());

                        DataTable dtTotalTrnxToday = USSD_db.getTotalTransDonePerday(amt, mobile, sessionId, Max_per_day);
                        todayTxn += decimal.Parse(dtTotalTrnxToday.Rows[0]["totalTOday"].ToString());

                    }

                    if (amt >= Minimum)
                    {
                        if (Maximum >= amt)
                        {
                            if (Max_per_day >= (todayTxn + amt))
                            {
                                Go_Request req = new Go_Request();
                                req.Mobile = mobile;
                                //req.NUBAN = mobile;
                                req.NUBAN = Nuban;
                                req.RequestRef = Common.GetRef(mobile);
                                req.RequestStatus = Constants.Status_Yellow.ToString();
                                req.RequestString = "";
                                req.RequestType = Constants.RequestType_Other.ToString();
                                req.Amount = Amount;
                                req.Beneficiary = mobile_friend;
                                req.NetworkID = network;
                                req.sessionId = sessionId;
                                req.ChannelID = "2";
                                object obj = USSD_db.LogRequest_getRefID(req);
                                long retVal = Convert.ToInt64(obj);
                                switch (retVal)
                                {
                                    case -99:
                                        resp = "All connections are currently in use. Please try again!";
                                        break;

                                    case -1:
                                        resp = "All connections are currently in use. Please try again!";
                                        break;

                                    default:
                                        if (retVal >= 10000) //unique caller ID
                                        {
                                            //update the table for the Authkey

                                            //req.CallerRefID = retVal.ToString();
                                            //resp = pushToMfino(req, authkey);

                                            //authenticate the PIN


                                            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
                                            cn.SetProcedure("spd_UpdateAuthKey");
                                            cn.AddParam("@nuban", Nuban);
                                            cn.AddParam("@CallerRefID", retVal);
                                            cn.AddParam("@CardAuth", authkey);
                                            cn.AddParam("@Finalint", 0);
                                            int cnt = cn.ExecuteProc();

                                            if (cn.returnValue > 0)
                                            {
                                                resp = "Your airtime request of NGN" + req.Amount + " is being processed.%0AThank you for banking with us.";// You can now recharge your phone for up to N2000/request & 5000/day.%0AThank you for banking with us.";
                                            }
                                            else if (cn.returnValue == -1)
                                            {
                                                resp = "The USSD PIN you provided is incorrect. Please check & try again.";
                                            }
                                        }
                                        else
                                        {
                                            resp = "All connections are currently in use. Please try again!";
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                //Maximum daily transaction exceeded
                                resp = "Your airtime request of N" + Amount + " was not successful as you have exceeded the per trnx limit of N2000 & daily limit of N5000. Kindly try again.";
                            }
                        }
                        else
                        {
                            //Maximum allowd is Maximum
                            resp = "Maximum VTU allowed is NGN" + Maximum.ToString();
                        }
                    }
                    else
                    {
                        //Minimum allowed is Minimum ...
                        resp = "Minimum VTU allowed is NGN" + Minimum.ToString();
                    }
                }
            }
            else if (isBlacklist == 1) //
            {
                resp = "Your account has been deactivated from this service!.%0AKindly approach the nearest sterling bank to reactivate.";
            }
            else if (isBlacklist == -1) //database error
            {
                resp = "All connections are currently in use. Please try again!";
            }
            else if (isBlacklist == -99) //exception error
            {
                resp = "All connections are currently in use. Please try again!";
            }
            return resp;
        }

        /// <summary>
        /// This method call the Aitime request for both Mobile money when buying for friends
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="Amount"></param>
        /// <param name="network"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        //public static string Buy_Friend(string mobile_friend, string mobile, string Amount, string network, string sessionId,string authkey)
        //{
        //    string resp = "";
        //    //Activation validation
        //    Common.formatMobile(ref mobile);
        //    int isBlacklist = USSD_db.ValidateBlacklist(mobile, mobile);
        //    if (isBlacklist == 0)
        //    {
        //        //log request .. ..
        //        DataTable dtDailySum = USSD_db.getDailyTxnSum(mobile, Constants.RequestType_Other); 
        //            if (dtDailySum != null)
        //            {
        //                //check daily max ...
        //                DataTable dtConfig = USSD_db.getConfigAccount();
        //                var Go_account = (from p in dtConfig.AsEnumerable()
        //                                  where p.Field<int>("code") == 600
        //                                  select new
        //                                  {
        //                                      Minimum = p.Field<string>("var1"),//Minimum
        //                                      Maximum = p.Field<string>("var2"),//Maximum
        //                                      Max_per_day = p.Field<string>("var3")//Max_per_day
        //                                  }).FirstOrDefault();

        //                decimal amt = Common.GetDecimalValue(Amount);
        //                decimal todayTxn = Common.GetDecimalValue(dtDailySum.Rows[0][0]);
        //                decimal Minimum = Common.GetDecimalValue(Go_account.Minimum);
        //                decimal Maximum = Common.GetDecimalValue(Go_account.Maximum);
        //                decimal Max_per_day = Common.GetDecimalValue(Go_account.Max_per_day);

        //                if (amt >= Minimum)
        //                {
        //                    if (Maximum >= amt)
        //                    {
        //                        if (Max_per_day >= (todayTxn + amt))
        //                        {
        //                            Go_Request req = new Go_Request();
        //                            req.Mobile = mobile;
        //                            req.NUBAN = mobile;
        //                            req.RequestRef = Common.GetRef(mobile);
        //                            req.RequestStatus = Constants.Status_Blue.ToString();
        //                            req.RequestString = "";
        //                            req.RequestType = Constants.RequestType_Other.ToString();
        //                            req.Amount = Amount;
        //                            req.Beneficiary = mobile_friend;
        //                            req.NetworkID = network;
        //                            req.sessionId = sessionId;
        //                            req.ChannelID = "2";
        //                            object obj = USSD_db.LogRequest_getRefID(req);
        //                            long retVal = Convert.ToInt64(obj);
        //                            switch (retVal)
        //                            {
        //                                case -99:
        //                                    resp = "All connections are currently in use. Please try again!";
        //                                    break;

        //                                case -1:
        //                                    resp = "All connections are currently in use. Please try again!";
        //                                    break;

        //                                default:
        //                                    if (retVal >= 10000) //unique caller ID
        //                                    {
        //                                        req.CallerRefID = retVal.ToString();
        //                                        resp = pushToMfino(req, authkey);
        //                                    }
        //                                    else
        //                                    {
        //                                        resp = "All connections are currently in use. Please try again!";
        //                                    }
        //                                    break;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //Maximum daily transaction exceeded
        //                            resp = "Maximum daily transaction limit is NGN" + Max_per_day + "!";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Maximum allowd is Maximum
        //                        resp = "Maximum VTU allowed is NGN" + Maximum.ToString();
        //                    }
        //                }
        //                else
        //                {
        //                    //Minimum allowed is Minimum ...
        //                    resp = "Minimum VTU allowed is NGN" + Minimum.ToString();
        //                }
        //            }
        //    }
        //    else if (isBlacklist == 1) //
        //    {
        //        resp = "Your account has been deactivated from this service!.%0AKindly approach the nearest sterling bank to reactivate.";
        //    }
        //    else if (isBlacklist == -1) //database error
        //    {
        //        resp = "All connections are currently in use. Please try again!";
        //    }
        //    else if (isBlacklist == -99) //exception error
        //    {
        //        resp = "All connections are currently in use. Please try again!";
        //    }
        //    return resp;
        //}

        private static string pushToMfino(Go_Request req, string pin)
        {
            Svc_Response svc_resp = new Svc_Response();
            try
            {
                string mobile = req.Mobile;
                string amount = req.Amount;
                string compID = "01";
                int nwt = Convert.ToInt32(req.NetworkID);
                switch (nwt)
                {
                    //MTN
                    case 1:
                        compID = "01";
                        break;

                    //GLO
                    case 2:
                        compID = "02";
                        break;

                    //AIRTEL
                    case 3:
                        compID = "04";
                        break;

                    //ETISALAT
                    case 4:
                        compID = "03";
                        break;

                    //MTN - by default
                    default:
                        compID = "01";
                        break;
                }

                string pstData = string.Format("channelID={1}&service=Buy&sourceMDN={0}&txnName=AirtimePurchaseInquiry&sourcePocketCode=2&destMDN={5}&amount={2}&companyID={3}&sourcePIN={4}", mobile, Constants.mfionChannelID, amount, compID, pin, req.Beneficiary.Trim());

                string maskedData = string.Format("channelID={1}&service=Buy&sourceMDN={0}&txnName=AirtimePurchaseInquiry&sourcePocketCode=2&destMDN={5}&amount={2}&companyID={3}&sourcePIN={4}", mobile, Constants.mfionChannelID, amount, compID, "****", req.Beneficiary.Trim());

                ApplicationLog reqlg = new ApplicationLog(maskedData, "mfino_user", mobile);
                string responseString = USSD_db.SendVTURequestwithPin(pstData, mobile);
                ApplicationLog reslg = new ApplicationLog(responseString, "mfino_user", mobile);

                string retnode = XMLTool.GetNodeAttribute(responseString, "message", "code");

                if (retnode == "715")//success ....
                {
                    svc_resp.RefId = XMLTool.GetNodeData(responseString, "sctlID"); //
                }

                svc_resp.ResponseCode = retnode;
                svc_resp.ResponseText = XMLTool.GetNodeData(responseString, "message");
                svc_resp.BillerRef = retnode;
                svc_resp.HashValue = "";
                svc_resp.BillerResp = retnode;
                req.RequestStatus = svc_resp.ResponseCode == "715" ? "1" : "2";
                Console_BLL.UpdateRequest(req, svc_resp);

                switch (retnode)
                {
                    case "11": //control response for unregistered customers
                        svc_resp.ResponseText = "You are currently not registered as a STERLING MONEY user.%0ADial *822# to get started.";
                        break;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog lg = new ApplicationLog(ex, "errorlog");
            }
            return svc_resp.ResponseText;
        }

        /// <summary>
        ///  This method validate if user is a mobile money user
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool ValidateMfion(string mobile, ref string status)
        {
            string pstData = string.Format("sourceMDN={0}&channelID={1}&txnName=SubscriberStatus&service=Account", mobile, Constants.mfionChannelID);
            string respXml = USSD_db.SendMessage(pstData, mobile);

            string recode = XMLTool.GetNodeData(respXml, "status");
            status = recode;
            if (recode == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DataTable SaveSteps(string SessionId, string Misdn, int reqType, string val, string prev, string next)
        {
            return USSD_db.SaveSteps(SessionId, Misdn, reqType, val, prev, next);
        }

        public static DataTable GetSteps(string SessionId, string Misdn)
        {
            return USSD_db.GetSteps(SessionId, Misdn);
        }

        public static int UpdateInput(string SessionId, string Misdn, string val, string prev, string next)
        {
            return USSD_db.UpdateInput(SessionId, Misdn, val, prev, next);
        }
    }

    class Console_BLL
    {
        public static string DoVTU(Svc_Request svc)
        {
            return Console_db.DoVTU_Request(svc);
        }

        public static DataTable getNetwork(string prefix)
        {
            return Console_db.getNetwork(prefix);
        }

        public static int UpdateRequest(Go_Request req, Svc_Response svc_resp)
        {
            return Console_db.UpdateRequest(req, svc_resp);
        }

        public static void dataArchive(DataTable dt)
        {
            Console_db.dataArchive(dt);
        }

        public static DataTable getTimedOutRequest(int MinLapses)
        {
            return Console_db.getTimedOutRequest(MinLapses);
        }

        public static DataTable getCompletedRequest()
        {
            return Console_db.getCompletedRequest();
        }

        public static int UpdateStamp(RequestLog log)
        {
            return Console_db.UpdateStamp(log);
        }

        public static int LogStamp(RequestLog log)
        {
            return Console_db.LogStamp(log);
        }

        public static string SendMessage(string data, string mobile)
        {
            return Console_db.SendMessage(data, mobile);
        }

        public static int SetFlag(Go_Request a)
        {
            return Console_db.SetFlag(a);
        }

        public static void resetTimedOut(int minLapse)
        {
            Console_db.resetTimedOut(minLapse);

        }

        public static DataSet getBankDetail(string nuban)
        {
            return USSD_db.getBankDetail(nuban);
        }

        public static DataTable getRegisteredAcct()
        {
            return Console_db.getRegisteredAcct();
        }

        public static void UpdateRegisteredAcct(string oMobile, string nMobile, string nuban)
        {
            Console_db.UpdateRegisteredAcct(oMobile, nMobile, nuban);
        }
    }

    class GUI_BLL
    {
        public static void SaveLog(AuditLog a)
        {
            GUI_db.SaveLog(a);
        }

        public static int UpdateAdminProfile(int ID, int RoleID, int Status)
        {
            return GUI_db.UpdateAdminProfile(ID, RoleID, Status);
        }

        public static int InsertAdminProfile(string username, string Fullname, string StaffID, string RoleID, string CreatedBy, string Status, string Email, string Deptname, string Unit)
        {
            return GUI_db.InsertAdminProfile(username, Fullname, StaffID, RoleID, CreatedBy, Status, Email, Deptname, Unit);
        }

        public static retObject getAdminInfoByUsername(string username)
        {
            return GUI_db.getAdminInfoByUsername(username);
        }

        public static DataTable getAdminInfo()
        {
            return GUI_db.getAdminInfo();
        }

        public static DataTable getAdminInfoByID(int ID)
        {
            return GUI_db.getAdminInfoByID(ID);
        }

        public static int UpdateConfig(decimal minPtxn, decimal maxPtxn, decimal maxPday, string code)
        {
            return GUI_db.UpdateConfig(minPtxn, maxPtxn, maxPday, code);
        }

        public static DataTable FetchConfig(string code)
        {
            return GUI_db.FetchConfig(code);
        }

        public static DataTable getReport(string nm, string st, string en, string fl, string chn)
        {
            return GUI_db.getReport(nm, st, en, fl, chn);
        }

        public static DataTable getReport_Daily(string nm, string fl, string chn)
        {
            return GUI_db.getReport_Daily(nm, fl, chn);
        }

        public static DataSet DashBoadr()
        {
            return GUI_db.DashBoadr();
        }
        public static DataSet DashBoard(string st, string en)
        {
            return GUI_db.DashBoard(st, en);
        }

        public static DataTable getBlackList(string mobile)
        {
            return GUI_db.getBlackList(mobile);
        }

        public static DataTable getBlackListHistory(string mobile)
        {
            return GUI_db.getBlackListHistory(mobile);
        }

        public static DataSet getBlackListHistoryAll(string mobile)
        {
            return GUI_db.getBlackListHistoryAll(mobile);
        }

        public static DataTable getRegiteredAccount(string mobile, string StatusFlag)
        {
            return GUI_db.getRegiteredAccount(mobile, StatusFlag);
        }

        public static int enListBlackList(BlackList b, BlackListHistory h, int channel)
        {
            return GUI_db.enListBlackList(b, h, channel);
        }

        public static int deListBlackList(BlackList b, BlackListHistory h, int channel)
        {
            return GUI_db.deListBlackList(b, h, channel);
        }
    }

    class Generic_BLL
    {
        public static DataTable getRequestByStatus(int RequestStatus)
        {
            return Generic_db.getRequestByStatus(RequestStatus);
        }

        public static DataTable getRequestByNetwork(int RequestStatus, int Network)
        {
            return Generic_db.getRequestByNetwork(RequestStatus, Network);
        }

        public static DataTable getRequestByNotNetwork(int RequestStatus, int Network)
        {
            return Generic_db.getRequestByNotNetwork(RequestStatus, Network);
        }

        public static DataTable getRequestByChannelID(int RequestStatus, int channelID)
        {
            return Generic_db.getRequestByChannelID(RequestStatus, channelID);

        }

        public static DataTable getRequestByNetworkID(int ChannelID, int NetworkID)
        {
            return Generic_db.getRequestByNetworkID(ChannelID, NetworkID);
        }
    }
}