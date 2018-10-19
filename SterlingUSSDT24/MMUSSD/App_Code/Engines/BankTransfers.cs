using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for BankTransfers
/// </summary>
public class BankTransfers : BaseEngine
{
    EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
    public BankTransfers()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string SaveRequest(UReq req)
    {
        //*822*9*100*0005969437#
        string resp = "0"; int enrollType = 0;
        string frmNuban = ""; Gadget g = new Gadget();
        try
        {
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = ""; int cnt = 0;
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    activated = int.Parse(dr["Activated"].ToString());
                    enrollType = int.Parse(dr["EnrollType"].ToString());
                    nuban = dr["nuban"].ToString().Trim();
                    if (activated != 1)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }

            if (activated == 1)
            {
                char[] delm = { '*', '#' };
                string msg = req.Msg.Trim();
                string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                //string from menu breaks here and continues the steps
                if (nuban == s[3].Trim())
                {
                    req.next_op = 122;
                    return "9";
                }
                else
                {
                    addParam("NUBAN", nuban, req);

                    //Check if customer balance is ok
                    int noOfAccts = g.GetNoofAccts(req.Msisdn.Trim());
                    if (noOfAccts > 1)
                    {
                        //check will happen at one the SaveAcctSelected methods
                    }
                    else
                    {
                        frmNuban = getDefaultAccount(req.Msisdn.Trim());
                        decimal cusbal = 0; decimal amtval = 0;
                        amtval = decimal.Parse(s[2]);

                        //if amount is over 20k check if customer has set pin 2
                        if (amtval > 20000)
                        {
                            var isPin2Set = checkForPin2(frmNuban, req.Msisdn, req);
                            if (!isPin2Set)
                            {
                                req.next_op = 777;
                                return "0";
                            }
                        }

                        if (frmNuban.StartsWith("05"))
                        {
                            IMALEngine iMALEngine = new IMALEngine();
                            ImalDetails imalinfo = new ImalDetails();
                            imalinfo = iMALEngine.GetImalDetailsByNuban(frmNuban);
                            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
                            if (imalinfo.Status == 0)
                            {
                                req.next_op = 4190;
                                return "0";
                            }
                            if (cusbal > amtval)
                            {
                                //proceed
                                var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                                DataRow dr1 = ds1.Tables[0].Rows[0];
                                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                                activated = int.Parse(dr1["Activated"].ToString());

                                if (trnxLimit == 1 && activated == 1)
                                {
                                    Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                    getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);

                                    if (limitedTuple.Item2 < amtval + Totaldone)
                                    {
                                        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                        string response = cardws.GetActiveCards(frmNuban);
                                        if (response.StartsWith("00"))
                                        {
                                            cnt = 111;
                                            addParam("NoCard", cnt.ToString(), req);
                                            req.next_op = 4290;
                                            return "0";
                                        }
                                        else
                                        {
                                            cnt = 999;
                                            addParam("cnt", cnt.ToString(), req);
                                            // addParam("NUBAN", nuban, req);
                                            req.next_op = 124;
                                            return "0";
                                        }
                                    }
                                }

                                if (trnxLimit == 0)
                                {
                                    Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                    if (limitedTuple.Item1 < amtval)
                                    {
                                        //Reroute to amount field
                                        cnt = 104;
                                        int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                        addParam("cnt", cnt.ToString(), req);
                                        if (s[1] == "4" || s[1] == "9")
                                        {
                                            req.next_op = 91100;
                                            return "9";
                                        }
                                        if (s[1] == "5" || s[1] == "14")
                                        {
                                            req.next_op = 92006;
                                            return "9";
                                        }

                                    }
                                    if (limitedTuple.Item2 < amtval + Totaldone)
                                    {
                                        //reroute to amount field
                                        cnt = 103;
                                        addParam("cnt", cnt.ToString(), req);
                                        if (s[1] == "4" || s[1] == "9")
                                        {
                                            req.next_op = 91100;
                                            return "9";
                                        }
                                        if (s[1] == "5" || s[1] == "14")
                                        {
                                            req.next_op = 92006;
                                            return "9";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                cnt = 102;
                                addParam("cnt", cnt.ToString(), req);
                                if (s[1] == "4" || s[1] == "9")
                                {
                                    req.next_op = 91100;
                                    return "9";
                                }
                                if (s[1] == "5" || s[1] == "14")
                                {
                                    req.next_op = 92006;
                                    return "9";
                                }
                            }

                        }
                        else if (frmNuban.StartsWith("11"))
                        {
                            //Treat as BankOne
                            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
                            //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
                            BankOneClass bank1 = new BankOneClass();
                            bank1.accountNumber = frmNuban;
                            string bankOneReq = bank1.createReqForBalanceEnq();
                            var Bank1ref = LogBankOneRequest(req.SessionID, frmNuban, bankOneReq);
                            string encrptdReq = EncryptTripleDES(bankOneReq);
                            string getBankOneResp = bankOne_WS.BankOneBalanceEnquiry(encrptdReq);
                            bool isBankOne = bank1.readRespForBalEnq(getBankOneResp);
                            LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
                            cusbal = bank1.availableBal;
                            if (cusbal > amtval)
                            {
                                //proceed
                                var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                                DataRow dr1 = ds1.Tables[0].Rows[0];
                                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                                if (trnxLimit == 0)
                                {
                                    Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                    if (limitedTuple.Item1 < amtval)
                                    {
                                        //Reroute to amount field
                                        cnt = 104;
                                        //removeParam("AMOUNT", req);
                                        int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                        addParam("cnt", cnt.ToString(), req);
                                        if (s[1] == "4" || s[1] == "9")
                                        {
                                            req.next_op = 91100;
                                            return "9";
                                        }
                                        if (s[1] == "5" || s[1] == "14")
                                        {
                                            req.next_op = 92006;
                                            return "9";
                                        }

                                    }
                                    if (limitedTuple.Item2 < amtval + Totaldone)
                                    {
                                        //reroute to amount field
                                        cnt = 103;
                                        //removeParam("AMOUNT", req);
                                        addParam("cnt", cnt.ToString(), req);
                                        if (s[1] == "4" || s[1] == "9")
                                        {
                                            req.next_op = 91100;
                                            return "9";
                                        }
                                        if (s[1] == "5" || s[1] == "14")
                                        {
                                            req.next_op = 92006;
                                            return "9";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                cnt = 102;
                                //removeParam("AMOUNT", req);
                                addParam("cnt", cnt.ToString(), req);
                                if (s[1] == "4" || s[1] == "9")
                                {
                                    req.next_op = 91100;
                                    return "9";
                                }
                                if (s[1] == "5" || s[1] == "14")
                                {
                                    req.next_op = 92006;
                                    return "9";
                                }
                            }
                        }
                        else
                        {
                            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                            DataSet dss = ws.getAccountFullInfo(frmNuban);
                            if (dss.Tables[0].Rows.Count > 0)
                            {
                                DataRow dr = dss.Tables[0].Rows[0];
                                string restFlag = dr["REST_FLAG"].ToString();
                                if (restFlag == "TRUE")
                                {
                                    //Check for restriction code
                                    var restCode = dss.Tables[1].Rows[0]["RestrCode"].ToString();
                                    var restrictedCodes = ConfigurationManager.AppSettings["RestrictedCodes"].Split(',');

                                    var isRestricted = restrictedCodes.Contains(restCode, StringComparer.OrdinalIgnoreCase);
                                    if (isRestricted)
                                    {
                                        cnt = 99;
                                        addParam("RestFlag", cnt.ToString(), req);
                                        req.next_op = 419;
                                        return "0";
                                    }
                                }
                                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                                if (cusbal > amtval)
                                {
                                    //proceed
                                    var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                                    DataRow dr1 = ds1.Tables[0].Rows[0];
                                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                                    activated = int.Parse(dr1["Activated"].ToString());

                                    if (trnxLimit == 1 && activated == 1)
                                    {
                                        Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                                        getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                        if (limitedTuple.Item2 < amtval + Totaldone)
                                        {
                                            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                            string response = cardws.GetActiveCards(frmNuban);
                                            if (response.StartsWith("00"))
                                            {
                                                cnt = 111;
                                                addParam("NoCard", cnt.ToString(), req);
                                                req.next_op = 4290;
                                                return "0";
                                            }
                                            else
                                            {
                                                cnt = 999;
                                                addParam("cnt", cnt.ToString(), req);
                                                addParam("NUBAN", frmNuban, req);
                                                req.next_op = 124;
                                                return "0";
                                            }
                                        }
                                    }

                                    if (trnxLimit == 0)
                                    {
                                        Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                                        getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                        if (limitedTuple.Item1 < amtval)
                                        {
                                            //Reroute to amount field
                                            cnt = 104;
                                            //removeParam("AMOUNT", req);
                                            int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                            addParam("cnt", cnt.ToString(), req);
                                            if (s[1] == "4" || s[1] == "9")
                                            {
                                                req.next_op = 91100;
                                                return "9";
                                            }
                                            if (s[1] == "5" || s[1] == "14")
                                            {
                                                req.next_op = 92006;
                                                return "9";
                                            }

                                        }
                                        if (limitedTuple.Item2 < amtval + Totaldone)
                                        {
                                            //reroute to amount field
                                            cnt = 103;
                                            //removeParam("AMOUNT", req);
                                            addParam("cnt", cnt.ToString(), req);
                                            if (s[1] == "4" || s[1] == "9")
                                            {
                                                req.next_op = 91100;
                                                return "9";
                                            }
                                            if (s[1] == "5" || s[1] == "14")
                                            {
                                                req.next_op = 92006;
                                                return "9";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    cnt = 102;
                                    //removeParam("AMOUNT", req);
                                    addParam("cnt", cnt.ToString(), req);
                                    if (s[1] == "4" || s[1] == "9")
                                    {
                                        req.next_op = 91100;
                                        return "9";
                                    }
                                    if (s[1] == "5" || s[1] == "14")
                                    {
                                        req.next_op = 92006;
                                        return "9";
                                    }
                                }
                            }
                            else
                            {
                                req.next_op = 4190;
                                return "0";
                            }
                        }
                    }

                    addParam("AMOUNT", s[2], req);
                    addParam("TONUBAN", s[3], req);
                }
            }
            else if (activated == 0)
            {
                if (enrollType == 1)
                {
                    if (req.Msg.Length == 1)
                    {
                        cnt = 102;
                        addParam("Bypass", cnt.ToString(), req);
                        //redirect to set PIN
                        req.next_op = 126;
                        return "0";
                    }

                    char[] delm = { '*', '#' };
                    string msg = req.Msg.Trim();
                    string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                    if (nuban == s[3].Trim())
                    {
                        req.next_op = 122;
                        return "9";
                    }
                    else
                    {
                        //return "0";
                        //addParam("AMOUNT", s[2], req);
                        //addParam("TONUBAN", s[3], req);
                        req.next_op = 126;
                    }
                    return "0";
                }

                //redirect to set PIN
                req.next_op = 115;
                return "0";
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    //update the account table
    public void Upd_acct_byTranstype(string sessionid, int val)
    {
        string sql = "update tbl_USSD_account_id set trans_type=@tt where sessionid = @sid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@tt", val);
        cn.AddParam("@sid", sessionid);
        cn.Update();
    }
    public string getResponseDesc(string rspcode)
    {
        string txt = "";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_nip");
        cn.SetProcedure("spd_GetResponseDesc");
        cn.AddParam("@respCode", rspcode);
        DataSet ds = cn.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            txt = dr["respText"].ToString();
        }
        else
        {
            txt = "";
        }
        return txt;
    }
    //sterling to other banks trnx
    public string DoInterBank(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = "";
        int k = 0;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            summary = int.Parse(prm["SUMMARY"]);
            string FRMACCT = prm["FROMACCT"];
            if (FRMACCT == "-1")
            {
                resp = "You are currently not registered for this USSD service. Dial *822*1*YourNUBAN#";
                return resp;
            }
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*5*AMT*NUBAN# to try again";
                return resp;
            }
            string NERESPONSE = prm["NERESPONSE"];
            if (NERESPONSE != "00" && NERESPONSE != null)
            {
                resp = "Transaction will not be processed.  Reason: " + getResponseDesc(NERESPONSE);
                return resp;
            }


            PIN = prm["PIN"];
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,bankcode,frmAccount) " +
                " values(@mb,@ta,@am,@pn,2,@sid,@bc,@frm)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@bc", prm["TOBANK"]);
            cn.AddParam("@frm", prm["FROMACCT"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 2);
            //resp = "Transaction has been submitted for processing";
            resp = "Your transaction has been submitted, do you want to save the beneficiary?%0A1.Yes%0A2.No";

        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }

    public string DoInterBankNoReply(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = "";
        int k = 0;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            summary = int.Parse(prm["SUMMARY"]);
            string FRMACCT = prm["FROMACCT"];
            if (FRMACCT == "-1")
            {
                resp = "You are currently not registered for this USSD service. Dial *822*1*YourNUBAN#";
                return resp;
            }
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*5*AMT*NUBAN# to try again";
                return resp;
            }
            string NERESPONSE = prm["NERESPONSE"];
            if (NERESPONSE != "00" && NERESPONSE != null)
            {
                resp = "Transaction will not be processed.  Reason: " + getResponseDesc(NERESPONSE);
                return resp;
            }


            PIN = prm["PIN"];
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,bankcode,frmAccount) " +
                " values(@mb,@ta,@am,@pn,2,@sid,@bc,@frm)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@bc", prm["TOBANK"]);
            cn.AddParam("@frm", prm["FROMACCT"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 2);
            resp = "Transaction has been submitted for processing";

        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
    //sterling to sterling trnx

    public string DoSubmitS2S(UReq req)
    {
        string resp = ""; int summary = 0; string PIN = "";
        int k = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            //*************************************************************
            //EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            //Gadget g = new Gadget();
            //string frmAccount = ""; string Ledcodeval = ""; string t24Mob = "";
            ////check if the customer is corporate
            //frmAccount = g.GetAccountsByMobileNo2(req.Msisdn);
            //DataSet ds = ws.getAccountFullInfo(frmAccount);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    DataRow dr = ds.Tables[0].Rows[0];
            //    Ledcodeval = dr["T24_LED_CODE"].ToString();
            //    t24Mob = dr["MOB_NUM"].ToString();
            //    if (Ledcodeval == "1200" || Ledcodeval == "1201" || Ledcodeval == "1202" || Ledcodeval == "1203")
            //    {
            //        return "Corporate customers are not allowed on this platform";
            //    }
            //    string themobnum = "";
            //    themobnum = req.Msisdn;
            //    if (themobnum.StartsWith("234"))
            //    {
            //        themobnum = themobnum.Replace("234", "0");
            //    }

            //    if (t24Mob.Trim() != themobnum.Trim())
            //    {
            //        return "The Mobile number your are using is different from mobile number in our record";
            //    }
            //}
            //***************************************************************
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*4*AMT*NUBAN# to try again";
                return resp;
            }
            PIN = prm["PIN"];

            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,1,@sid,@frmNuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@frmNuban", prm["frmNuban"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 1);
            //resp = "Transaction has been submitted for processing, do you want to save the beneficiary?%0A1.Yes%0A2.No";
            resp = "Your transaction has been submitted, do you want to save the beneficiary?%0A1.Yes%0A2.No";

            Gadget g = new Gadget();
            g.Insert_Charges(req.SessionID, g.GetAccountsByMobileNo2(req.Msisdn), 1);
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }

    public string DoSubmitS2SNoReply(UReq req)
    {
        string resp = ""; int summary = 0; string PIN = "";
        int k = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            //*************************************************************
            //EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            //Gadget g = new Gadget();
            //string frmAccount = ""; string Ledcodeval = ""; string t24Mob = "";
            ////check if the customer is corporate
            //frmAccount = g.GetAccountsByMobileNo2(req.Msisdn);
            //DataSet ds = ws.getAccountFullInfo(frmAccount);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    DataRow dr = ds.Tables[0].Rows[0];
            //    Ledcodeval = dr["T24_LED_CODE"].ToString();
            //    t24Mob = dr["MOB_NUM"].ToString();
            //    if (Ledcodeval == "1200" || Ledcodeval == "1201" || Ledcodeval == "1202" || Ledcodeval == "1203")
            //    {
            //        return "Corporate customers are not allowed on this platform";
            //    }
            //    string themobnum = "";
            //    themobnum = req.Msisdn;
            //    if (themobnum.StartsWith("234"))
            //    {
            //        themobnum = themobnum.Replace("234", "0");
            //    }

            //    if (t24Mob.Trim() != themobnum.Trim())
            //    {
            //        return "The Mobile number your are using is different from mobile number in our record";
            //    }
            //}
            //***************************************************************
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*4*AMT*NUBAN# to try again";
                return resp;
            }
            PIN = prm["PIN"];

            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,1,@sid,@frmNuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@frmNuban", prm["frmNuban"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 1);
            resp = "Transaction has been submitted for processing";

            Gadget g = new Gadget();
            g.Insert_Charges(req.SessionID, g.GetAccountsByMobileNo2(req.Msisdn), 1);
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
    //public string DoSubmitS2S(UReq req)
    //{
    //    string resp = ""; int summary = 0; string PIN = "";
    //    string prms = getParams(req);
    //    NameValueCollection prm = splitParam(prms);
    //    int k = 0;
    //    try
    //    {
    //        summary = int.Parse(prm["SUMMARY"]);
    //        if (summary == 2)
    //        {
    //            resp = "Transaction cancelled. Dial *822*9*AMT*NUBAN# to try again";
    //            return resp;
    //        }
    //        PIN = prm["PIN"];
    //        string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid) " +
    //            " values(@mb,@ta,@am,@pn,1,@sid)";
    //        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
    //        cn.SetSQL(sql);
    //        cn.AddParam("@mb", req.Msisdn);
    //        cn.AddParam("@ta", prm["TONUBAN"]);
    //        cn.AddParam("@am", prm["AMOUNT"]);
    //        cn.AddParam("@pn", PIN);
    //        cn.AddParam("@sid", req.SessionID);
    //        k = Convert.ToInt32(cn.Insert());

    //    }
    //    catch (Exception ex)
    //    {
    //        resp = "ERROR: Could not contact core banking system";
    //    }
    //    if (k > 0)
    //    {
    //        Upd_acct_byTranstype(req.SessionID, 1);
    //        resp = "Transaction has been submitted for processing";
    //        Gadget g = new Gadget();
    //        g.Insert_Charges(req.SessionID, prm["NUBAN"], 1);
    //    }
    //    else
    //    {
    //        resp = "Unable to submit please try again";
    //    }

    //    return resp;
    //}

    public string SaveFromAccount(UReq req)
    {
        //EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        //DataSet ds = ws.getCustomerAccountsByMobileNo(req.Msisdn);
        //*822*9*100*0005969437#
        string resp = "0";
        try
        {
            addParam("FROMNUBAN", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    //****************************************************************
    //****************************************************************
    public void insert(int id, string nuban, string sessionid)
    {
        string sql = "insert into tbl_USSD_account_id(acctid,nuban,sessionid) " +
            "values (@id,@nu,@sid)";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@id", id);
        cn.AddParam("@nu", nuban);
        cn.AddParam("@sid", sessionid);
        cn.Insert();
    }

    public string T24CurrLedcode(string lc)
    {
        string val = "";
        if (lc == "9")
        {
            val = "1006";
        }
        else if (lc == "1")
        {
            val = "1000";
        }
        else if (lc == "5051")
        {
            val = "1001";
        }
        else if (lc == "5052")
        {
            val = "1201";
        }
        else if (lc == "5054")
        {
            val = "1202";
        }
        else if (lc == "5082")
        {
            val = "1304";
        }
        else if (lc == "5060")
        {
            val = "1007";
        }
        else if (lc == "59")
        {
            val = "6001";
        }
        else if (lc == "84")
        {
            val = "6009";
        }
        else if (lc == "95")
        {
            val = "6003";
        }
        else if (lc == "73")
        {
            val = "6002";
        }
        else if (lc == "65")
        {
            val = "6009";
        }
        return val;
    }
    public string GetAcctByMobile(UReq req)
    {
        Gadget g = new Gadget();
        string resp = ""; string txt = ""; string cusid = "";
        try
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            //EWS.ServiceSoapClient ws = new EWS.ServiceSoapClient();
            string[] savings_ledcode = new string[] { "1000", "6001", "6002", "6003", "6004", "6009" };
            string[] current_ledcode = new string[] { "1000", "1001", "1006", "1007", "1201", "1202", "1304", "1305" };

            string[] AccountType_Sav = new string[] { System.Web.Configuration.WebConfigurationManager.AppSettings["AccountType_Sav"].ToString() };
            string[] AccountType_Cur = new string[] { System.Web.Configuration.WebConfigurationManager.AppSettings["AccountType_Cur"].ToString() };

            string STA_CODE = ""; string accttype = ""; int cnt = 0; string Currency_Code = "";
            string AccountType = ""; string imalNuban = ""; string imalBal = ""; string imalLedgerCode = "";
            string Msisdn = ConvertMobile080(req.Msisdn.Trim());
            try
            {
                //cusid = g.GetCusNumByMobileNo(Msisdn);
                cusid = g.GetT24CustId(Msisdn);
            }
            catch { }
            if (cusid == "")
            {
                DataSet imalDs = getImalAcctsByMobile(req.Msisdn.Trim());
                int imalCnt = imalDs.Tables[0].Rows.Count;
                if (imalCnt > 0)
                {
                    for (int x = 0; x < imalCnt; x++)
                    {
                        DataRow drim = imalDs.Tables[0].Rows[x];
                        imalNuban = drim["Nuban"].ToString();
                        imalLedgerCode = drim["LedgerCode"].ToString();
                        cnt += 1;
                        if (imalLedgerCode == "210801" || imalLedgerCode == "210804" || imalLedgerCode == "210805")
                        {
                            txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + imalNuban + "";
                        }
                        else if (imalLedgerCode == "210153" || imalLedgerCode == "210101" || imalLedgerCode == "210201")
                        {
                            txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + imalNuban + "";
                        }
                    }
                }
                else
                {
                    return "There is no account currently associated with this mobile number. Kindly dial *822*7# to open an account with us.";
                }

                return txt;
            }

            //DataSet ds =  g.GetAccountsByMobileNo(Msisdn);
            DataSet ds = ws.getCustomerAccountsByMobileNo2(Msisdn);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    Currency_Code = dr["Currency"].ToString();
                    if (Currency_Code == "566")
                    {
                        AccountType = dr["AccountType"].ToString().Trim().ToUpper();
                        if (AccountType != "")
                        {
                            if (AccountType_Cur.Any(x => x.Trim().ToUpper().Contains(AccountType)))
                            {
                                cnt += 1;
                                txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + dr["NUBAN"].ToString();
                                //insert(cnt, dr["MAP_ACC_NO"].ToString(), req.SessionID);
                            }

                            if (AccountType_Sav.Any(x => x.Trim().ToUpper().Contains(AccountType)))
                            {
                                cnt += 1;
                                txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + dr["NUBAN"].ToString();
                                //insert(cnt, dr["MAP_ACC_NO"].ToString(), req.SessionID);
                            }
                        }
                    }
                }


                DataSet imalDs = getImalAcctsByMobile(req.Msisdn.Trim());
                if (imalDs != null)
                {
                    int imalCnt = imalDs.Tables[0].Rows.Count;
                    if (imalCnt > 0)
                    {
                        for (int x = 0; x < imalCnt; x++)
                        {
                            DataRow drim = imalDs.Tables[0].Rows[x];
                            imalNuban = drim["Nuban"].ToString();
                            imalLedgerCode = drim["LedgerCode"].ToString();
                            cnt += 1;
                            if (imalLedgerCode == "210801" || imalLedgerCode == "210804" || imalLedgerCode == "210805")
                            {
                                txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + imalNuban + "";
                            }
                            else if (imalLedgerCode == "210153" || imalLedgerCode == "210101" || imalLedgerCode == "210201")
                            {
                                txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + imalNuban + "";
                            }
                        }
                    }
                    else
                    {
                        if (cnt == 0)
                        {
                            return "There is no account currently associated with this mobile number. Kindly dial *822*7# to open an account with us.";
                        }
                    }
                }
            }
            else
            {
                return "There is no account currently associated with this mobile number. Kindly dial *822*7# to open an account with us.";
            }
        }
        catch
        {
            return "An error has occured.  Kindly try again later";
        }
        return "Your NGN Accounts: " + txt + "%0A Dial *822*6# to check your account balance.";
    }



    //newly added 27 jul 2016
    public string SavePin(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("PIN", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    //Sterling to Sterling Summary
    //*********************** for IMAL **********************************
    //********************************************************************

    public string DisplaySummary(UReq req)
    {
        Gadget g = new Gadget(); int flag = 0; int ITEMSELECT = 0;
        string resp = ""; string ToNuban = ""; string frmNuban = "";
        string prms = getParams(req); string ToName = ""; decimal amt = 0;
        NameValueCollection prm = splitParam(prms);
        flag = int.Parse(prm["A"]);
        if (flag == 1)
        {
            frmNuban = prm["NUBAN"];
        }
        else if (flag == 2)
        {
            ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
            frmNuban = g.GetNubanByListID(ITEMSELECT, req);
        }
        addParam("frmNuban", frmNuban, req);
        ToNuban = prm["TONUBAN"];
        amt = decimal.Parse(prm["AMOUNT"]);
        //Check if the account is sterling or imal
        if (ToNuban.StartsWith("05"))
        {

            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = iMALEngine.GetImalDetailsByNuban(ToNuban);
            ToName = imalinfo.CustomerName;
        }
        else if (ToNuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = ToNuban;
            string bankOneReq = bank1.createRequestForNameEnq();
            var Bank1ref = LogBankOneRequest(req.SessionID, ToNuban, bankOneReq);
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
            bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
            LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
            ToName = bank1.accountName;
        }
        else
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_S2S_GetBeneByNuban");
            c.AddParam("@nuban", ToNuban);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ToName = dr["CUS_SHOW_NAME"].ToString();
            }
            else
            {
                ToName = "Unable to fetch beneficiary account name at this time.";
            }
        }

        if (ToName == "" || ToName == null)
        {
            resp = "An error occured at this time as we are unable to validate the customers details from the core.  Kindly try again later.";
        }
        else
        {
            //adding params for saving beneficiary
            addParam("ToName", ToName, req);
            addParam("BankName", "STERLING", req);
            addParam("ToBankCode", "99", req);
            resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + ToName + "%0A1.Yes%0A2.No";
        }
        return "Confirmation: " + resp;
    }
    //public string DisplaySummary(UReq req)
    //{
    //    Gadget g = new Gadget();
    //    string resp = ""; string ToNuban = ""; string frmNuban = "";
    //    string prms = getParams(req); string ToName = ""; decimal amt = 0;
    //    NameValueCollection prm = splitParam(prms);
    //    frmNuban = prm["NUBAN"];
    //    ToNuban = prm["TONUBAN"];
    //    //Check if the account is sterling or imal
    //    if (ToNuban.StartsWith("05"))
    //    {
    //        ImalResponses Irsp1 = new ImalResponses();
    //        ImalRequest Irqt = new ImalRequest();
    //        Irqt.account = ToNuban;
    //        Irqt.requestCode = "112";
    //        Irqt.principalIdentifier = "003";
    //        Irqt.referenceCode = "#" + Irqt.principalIdentifier + "#" + g.GenerateRndNumber(12);
    //        //call imal service
    //        imal1.NIBankingServiceClient ws = new imal1.NIBankingServiceClient();
    //        //serialize the json
    //        JavaScriptSerializer js = new JavaScriptSerializer();
    //        string json = js.Serialize(Irqt);
    //        json = g.Encrypt(json);
    //        try
    //        {
    //            resp = ws.process(json, Irqt.principalIdentifier);
    //            resp = g.Decrypt(resp);
    //            Irsp1 = js.Deserialize<ImalResponses>(resp);
    //            ToName = Irsp1.name;
    //        }
    //        catch (Exception ex)
    //        {
    //            ToName = "";
    //        }
    //    }
    //    else
    //    {
    //        amt = decimal.Parse(prm["AMOUNT"]);
    //        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
    //        c.SetProcedure("spd_S2S_GetBeneByNuban");
    //        c.AddParam("@nuban", ToNuban);
    //        DataSet ds = c.Select("rec");
    //        if (ds.Tables[0].Rows.Count > 0)
    //        {
    //            DataRow dr = ds.Tables[0].Rows[0];
    //            ToName = dr["CUS_SHOW_NAME"].ToString();
    //        }
    //    }
    //    if (ToName == "" || ToName == null)
    //    {
    //        resp = "An error occured at this time as we are unable to validate the customers details from the core.  Kindly try again later.";
    //    }
    //    else
    //    {
    //        resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + ToName + "%0A1.Yes%0A2.No";
    //    }
    //    return "Confirmation: " + resp;
    //}
    //public string DisplaySummary(UReq req)
    //{
    //    string prms = getParams(req); string resp = "";
    //    string ToNuban = ""; string ToName = ""; string cusMobile = "";
    //    NameValueCollection prm = splitParam(prms); string getdetails = ""; string frmNuban = ""; string frmName = "";
    //    string SessionID = req.SessionID; string Msisdn = req.Msisdn; decimal amt = 0;
    //    char[] sep = { '*' };
    //    getdetails = getSessionDetails(SessionID);
    //    string[] bits = getdetails.Split(sep, StringSplitOptions.RemoveEmptyEntries);
    //    cusMobile = bits[0]; ToName = bits[2]; ToNuban = bits[1]; amt = decimal.Parse(bits[3]);
    //    frmNuban = getNubanByMobile(cusMobile);
    //    cusMobile = ConvertMobile080(cusMobile);

    //    resp = "You are transferring the sum of " + getRealMoney(amt) + " from " +  frmNuban + " to "  + bits[2]  + "%0A1.Yes%0A2.No";
    //    return "Confirmation: " + resp;
    //}
    public string getNubanByMobile(string mobile)
    {
        string nuban = "";
        try
        {
            string sql = "select nuban from Go_Registered_Account where mobile =@mb ";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", mobile);

            DataSet ds = cn.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                nuban = dr["nuban"].ToString();
            }
        }
        catch (Exception ex)
        {

        }
        return nuban;
    }
    public string SaveSummary(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 514;
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveSummaryInter(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                //req.next_op = 14205;
                req.next_op = 14206;
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public string getRespDesc(string rcode)
    {
        string resptext = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_nip");
        c.SetProcedure("spd_GetResponseDesc");
        c.AddParam("@respCode", rcode);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            resptext = dr["respText"].ToString();
        }
        return resptext;
    }
    public static string ConvertMobile080(string mobile)
    {
        char[] trima = { '+', ' ' };
        mobile = mobile.Trim(trima);
        if (mobile.Length == 11 && mobile.StartsWith("0"))
        {
            return mobile;
        }
        if (mobile.Length >= 10)
        {
            mobile = "0" + mobile.Substring(mobile.Length - 10, 10);
            return mobile;
        }
        return mobile;
    }
    public string getSessionDetails(string sid)
    {
        string prms = "";
        string val = ""; string acctName = ""; decimal amt = 0; string mob = "";
        string sql = "select params,msisdn from  tbl_USSD_reqstate where sessionid = @sid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@sid", sid);
        DataSet ds = cn.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            prms = dr["params"].ToString();
            mob = dr["msisdn"].ToString();
            NameValueCollection prm = splitParam(prms);

            amt = decimal.Parse(prm["AMOUNT"]);
            DataSet ds1 = ws.GetAccountfromDByAccountNum(prm["TONUBAN"]);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                DataRow dr2 = ds1.Tables[0].Rows[0];
                acctName = dr2["CUS_SHO_NAME"].ToString();
            }
            val = mob + "*" + prm["TONUBAN"] + "*" + acctName + "*" + amt.ToString();
        }
        return val;
    }
    public string getFromAcctBySid(string sid)
    {
        string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getFromAcctNum");
        c.AddParam("@sid", sid);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["nuban"].ToString();
        }
        else
        {
            return "-1";
        }
        return nuban;
    }
    //display Interbank summary
    public string DoNameEnquiry(string toAcct, int tobankcode, string sidval)
    {
        Gadget g = new Gadget(); string RandomNum = ""; string SessionID = ""; string resp = ""; string Dest_bank_code = "";
        RandomNum = g.GenerateRndNumber(3);
        SessionID = g.newSessionGlobal(RandomNum, 1);
        Dest_bank_code = getBankcode(tobankcode.ToString());
        string ToName = "";
        char[] sep = { '*' };
        string[] bits = Dest_bank_code.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        //first check our local table 
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_nip");
        c.SetProcedure("spd_Get_NameEnquiry_From_Table0504");
        c.AddParam("@nuban", toAcct);
        c.AddParam("@bankcode", bits[0]);

        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            return "00:" + dr["AccountName"].ToString() + ":" + " " + ":" + bits[1];
        }
        else
        {

            //call NIBSS based on the toAcct number
            NIPNameEnquiry.NewIBSSoapClient ws = new NIPNameEnquiry.NewIBSSoapClient();
            resp = ws.NameEnquiry(SessionID, bits[0], "3", toAcct);
            resp = resp.Replace("::", ":");
            resp = resp.Replace(":1", ":");
        }
        return resp + ":" + bits[1];
    }
    public string DisplaySummaryInter(UReq req)
    {
        string resp = ""; decimal amt = 0; string frmNuban = ""; string theResp = "";
        string prms = getParams(req); int flag = 0; Gadget g = new Gadget();
        NameValueCollection prm = splitParam(prms);
        string SessionID = req.SessionID; char[] sep = { ':' };
        theResp = DoNameEnquiry(prm["TONUBAN"], int.Parse(prm["TOBANK"]), SessionID);
        amt = decimal.Parse(prm["AMOUNT"]);
        string[] bits = theResp.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        int ITEMSELECT = 0;
        flag = int.Parse(prm["A"]);
        if (flag == 1)
        {
            frmNuban = prm["NUBAN"];
        }
        else if (flag == 2)
        {
            ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
            frmNuban = g.GetNubanByListID(ITEMSELECT, req);
        }

        addParam("FROMACCT", frmNuban, req);
        if (frmNuban == "-1" || frmNuban == "")
        {
            req.next_op = 14205;
            return "You are currently not registered with us for USSD service.  Dial *822*1*YourNUBAN#";
        }
        addParam("NERESPONSE", bits[0], req);



        if (bits[0] == "00")
        {
            //adding params for saving beneficiary
            addParam("ToName", bits[1], req);
            addParam("BankName", bits[3], req);

            resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + bits[1] + "(" + bits[3] + ")" + "%0A1.Yes%0A2.No";
        }
        else
        {
            resp = getRespDesc(bits[0]) + " kindly try again later.";
            req.next_op = 14205;
            return "Error occured: " + resp;
        }
        return "Confirmation: " + resp;
    }
    public string getBankcode(string the_bcode)
    {
        string bcode = "";
        try
        {
            bcode = getTheBankCode(the_bcode);
        }
        catch (Exception ex)
        {
        }
        return bcode;
    }
    public string getTheBankCode(string bcode)
    {
        string bcodeval = "";
        try
        {
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_nip");
            cn.SetProcedure("spd_GetParticipatingBanksByID");
            cn.AddParam("@id", bcode);
            DataSet ds = cn.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                bcodeval = dr["bankcode"].ToString() + "*" + dr["bankname"].ToString();
            }
        }
        catch { }
        return bcodeval;
    }

    //Authentication:%0AEnter your USSD PIN
    public string CollectUSSDPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter your USSD PIN";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 4 digits";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 digits and not alphanumeric";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The USSD PIN you provided is incorrect. Please check and try again.";
            return "Authentication:%0A" + resp;
        }
        resp = "Enter your USSD PIN";
        return "Authentication:%0A" + resp;
    }

    public string SavetxnPIN(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 513;
            return "9";
        }
        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 513;
            return "9";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {

            try
            {
                addParam("PIN", req.Msg, req);
                //submit request to transfer is amount less than 20K

                int amt = RunCount(prm["AMOUNT"]);
                if (amt < 20000)
                {
                    //Check if beneficiary is saved
                    bool isBeneSaved = CheckIfBeneficiaryIsSaved(99, prm["TONUBAN"], req.Msisdn);//99 for sterling customers
                    if (isBeneSaved)
                    {
                        //Direct to DoSubmitS2SNoReply
                        req.next_op = 514;
                        return "1";
                    }
                    else
                    {
                        //Direct to DoSubmitS2S (Beneficiary not saved)
                        req.next_op = 514;
                        return "0";
                    }

                }
            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 513;
            return "9";
        }


        return resp;
    }
    public string SavetxnPIN2(UReq req)
    {
        int cnt = 0;

        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 14205;
            return "9";
        }
        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 14205;
            return "9";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);
        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {
            try
            {
                addParam("PIN", req.Msg, req);
                //submit request to transfer is amount less than 20K

                int amt = RunCount(prm["AMOUNT"]);
                if (amt < 20000)
                {
                    //Check if beneficiary is saved
                    bool isBeneSaved = CheckIfBeneficiaryIsSaved(Convert.ToInt16(prm["TOBANK"]), prm["TONUBAN"], req.Msisdn);
                    if (isBeneSaved)
                    {
                        //Direct to DoSubmitS2SNoReply
                        req.next_op = 14206;
                        return "1";
                    }
                    else
                    {
                        //Direct to DoSubmitS2S (Beneficiary not saved)
                        req.next_op = 14206;
                        return "0";
                    }
                }
            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 14205;
            return "9";
        } 

        return resp;
    }


    //balance
    public string GetBalanceByCustID(UReq req)
    {
        Gadget g = new Gadget();
        string resp = ""; string txt = "";
        try
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            string cusid = "";
            List<string> custidList = new List<string>();
            string[] savings_ledcode = new string[] { "1000", "6001", "6002", "6003", "6004", "6009" };
            string[] current_ledcode = new string[] { "1000", "1001", "1006", "1007", "1201", "1202", "1304", "1305" };

            string[] AccountType_Sav = new string[] { System.Web.Configuration.WebConfigurationManager.AppSettings["AccountType_Sav"].ToString() };
            string[] AccountType_Cur = new string[] { System.Web.Configuration.WebConfigurationManager.AppSettings["AccountType_Cur"].ToString() };

            string STA_CODE = ""; string accttype = ""; int cnt = 0; string Currency_Code = ""; string T24_LED_CODE = "";
            string Msisdn = ConvertMobile080(req.Msisdn.Trim()); string AccountType = ""; decimal bal = 0;
            string defAcct = "";
            //get the customerid by mobile
            try
            {
                custidList = g.GetT24CustIdList(Msisdn).Distinct().ToList();
                defAcct = getDefaultAccount(req.Msisdn);
            }
            catch { }

            if (defAcct.StartsWith("11"))
            {
                //Treat as BankOne
                BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
                //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
                BankOneClass bank1 = new BankOneClass();
                bank1.accountNumber = defAcct;
                string bankOneReq = bank1.createReqForBalanceEnq();
                var Bank1ref = LogBankOneRequest(req.SessionID, defAcct, bankOneReq);
                string encrptdReq = EncryptTripleDES(bankOneReq);
                string getBankOneResp = bankOne_WS.BankOneBalanceEnquiry(encrptdReq);
                bool isBankOne = bank1.readRespForBalEnq(getBankOneResp);
                LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
                bal = bank1.availableBal;
                return "Acct: " + defAcct + " Bal: " + bal.ToString("#,##0.00");
            }

            if ((custidList == null) || !custidList.Any())
            {
                //go to imal to get the details
                string imalNuban = ""; string imalBal = ""; string imalLedgerCode = ""; decimal thebalval = 0;
                IMALEngine imalEng = new IMALEngine();
                DataSet imalDs = getImalAcctsByMobile(req.Msisdn.Trim());
                int imalCnt = imalDs.Tables[0].Rows.Count;
                if (imalCnt > 0)
                {
                    for (int x = 0; x < imalCnt; x++)
                    {
                        DataRow drim = imalDs.Tables[0].Rows[x];
                        imalNuban = drim["Nuban"].ToString();
                        imalBal = imalEng.GetRealImalBalance(drim["Balance"].ToString());
                        imalLedgerCode = drim["LedgerCode"].ToString();
                        cnt += 1;
                        thebalval = Convert.ToDecimal(imalBal);
                        if (imalLedgerCode == "210801" || imalLedgerCode == "210804" || imalLedgerCode == "210805")
                        {
                            txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + imalNuban + " Bal: " + thebalval.ToString("#,##0.00");
                        }
                        else if (imalLedgerCode == "210153" || imalLedgerCode == "210101" || imalLedgerCode == "210201")
                        {
                            txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + imalNuban + " Bal: " + thebalval.ToString("#,##0.00");
                        }
                    }
                }
                else
                {
                    return "There is no account currently associated with this mobile number. Kindly dial *822*7# to open an account with us.";
                }

                return txt;
            }

            foreach (var custInfo in custidList)
            {
                cusid = custInfo;
                DataSet ds = ws.getCustomerAccountsByCustomerId(cusid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        Currency_Code = dr["T24_CUR_CODE"].ToString();
                        T24_LED_CODE = dr["T24_LED_CODE"].ToString();
                        if (T24_LED_CODE == "NULL")
                        {
                            continue;
                        }
                        if (T24_LED_CODE == "3101" || T24_LED_CODE == "3144")
                        {
                            continue;
                        }
                        bal = decimal.Parse(dr["UsableBal"].ToString());
                        if (bal == 0)
                        {
                            continue;
                        }
                        if (Currency_Code == "566")
                        {
                            if (bal >= 0)
                            {
                                AccountType = dr["ACCT_TYPE"].ToString().Trim().ToUpper();
                                if (AccountType != "")
                                {
                                    if (AccountType_Cur.Any(x => x.Trim().ToUpper().Contains(AccountType)))
                                    {
                                        cnt += 1;
                                        txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + dr["NUBAN"].ToString() + " Bal: " + bal.ToString("#,##0.00");
                                    }

                                    if (AccountType_Sav.Any(x => x.Trim().ToUpper().Contains(AccountType)))
                                    {
                                        cnt += 1;
                                        txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + dr["NUBAN"].ToString() + " Bal: " + bal.ToString("#,##0.00");
                                    }
                                }
                            }
                        }

                    }

                    //go to imal to get the details 
                    string imalNuban = ""; string imalBal = ""; string imalLedgerCode = ""; decimal thebalval = 0;
                    IMALEngine imalEng = new IMALEngine();
                    DataSet imalDs = getImalAcctsByMobile(req.Msisdn.Trim());
                    if (imalDs != null)
                    {
                        int imalCnt = imalDs.Tables[0].Rows.Count;
                        if (imalCnt > 0)
                        {
                            for (int x = 0; x < imalCnt; x++)
                            {
                                DataRow drim = imalDs.Tables[0].Rows[x];
                                imalNuban = drim["Nuban"].ToString();
                                imalBal = imalEng.GetRealImalBalance(drim["Balance"].ToString());
                                imalLedgerCode = drim["LedgerCode"].ToString();
                                cnt += 1;
                                thebalval = Convert.ToDecimal(imalBal);
                                if (imalLedgerCode == "210801" || imalLedgerCode == "210804" || imalLedgerCode == "210805")
                                {
                                    txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + imalNuban + " Bal: " + thebalval.ToString("#,##0.00");
                                }
                                else if (imalLedgerCode == "210153" || imalLedgerCode == "210101" || imalLedgerCode == "210201")
                                {
                                    txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + imalNuban + " Bal: " + thebalval.ToString("#,##0.00");
                                }
                            }
                        }
                        else
                        {
                            if (cnt == 0)
                            {
                                return "There is no account currently associated with this mobile number. Kindly dial *822*7# to open an account with us.";
                            }
                        }
                    }

                }
                else
                {
                    //go to imal to get the details
                    string imalNuban = ""; string imalBal = ""; string imalLedgerCode = ""; decimal thebalval = 0;
                    IMALEngine imalEng = new IMALEngine();
                    DataSet imalDs = getImalAcctsByMobile(req.Msisdn.Trim());
                    int imalCnt = imalDs.Tables[0].Rows.Count;
                    if (imalCnt > 0)
                    {
                        for (int x = 0; x < imalCnt; x++)
                        {
                            DataRow drim = imalDs.Tables[0].Rows[x];
                            imalNuban = drim["Nuban"].ToString();
                            imalBal = imalEng.GetRealImalBalance(drim["Balance"].ToString());
                            imalLedgerCode = drim["LedgerCode"].ToString();
                            cnt += 1;
                            thebalval = Convert.ToDecimal(imalBal);
                            if (imalLedgerCode == "210801" || imalLedgerCode == "210804" || imalLedgerCode == "210805")
                            {
                                txt += " %0A " + cnt.ToString() + ". " + "(SAV)" + imalNuban + " Bal: " + thebalval.ToString("#,##0.00");
                            }
                            else if (imalLedgerCode == "210153" || imalLedgerCode == "210101" || imalLedgerCode == "210201")
                            {
                                txt += " %0A " + cnt.ToString() + ". " + "(CRNT)" + imalNuban + " Bal: " + thebalval.ToString("#,##0.00");
                            }
                        }
                    }
                    else
                    {
                        if (cnt == 0)
                        {
                            return "There is no account currently associated with this mobile number. Kindly dial *822*7# to open an account with us.";
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            return "An error has occured.  Kindly try again ";
        }
        return txt + "%0AYou can now recharge your phone directly from your bank acccount. Simply dial *822*Amount#.";
    }


    //4th Aug 2017************************************************************************************

    //Sterling to Sterling transfer.... From long string
    public string CheckNoofAccts(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer

        string nuban = "";
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 500;
            return "0";
        }
        else
        {
            //instead of listing accounts set NUBAN as primary/most frequently used
            //nuban = GetPrimaryAccount(req.Msisdn.Trim());
            //if (string.IsNullOrEmpty(nuban))
            //{
            //    nuban = GetMostUsedAccount(req.Msisdn.Trim());
            //    if (string.IsNullOrEmpty(nuban))
            //    {
            //        nuban = getDefaultAccount(req.Msisdn.Trim());
            //    }
            //}

            //addParam("A", "1", req);
            //removeParam("NUBAN", req);
            //addParam("NUBAN", nuban, req);
            //req.next_op = 500;
            //return "0";
            req.next_op = 401;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb(UReq req)
    {
        string primeAcct = GetPrimaryAccount(req.Msisdn);
        if (string.IsNullOrEmpty(primeAcct))
        {
            //Get most used account for transfers
            string favAcct = GetMostUsedAccount(req.Msisdn);
            if (!string.IsNullOrEmpty(favAcct))
            {
                //Order by most used account
                Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect("mssqlconn");
                cc.SetProcedure("spd_getAcctListWithPrime");
                cc.AddParam("@mob", req.Msisdn);
                cc.AddParam("@sessionid", req.SessionID);
                cc.AddParam("@account", favAcct.Trim());
                int ccn = cc.ExecuteProc();
                if (ccn > 0)
                {
                    req.next_op = 402;
                    return "0";
                }
                else
                {
                    return "0";
                }
            }

            //Use default list
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getAcctList");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@sessionid", req.SessionID);
            int cn = c.ExecuteProc();
            if (cn > 0)
            {
                req.next_op = 402;
                return "0";
            }
            else
            {
                return "0";
            }
        }
        else
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getAcctListWithPrime");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@sessionid", req.SessionID);
            c.AddParam("@account", primeAcct.Trim());
            int cn = c.ExecuteProc();
            if (cn > 0)
            {
                req.next_op = 402;
                return "0";
            }
            else
            {
                return "0";
            }
        }
    }
    public string ListCustomerAccts(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            List<AccountList> lb = g.GetListofAcctsByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("NXTPAG", req);
                lb = g.GetListofAcctsByPage("0", req.SessionID);
            }
            //resp = "Select Biller Item";
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.Nuban;
                }
            }

            if (cntfirst < 5)
            {
                resp += "";
            }
            else
            {
                resp += "%0A0 Next";
            }
        }
        catch
        {
        }
        return "Select Account%0A" + resp;
    }
    public int SaveAcctSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                //string prms = getParams(req);
                //NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        string nuban = g.GetNubanByListID(int.Parse(req.Msg), req);
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_CheckCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                int activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (nuban != regisNuban)
                {
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    nuban = regisNuban;
                    if (activated == 0)
                    {
                        cnt = -1;
                        continue;
                    }
                    if (activated == 1)
                    {
                        cnt = 1;
                        break;
                    }
                }

            }
            if (cnt == -1)
            {
                CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                string response = cardws.GetActiveCards(nuban);
                if (response.StartsWith("00"))
                {
                    cnt = 102;
                    removeParam("NUBAN", req);
                    addParam("NUBAN", nuban, req);
                    addParam("Bypass", cnt.ToString(), req);
                    req.op = 126;
                    return 0;
                }
                else
                {
                    removeParam("NUBAN", req);
                    addParam("NUBAN", nuban, req);
                    req.op = 124;
                    return 0;
                }
            }
        }
        else
        {
            //customer is okay to continue
        }
        string amount = prm["AMOUNT"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);

        //if amount is over 20k check if customer has set pin 2
        if (amtval > 20000)
        {
            var isPin2Set = checkForPin2(nuban, req.Msisdn, req);
            if (!isPin2Set)
            {
                addParam("NUBAN", nuban, req);

                req.op = 777;
                return 0;
            }
        }

        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = new ImalDetails();
            imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
            if (imalinfo.Status == 0)
            {
                req.op = 4190;
                return 0;
            }
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                DataRow dr1 = ds1.Tables[0].Rows[0];
                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                int activated = int.Parse(dr1["Activated"].ToString());

                if (trnxLimit == 1 && activated == 1)
                {
                    Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                    getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);

                    if (limitedTuple.Item2 < amtval + Totaldone)
                    {
                        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            cnt = 111;
                            addParam("NoCard", cnt.ToString(), req);
                            req.op = 4290;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
                            return 0;
                        }
                    }
                }

                if (trnxLimit == 0)
                {
                    Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                    if (limitedTuple.Item1 < amtval)
                    {
                        //Reroute to amount field
                        cnt = 104;
                        int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                        removeParam("AMOUNT", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 91100;
                        return 9;

                    }
                    if (limitedTuple.Item2 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 91100;
                        return 9;
                    }
                }
            }
            else
            {
                cnt = 102;
                removeParam("AMOUNT", req);
                addParam("cnt", cnt.ToString(), req);
                req.op = 91100;
                return 9;
            }

        }
        else if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            string bankOneReq = bank1.createReqForBalanceEnq();
            var Bank1ref = LogBankOneRequest(req.SessionID, nuban, bankOneReq);
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneBalanceEnquiry(encrptdReq);
            bool isBankOne = bank1.readRespForBalEnq(getBankOneResp);
            LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
            cusbal = bank1.availableBal;
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                DataRow dr1 = ds1.Tables[0].Rows[0];
                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                int activated = int.Parse(dr1["Activated"].ToString());
                if (trnxLimit == 0)
                {
                    Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                    if (limitedTuple.Item1 < amtval)
                    {
                        //Reroute to amount field
                        cnt = 104;
                        removeParam("AMOUNT", req);
                        int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 91100;
                        return 9;

                    }
                    if (limitedTuple.Item2 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 91100;
                        return 9;
                    }
                }
            }
            else
            {
                cnt = 102;
                removeParam("AMOUNT", req);
                addParam("cnt", cnt.ToString(), req);
                req.op = 91100;
                return 9;
            }
        }
        else
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet dss = ws.getAccountFullInfo(nuban);
            if (dss.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dss.Tables[0].Rows[0];
                string restFlag = dr["REST_FLAG"].ToString();
                if (restFlag == "TRUE")
                {
                    //Check for restriction code
                    var restCode = dss.Tables[1].Rows[0]["RestrCode"].ToString();
                    var restrictedCodes = ConfigurationManager.AppSettings["RestrictedCodes"].Split(',');

                    var isRestricted = restrictedCodes.Contains(restCode, StringComparer.OrdinalIgnoreCase);
                    if (isRestricted)
                    {
                        cnt = 99;
                        addParam("RestFlag", cnt.ToString(), req);
                        req.op = 419;
                        return 0;
                    }
                }
                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                if (cusbal > amtval)
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                    int activated = int.Parse(dr1["Activated"].ToString());

                    if (trnxLimit == 1 && activated == 1)
                    {
                        Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                        getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        if (limitedTuple.Item2 < amtval + Totaldone)
                        {
                            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                cnt = 111;
                                addParam("NoCard", cnt.ToString(), req);
                                req.op = 4290;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
                                return 0;
                            }
                        }
                    }

                    if (trnxLimit == 0)
                    {
                        Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                        getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        if (limitedTuple.Item1 < amtval)
                        {
                            //Reroute to amount field
                            cnt = 104;
                            removeParam("AMOUNT", req);
                            int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 91100;
                            return 9;


                        }
                        if (limitedTuple.Item2 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            removeParam("AMOUNT", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 91100;
                            return 9;
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    removeParam("AMOUNT", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 91100;
                    return 9;
                }
            }
            else
            {
                req.op = 4190;
                return 0;
            }
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }

    //Interbank transfer from long string
    public string CheckNoofAccts1(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer

        string nuban = "";

        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 14000;
            return "0";
        }
        else
        {
            ////instead of listing accounts set NUBAN as primary/most frequently used
            //nuban = GetPrimaryAccount(req.Msisdn.Trim());
            //if (string.IsNullOrEmpty(nuban))
            //{
            //    nuban = GetMostUsedAccount(req.Msisdn.Trim());
            //    if (string.IsNullOrEmpty(nuban))
            //    {
            //        nuban = getDefaultAccount(req.Msisdn.Trim());
            //    }
            //}

            //addParam("A", "1", req);
            //removeParam("NUBAN", req);
            //addParam("NUBAN", nuban, req);
            //req.next_op = 14000;
            //return "0";

            req.next_op = 601;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb1(UReq req)
    {
        string primeAcct = GetPrimaryAccount(req.Msisdn);
        if (string.IsNullOrEmpty(primeAcct))
        {
            //Get most used account for transfers
            string favAcct = GetMostUsedAccount(req.Msisdn);
            if (!string.IsNullOrEmpty(favAcct))
            {
                //Order by most used account
                Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect("mssqlconn");
                cc.SetProcedure("spd_getAcctListWithPrime");
                cc.AddParam("@mob", req.Msisdn);
                cc.AddParam("@sessionid", req.SessionID);
                cc.AddParam("@account", favAcct.Trim());
                int ccn = cc.ExecuteProc();
                if (ccn > 0)
                {
                    req.next_op = 602;
                    return "0";
                }
                else
                {
                    return "0";
                }
            }

            //Use default list
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getAcctList");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@sessionid", req.SessionID);
            int cn = c.ExecuteProc();
            if (cn > 0)
            {
                req.next_op = 602;
                return "0";
            }
            else
            {
                return "0";
            }
        }
        else
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getAcctListWithPrime");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@sessionid", req.SessionID);
            c.AddParam("@account", primeAcct.Trim());
            int cn = c.ExecuteProc();
            if (cn > 0)
            {
                req.next_op = 602;
                return "0";
            }
            else
            {
                return "0";
            }
        }

    }
    public string ListCustomerAccts1(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            List<AccountList> lb = g.GetListofAcctsByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("NXTPAG", req);
                lb = g.GetListofAcctsByPage("0", req.SessionID);
            }
            //resp = "Select Biller Item";
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.Nuban;
                }
            }

            if (cntfirst < 5)
            {
                resp += "";
            }
            else
            {
                resp += "%0A0 Next";
            }
        }
        catch
        {
        }
        return "Select Account%0A" + resp;
    }
    public int SaveAcctSelected1(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                //string prms = getParams(req);
                //NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        string nuban = g.GetNubanByListID(int.Parse(req.Msg), req);
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_CheckCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                int activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (nuban != regisNuban)
                {
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    nuban = regisNuban;
                    if (activated == 0)
                    {
                        cnt = -1;
                        continue;
                    }
                    if (activated == 1)
                    {
                        cnt = 1;
                        break;
                    }
                }

            }
            if (cnt == -1)
            {
                CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                string response = cardws.GetActiveCards(nuban);
                if (response.StartsWith("00"))
                {
                    cnt = 102;
                    removeParam("NUBAN", req);
                    addParam("NUBAN", nuban, req);
                    addParam("Bypass", cnt.ToString(), req);
                    req.op = 126;
                    return 0;
                }
                else
                {
                    cnt = 111;
                    addParam("cnt", cnt.ToString(), req);
                    removeParam("NUBAN", req);
                    addParam("NUBAN", nuban, req);
                    req.op = 124;
                    return 0;
                }
            }
        }
        else
        {
            //customer is okay to continue
        }
        string amount = prm["AMOUNT"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);

        //if amount is over 20k check if customer has set pin 2
        if (amtval > 20000)
        {
            var isPin2Set = checkForPin2(nuban, req.Msisdn, req);
            if (!isPin2Set)
            {
                addParam("NUBAN", nuban, req);

                req.op = 777;
                return 0;
            }
        }

        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = new ImalDetails();
            imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
            if (imalinfo.Status == 0)
            {
                req.op = 4190;
                return 0;
            }
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                DataRow dr1 = ds1.Tables[0].Rows[0];
                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                int activated = int.Parse(dr1["Activated"].ToString());

                if (trnxLimit == 1 && activated == 1)
                {
                    Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                    getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);

                    if (limitedTuple.Item2 < amtval + Totaldone)
                    {
                        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            cnt = 111;
                            addParam("NoCard", cnt.ToString(), req);
                            req.op = 4290;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
                            return 0;
                        }
                    }
                }

                if (trnxLimit == 0)
                {
                    Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                    if (limitedTuple.Item1 < amtval)
                    {
                        //Reroute to amount field
                        cnt = 104;
                        int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                        removeParam("AMOUNT", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 92006;
                        return 9;
                    }
                    if (limitedTuple.Item2 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 92006;
                        return 9;
                    }
                }
            }
            else
            {
                cnt = 102;
                removeParam("AMOUNT", req);
                addParam("cnt", cnt.ToString(), req);
                req.op = 92006;
                return 9;
            }

        }
        else if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            string bankOneReq = bank1.createReqForBalanceEnq();
            var Bank1ref = LogBankOneRequest(req.SessionID, nuban, bankOneReq);
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneBalanceEnquiry(encrptdReq);
            bool isBankOne = bank1.readRespForBalEnq(getBankOneResp);
            LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
            cusbal = bank1.availableBal;
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                DataRow dr1 = ds1.Tables[0].Rows[0];
                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                int activated = int.Parse(dr1["Activated"].ToString());
                if (trnxLimit == 0)
                {
                    Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                    getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                    if (limitedTuple.Item1 < amtval)
                    {
                        //Reroute to amount field
                        cnt = 104;
                        removeParam("AMOUNT", req);
                        int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 92006;
                        return 9;

                    }
                    if (limitedTuple.Item2 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 92006;
                        return 9;
                    }
                }
            }
            else
            {
                cnt = 102;
                removeParam("AMOUNT", req);
                addParam("cnt", cnt.ToString(), req);
                req.op = 92006;
                return 9;
            }
        }
        else
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet dss = ws.getAccountFullInfo(nuban);
            if (dss.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dss.Tables[0].Rows[0];
                string restFlag = dr["REST_FLAG"].ToString();
                if (restFlag == "TRUE")
                {
                    //Check for restriction code
                    var restCode = dss.Tables[1].Rows[0]["RestrCode"].ToString();
                    var restrictedCodes = ConfigurationManager.AppSettings["RestrictedCodes"].Split(',');

                    var isRestricted = restrictedCodes.Contains(restCode, StringComparer.OrdinalIgnoreCase);
                    if (isRestricted)
                    {
                        cnt = 99;
                        addParam("RestFlag", cnt.ToString(), req);
                        req.op = 419;
                        return 0;
                    }
                }
                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                if (cusbal > amtval)
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                    int activated = int.Parse(dr1["Activated"].ToString());

                    if (trnxLimit == 1 && activated == 1)
                    {
                        Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                        getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        if (limitedTuple.Item2 < amtval + Totaldone)
                        {
                            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                cnt = 111;
                                addParam("NoCard", cnt.ToString(), req);
                                req.op = 4290;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
                                return 0;
                            }
                        }
                    }

                    if (trnxLimit == 0)
                    {
                        Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                        getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        //getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                        if (limitedTuple.Item1 < amtval)
                        {
                            //Reroute to amount field
                            cnt = 104;
                            removeParam("AMOUNT", req);
                            int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 92006;
                            return 9;


                        }
                        if (limitedTuple.Item2 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            removeParam("AMOUNT", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 92006;
                            return 9;
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    removeParam("AMOUNT", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 92006;
                    return 9;
                }
            }
            else
            {
                req.op = 4190;
                return 0;
            }
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }
    //************************************************************************************************

    public string CollectUSSDPIN2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter your USSD PIN 2";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 4 digits";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 digits and not alphanumeric";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The USSD PIN 2 you provided is incorrect. Please check and try again.";
            return "Authentication for PIN 2:%0A" + resp;
        }
        resp = "Enter your USSD PIN 2";
        return "Authentication:%0A" + resp;
    }

    public string Savetxn4PIN(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 515;
            return "0";
        }
        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 515;
            return "0";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN2");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {

            try
            {
                addParam("PIN2", req.Msg, req);

                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                //Check if beneficiary is saved
                bool isBeneSaved = CheckIfBeneficiaryIsSaved(99, prm["TONUBAN"], req.Msisdn);//99 for sterling customers
                if (isBeneSaved)
                {
                    //Direct to DoSubmitS2SNoReply
                    req.next_op = 514;
                    return "1";
                }
                else
                {
                    //Direct to DoSubmitS2S (Beneficiary not saved)
                    req.next_op = 514;
                    return "0";
                }
            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 515;
            return "0";
        }

        return resp;
    }
    public string Savetxn4PIN2(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 14207;
            return "0";
        }
        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 14207;
            return "0";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);
        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN2");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {

            try
            {
                addParam("PIN2", req.Msg, req);

                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                //Check if beneficiary is saved
                bool isBeneSaved = CheckIfBeneficiaryIsSaved(Convert.ToInt16(prm["TOBANK"]), prm["TONUBAN"], req.Msisdn);
                if (isBeneSaved)
                {
                    //Direct to DoInterBankNoReply
                    req.next_op = 14206;
                    return "1";
                }
                else
                {
                    //Direct to DoInterBank (Beneficiary not saved)
                    req.next_op = 14206;
                    return "0";
                }
            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 14207;
            return "0";
        }

        return resp;
    }

    public string SameAccount(UReq req)
    {
        string resp = "";
        resp = "Transaction not allowed on same account.";
        return resp;
    }

    //**************Set PIN 2 flow ******
    //
    public string CollectLast6Digits(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter last 6 digits of your card:";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is exactly 6 digits";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is not alphanumeric";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Oops! You have entered the last 6 digits of your Debit Card wrongly. Please check your Card and try again.";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly request for a debit card for this account to upgrade your transaction limit.%0ATo request for an Instant Card, please call +23470078375464 or visit a Branch close to you.";
            //resp = "Enter last 6 digits of your card to activate your profile:";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 112)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly request for a debit card from your bank for this account to upgrade your transaction limit.";
            //resp = "Enter last 6 digits of your card to activate your profile:";
            return "" + resp;
        }

        resp = "Enter last 6 digits of your card:";
        return "Authentication for PIN 2:%0A" + resp;
    }
    public string SaveLastSixDigits(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        int wrongCnt = RunCount(prm["wrongCnt"]);
        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();

        try
        {
            long last6digits = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 777;
            return "0";
        }
        if (req.Msg.Length != 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 777;
            return "0";
        }

        //Check if BankOne Customer
        if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            bank1.last6Digits = req.Msg.Trim();
            string bankOneReq = bank1.createReqForCardValidation();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            var getBankOneResp = bankOne_WS.CardIssuanceCardValidation(encrptdReq);
            bool isBankOne = bank1.readRespForCardValidation(getBankOneResp);
            if (!isBankOne)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 777;
                return "0";
            }
            return "0";
        }

        //if wiseBank = 1 validate with wise bank
        int wiseBank = RunCount(prm["Wisebank"]);
        if(wiseBank == 1)
        {
            WisebankClass wise = new WisebankClass();

            //check for valid card linked to account
            var cardCheck = wise.CheckForValidCard(nuban, req.SessionID);
            if (!cardCheck.result)
            {
                cnt = 111;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 777;
                return "0";
            }

            var cardValidation = wise.ValidateCardPan(nuban, req.Msg, req.SessionID);
            if (!cardValidation.result)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 777;
                return "0";
            }

            //Continue. Everything is ok
            return "0";

        }

        string response = cardws.GetActiveCards(nuban);
        if (response.StartsWith("00"))
        {
            cnt = 111;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 777;
            return "0";
        }

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_PostCard");
        c.SetProcedure("getCrdDet_From6Digits");
        c.AddParam("@pan", req.Msg);
        c.AddParam("@nuban", nuban);
        DataSet dss = c.Select("rec");
        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
        {
            //last 6 digits match with the nuban
            //continue
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 777;
            return "0";
        }

        return resp;
    }
    public string CollectNewPin2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4 digit number of your choice";
            return "Set your PIN 2:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is exactly 4 digits";
            return "Set your PIN 2:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is not alphanumeric";
            return "Set your PIN 2:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Please set a different PIN 2 from your PIN 1";
            return "Set your PIN 2:%0A" + resp;
        }
        resp = "Enter any 4 digit number of your choice";
        return "Set your PIN 2:%0A" + resp;
    }
    public string ValidatePin2(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];

        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 779;
            return "0";
        }
        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 779;
            return "0";
        }

        string PIN2 = req.Msg;
        string PIN1 = getUSSDPin1(req.Msisdn.Trim(), nuban);

        if (PIN1 == PIN2)//redirect to 
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 779;
        }
        return resp;

    }
    public string SetNewPin2(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_SetCustPin2");
        c.AddParam("@mobile", req.Msisdn);
        c.AddParam("@custauthid", req.Msg);
        c.AddParam("@nuban", nuban);
        int cn = c.Update();
        if (cn > 0)
        {

            resp = "Your Pin 2 has been set successfully. %0AYou can now do transactions over 20,000";
            return resp;
        }
        else
        {
            resp = "Your Pin 2 was not successfully set. Kindly try again later";
            return resp;
        }
    }

   
    //SaveBeneInfo
    public string SaveBeneInfo(UReq req)
    {
        string resp = ""; int saveBene = 0;
        string toBankAcct = "";
        string toBankCode = "";
        int bankID = 0;
        string toBankName = "";
        string toName = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);

        try
        {
            saveBene = Convert.ToInt16(req.Msg);
            if (saveBene == 2)
            {
                resp = "Thank you for banking with Us.";
                return resp;
            }

            toBankAcct = prm["TONUBAN"];
            toName = prm["ToName"];
            toBankName = prm["BankName"];
            bankID = RunCount(prm["ToBankCode"]);
            if (bankID == 99)
            {
                //Default for sterling
                toBankCode = "99";
                toBankName = "STERLING";
            }
            else if(bankID == 98)//Gateway bank
            {
                toBankCode = "98";
                toBankName = "GATEWAY MORTGAGE";
            }
            else
            {
                toBankCode = prm["TOBANK"];
                //get bank short code
                toBankName = GetBankShortName(Convert.ToInt16(prm["TOBANK"]));
            }

            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_SaveBeneficiaryInfo");
            c.AddParam("@acct", toBankAcct.Trim());
            c.AddParam("@mob", req.Msisdn.Trim());
            c.AddParam("@ToName", toName.Trim());
            c.AddParam("@ToBankName", toBankName.Trim());
            c.AddParam("@ToBankCode", Convert.ToInt16(toBankCode.Trim()));
            c.ExecuteProc();

            int? cnt = c.returnValue;
            if (cnt > 0)
            {
                resp = "Your beneficiary has been saved. Thank you for banking with us.";
            }
            else
            {
                resp = "We were unable to save your beneficiary. Please try again later.%0AThank you.";
            }

        }
        catch (Exception ex)
        {
            resp = "We were unable to save your beneficiary. Please try again later.%0AThank you.";
        }

        return resp;
    }

}

