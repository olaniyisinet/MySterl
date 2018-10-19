using BVN;
using com.sbp.instantacct.entity;
using com.sbp.instantacct.service;
using CustomerAccount;
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
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Sterling
/// </summary>
public class SterlingEngine : BaseEngine
{
    EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
    public string DoSterlingAccountsByMobile(UReq req)
    {
        Gadget g = new Gadget();
        string resp = "";
        try
        {
            AccountLookupInfo inf = new AccountLookupInfo();
            inf.SessionId = req.SessionID;
            inf.ClientMobileNo = req.Msisdn;
            inf.RequestMessage = req.Msg;

            //resp = CustomerAccount_DAL.getAccounts(inf);

            BankTransfers bt = new BankTransfers();
            resp = bt.GetAcctByMobile(req);
            if (resp.Contains("Your NGN Accounts"))
            {
                inf.AccountNumber = g.GetAccountsByMobileNo2(req.Msisdn);
                //save the account to be debited
                if (inf.AccountNumber == "" || inf.AccountNumber == null)
                {

                }
                else
                {
                    g.Insert_Charges(req.SessionID, inf.AccountNumber, 3);
                }
            }
            //go to and get the account details
        }
        catch
        {
            resp = "ERROR: Could not contact core banking system";
        }
        return resp;
    }

    public string DoSterlingAccountBalance(UReq req)
    {
        Gadget g = new Gadget();
        //*822*6*NUBAN#
        string resp = ""; string mob = "";
        try
        {
            AccountLookupInfo inf = new AccountLookupInfo();
            inf.SessionId = req.SessionID;
            inf.ClientMobileNo = req.Msisdn;
            mob = req.Msisdn;
            inf.RequestMessage = req.Msg;

            char[] sep = { '*', '#' };
            string[] bits = req.Msg.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                inf.AccountNumber = bits[2];
            }
            catch
            {
                inf.AccountNumber = g.GetAccountsByMobileNo2(req.Msisdn);
            }
            BankTransfers bt = new BankTransfers();
            //resp = CustomerAccount_DAL.getAccountBalance(inf);
            resp = bt.GetBalanceByCustID(req);
            if (resp.Length > 0)
            {
                if (resp.Contains("There is no account") || resp.Contains("An error has occured"))
                {

                }
                else
                {
                    g.Insert_Charges(req.SessionID, inf.AccountNumber, 2);
                }
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = "ERROR: CAB00 %0AKindly contact Sterling Bank." +
                "%0ACALL US - (+234) 01-4484481-5";
            req.next_op = 1;
            return "1";
        }
        return resp;
    }

    public string DoGetBVN(UReq req)
    {
        //*822*3*BVN#
        string resp = "";
        try
        {
            char[] sep = { '*', '#' };
            string[] bits = req.Msg.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string bvn_val = bits[2];
            string mob = req.Msisdn;
            //resp = BVN_DAL.DoBank(bvn_val, mob);
            DoBankBVN bn = new DoBankBVN();
            resp = bn.DoBank(bvn_val, mob);
        }
        catch
        {
            //resp = ex.Message;
            resp = "ERROR: Request string not supported!%0AFor Sterling Bank customer dial *822*3*BVN#%0AFor NIB customer dial *822*3*BVN#";
        }
        return resp;
    }

    public int DoSaveBVNFromUSSDString(UReq req)
    {
        //*822*3*BVN#
        int resp = -1;
        try
        {
            char[] sep = { '*', '#' };
            string[] bits = req.Msg.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string bvn_val = bits[2];
            addParam("BVN", bvn_val, req);
            resp = 0;
        }
        catch
        {
        }
        return resp;
    }

    public string DoInstantAccount(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            var item = new USSDAccountRequest();
            item.AccountNumber = "";
            item.ApprovedBy = "";
            item.CustomerId = "";
            item.CreatedAt = DateTime.Now;
            item.DateApproved = new DateTime(1900, 1, 1);
            item.DateOpened = item.DateApproved;
            item.UpdatedAt = item.CreatedAt;
            item.ExistingAccountNumber = "";
            item.IsCustomer = false;
            try
            {
                item.BVN = prm["BVN"];
                if (prm["CUSTSTATUS"] == "2") //existing customer
                {
                    item.IsCustomer = true;
                    item.ExistingAccountNumber = prm["FROMACCT"];
                }
            }
            catch { }
            item.Mobile = req.Msisdn;
            item.SessionId = req.SessionID;
            item.Statusflag = 0;
            item.Slug = "";
            string[] bits = USSDAccountRequestService.Insert(item);
            if (bits[0] == "00")
            {
                resp = "Thank you for choosing Sterling Bank, your account request has been submitted successfully";
            }
            else if (bits[0] == "01")
            {
                resp = "Thank you for choosing Sterling Bank, your account request has already been treated. Your account number is " + bits[1];
            }
            else if (bits[0] == "02")
            {
                resp = "Thank you for choosing Sterling Bank, you are already a Sterling Bank Customer. Your customer number is " + bits[1];
            }
            else
            {
                resp = "Your account request could not be treated now, please try again later.";
            }
        }
        catch
        {
            resp = "Your account request could not be treated now, please try again later..";
        }
        return resp;
    }

    /// <summary>
    /// ////////////////////////////////////////////////  //////////////////////////////////////////////////////////////////////////
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
    public string SaveTraAmount(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("AMOUNT", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveBene(UReq req)
    {
        string resp = "0";
        try
        {
            try
            {
                long acctno = long.Parse(req.Msg);
            }
            catch
            {
                if (req.op == 91120)
                {
                    req.next_op = 91110;
                    return "0";
                }
                if (req.op == 92004)
                {
                    req.next_op = 92002;
                    return "0";
                }
            }

            if (req.Msg.Length != 10)
            {
                if (req.op == 91120)
                {
                    req.next_op = 91110;
                    return "0";
                }
                if (req.op == 92004)
                {
                    req.next_op = 92002;
                    return "0";
                }
            }
            addParam("TONUBAN", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
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
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
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

        //Check if beneficiary was selected.
        int beneFlag = 0; string beneDetails = "";
        beneFlag = RunCount(prm["B"]);
        if (beneFlag != 1)
        {
            ToNuban = prm["TONUBAN"];
        }
        else
        {
            ITEMSELECT = RunCount(prm["BENEITEMSELECT"]);
            beneDetails = g.GetBeneByListID(ITEMSELECT, req);
            char[] sep1 = { '*' };
            string[] beneBits = beneDetails.Split(sep1, StringSplitOptions.RemoveEmptyEntries);
            ToNuban = beneBits[0];
            addParam("TONUBAN", beneBits[0].Trim(), req);
        }

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
                ToName = "Unable to fetch beneficiary account name at this type.";
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
    public string SaveSummary(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            string nuban = prm["NUBAN"];
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_CheckCusRegPIN");
            c.AddParam("@mob", req.Msisdn);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    //activated = int.Parse(dr["Activated"].ToString());
                    string regisNuban = dr["nuban"].ToString().Trim();
                    if (nuban != regisNuban)
                    {
                        continue;
                    }
                    else
                    {
                        //Nuban matches one of customer's profile
                        nuban = regisNuban;
                        int activated = int.Parse(dr["Activated"].ToString());
                        if (activated == 0)
                        {
                            cnt = 101;
                            addParam("Bypass", cnt.ToString(), req);
                            req.next_op = 115;
                            return "0";
                        }
                        if (activated == 1)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                //customer is okay to continue
            }

            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 91150;
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SavePin(UReq req)
    {
        string resp = "0";
        //check if the PIN is correct
        string sql = "select custauthid from Go_Registered_Account where mobile = @mb";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mb", req.Msisdn);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            try
            {
                addParam("PIN", req.Msg, req);

            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            req.next_op = 113;
            req.Method = "0";
            return "0";
        }
        return resp;
    }
    //sterling to sterling trnx
    public string DoSubmitS2S(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = ""; string PIN2 = "";
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
            //    t24Mob = ConvertMobile234(t24Mob);
            //    if (Ledcodeval == "1200" || Ledcodeval == "1201" || Ledcodeval == "1202" || Ledcodeval == "1203")
            //    {
            //        return "Corporate customers are not allowed on this platform";
            //    }
            //    string themobnum = "";
            //    themobnum = req.Msisdn;
            //    //if (themobnum.StartsWith("234"))
            //    //{
            //    //    themobnum = themobnum.Replace("234", "0");
            //    //}

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

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,1,@sid,@frmNuban)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,1,@sid,@frmNuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
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
            //resp = "Transaction has been submitted for processing";
            resp = "Your transaction has been submitted.%0ADo you want to save the beneficiary?%0A1.Yes%0A2.No";

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

        string resp = ""; int summary = 0; string PIN = ""; string PIN2 = "";
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
            //    t24Mob = ConvertMobile234(t24Mob);
            //    if (Ledcodeval == "1200" || Ledcodeval == "1201" || Ledcodeval == "1202" || Ledcodeval == "1203")
            //    {
            //        return "Corporate customers are not allowed on this platform";
            //    }
            //    string themobnum = "";
            //    themobnum = req.Msisdn;
            //    //if (themobnum.StartsWith("234"))
            //    //{
            //    //    themobnum = themobnum.Replace("234", "0");
            //    //}

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

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,1,@sid,@frmNuban)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,1,@sid,@frmNuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
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

    public string DoInterBank(UReq req)
    {

        string resp = ""; string PIN = ""; string PIN2 = "";
        int k = 0;
        try
        {

            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            //prm["AMOUNT"]
            int summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*5*AMT*NUBAN# to try again";
                return resp;
            }

            PIN = prm["PIN"];

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }
            
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,bankcode,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,2,@sid,@bc,@frm)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,bankcode,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,2,@sid,@bc,@frm)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@bc", prm["TOBANK"]);
            cn.AddParam("@frm", prm["FROMACCT"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            return resp = "ERROR: Could not contact core banking system " + ex.ToString();
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

        string resp = ""; string PIN = ""; string PIN2 = "";
        int k = 0;
        try
        {

            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            //prm["AMOUNT"]
            int summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*5*AMT*NUBAN# to try again";
                return resp;
            }

            PIN = prm["PIN"];

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }
           
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,bankcode,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,2,@sid,@bc,@frm)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,bankcode,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,2,@sid,@bc,@frm)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@bc", prm["TOBANK"]);
            cn.AddParam("@frm", prm["FROMACCT"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            return resp = "ERROR: Could not contact core banking system " + ex.ToString();
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
    public string SaveSummaryInter(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            string nuban = prm["NUBAN"];
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_CheckCusRegPIN");
            c.AddParam("@mob", req.Msisdn);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    //activated = int.Parse(dr["Activated"].ToString());
                    string regisNuban = dr["nuban"].ToString().Trim();
                    if (nuban != regisNuban)
                    {
                        continue;
                    }
                    else
                    {
                        //Nuban matches one of customer's profile
                        nuban = regisNuban;
                        int activated = int.Parse(dr["Activated"].ToString());
                        if (activated == 0)
                        {
                            cnt = 101;
                            addParam("Bypass", cnt.ToString(), req);
                            req.next_op = 115;
                            return "0";
                        }
                    }
                }
            }
            else
            {
                //customer is okay to continue
            }

            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 14206;
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string DoNameEnquiry(string toAcct, int tobankcode, string sidval)
    {
        Gadget g = new Gadget(); string RandomNum = ""; string SessionID = ""; string resp = ""; string Dest_bank_code = "";
        string ToName = "";
        RandomNum = g.GenerateRndNumber(3);
        SessionID = g.newSessionGlobal(RandomNum, 1);
        Dest_bank_code = getBankcode(tobankcode.ToString());
        char[] sep = { '*' };
        string[] bits = Dest_bank_code.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        //first check our local table 
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_nip");
        c.SetProcedure("spd_Get_NameEnquiry_From_Table0504");
        c.AddParam("@nuban", toAcct);
        c.AddParam("@bankcode", bits[0]);
        //c.SetProcedure("spd_Get_NameEnquiry_From_Table");
        //c.AddParam("@nuban", toAcct);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            return "00:" + dr["AccountName"].ToString() + ":" + " " + ":" + bits[1];
        }
        else
        {
            ////call NIBSS based on the toAcct number
            NIPNameEnquiry.NewIBSSoapClient ws = new NIPNameEnquiry.NewIBSSoapClient();
            resp = ws.NameEnquiry(SessionID, bits[0], "3", toAcct);
            resp = resp.Replace("::", ":");
            resp = resp.Replace(":1", ":");
        }
        return resp + ":" + bits[1];
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
    public string DisplaySummaryInter(UReq req)
    {
        Gadget g = new Gadget();
        string resp = ""; decimal amt = 0; string frmNuban = ""; string theResp = "";
        string prms = getParams(req); NameValueCollection prm = splitParam(prms);
        string SessionID = req.SessionID; char[] sep = { ':' }; int flag = 0;
        int ITEMSELECT = 0; string ToNuban = "";

        //Check if beneficiary was selected.
        int beneFlag = 0; string beneDetails = ""; int bankCode = 0;
        beneFlag = RunCount(prm["B"]);
        if (beneFlag != 1)
        {
            ToNuban = prm["TONUBAN"];
            bankCode = int.Parse(prm["TOBANK"]);
        }
        else
        {
            ITEMSELECT = RunCount(prm["BENEITEMSELECT"]);
            beneDetails = g.GetBeneByListID(ITEMSELECT, req);
            char[] sep1 = { '*' };
            string[] beneBits = beneDetails.Split(sep1, StringSplitOptions.RemoveEmptyEntries);
            ToNuban = beneBits[0].Trim();
            bankCode = RunCount(beneBits[1]);
            addParam("TONUBAN", ToNuban, req);
            addParam("TOBANK", beneBits[1].Trim(), req);
        }

        //theResp = DoNameEnquiry(prm["TONUBAN"], int.Parse(prm["TOBANK"]), SessionID);
        theResp = DoNameEnquiry(ToNuban, bankCode, SessionID);

        amt = decimal.Parse(prm["AMOUNT"]);
        string[] bits = theResp.Split(sep, StringSplitOptions.RemoveEmptyEntries);
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
        //addParam("frmNuban", frmNuban, req);

        //frmNuban = getFromAcctBySid(SessionID);

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
            //resp = "An error occured kindly try again later.";
            //req.next_op = 14205;
            return "Error occured: " + resp;

        }
        return "Confirmation: " + resp;
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
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
            req.next_op = 91152;
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
            req.next_op = 91152;
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
                    //check is bene was selected 
                    int flag = RunCount(prm["BeneSelected"]);
                    if (flag == 1)
                    {
                        //Beneficiary was selected. No reply needed
                        req.next_op = 91152;
                        return "1";
                    }
                    else
                    {
                        //Beneficiary 
                        req.next_op = 91152;
                        return "0";
                    }
                    //req.next_op = 91152;
                    //return "0";
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
            req.next_op = 91152;
            return "9";
        }    
        
        return resp;
    }

    public string SavetxnPINBillPay(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 110007;
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
            req.next_op = 110007;
            return "0";
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
            req.next_op = 110007;
            return "0";
        }




        return resp;
    }

    public string SavetxnPINData(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = req.op - 1;
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
            req.next_op = req.op - 1;
            return "0";
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
            req.next_op = req.op - 1;
            return "0";
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
            req.next_op = 92010;
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
            req.next_op = 92010;
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
               
                decimal amt = Convert.ToDecimal(prm["AMOUNT"]);
                if (amt < 20000)
                {
                    //check is bene was selected 
                    int flag = RunCount(prm["BeneSelected"]);
                    if (flag == 1)
                    {
                        //Beneficiary was selected. No reply needed
                        req.next_op = 92011;
                        return "1";
                    }
                    else
                    {
                        //Beneficiary 
                        req.next_op = 92011;
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
            req.next_op = 92010;
            return "9";
        }

    
        

        return resp;
    }
    public string IsUserRegis_Bal(UReq req)
    {
        string resp = "0";
        try
        {
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = ""; int enrollType = 0;
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());
                enrollType = int.Parse(dr["EnrollType"].ToString());
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }

            if (activated == 1)
            {
                try
                {
                    char[] delm = { '*', '#' };
                    string msg = req.Msg.Trim();
                    string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                    if (nuban == s[3].Trim())
                    {
                        req.next_op = 60;
                        return "0";
                    }
                    else
                    {
                        req.next_op = 60;
                        return "0";
                    }
                }
                catch
                {
                    req.next_op = 60;
                    return "0";
                }
            }
            else if (activated == 0)
            {
                if (enrollType == 1)
                {
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

    public string IsUserRegis_BuyOthers(UReq req)
    {
        string resp = "0";
        try
        {
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                //DataRow dr = ds.Tables[0].Rows[0];               
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    activated = int.Parse(dr["Activated"].ToString());
                    //activated = int.Parse(dr["Activated"].ToString());
                    string regisNuban = dr["nuban"].ToString().Trim();
                    if (activated != 1)
                    {
                        continue;
                    }
                    else
                    {
                        //Nuban matches one of customer's profile
                        //cnt = 111;
                        //addParam("NUBAN", req.Msg.Trim(), req);
                        return "0";
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
                try
                {

                    if (msg == "*822*3#")
                    {
                        //do something here
                        req.next_op = 170;
                        return "0";
                    }
                    else
                    {
                        string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                        if (nuban == s[3].Trim())
                        {
                            req.next_op = 111;
                            return "1";
                        }
                        else
                        {

                            req.next_op = 110;
                            return "0";
                        }
                    }
                }
                catch
                {
                    string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                    //addParam("AMOUNT", s[1], req);
                    req.next_op = 110;
                    return "0";
                }
            }
            else if (activated == 0)
            {
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

    public string IsUserRegis_BuySef(UReq req)
    {
        string resp = "0";
        int cnt = 0;
        Gadget g = new Gadget();
        string frmNuban = "";
        try
        {
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = ""; int enrollType = 0;
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
                    //string regisNuban = dr["nuban"].ToString().Trim();
                    if (activated != 1)
                    {
                        continue;
                    }
                    else
                    {
                        //Nuban matches one of customer's profile
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
                try
                {
                    char[] delm = { '*', '#' };
                    string msg = req.Msg.Trim();
                    string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                    if (msg == "*822*2#" && s.Length != 2)
                    {
                        //do something here
                        req.next_op = 1;
                        return "2";
                    }
                    else
                    {
                        if (s.Length == 2)
                        {
                            //Check if customer balance is ok
                            int noOfAccts = g.GetNoofAccts(req.Msisdn.Trim());
                            if (noOfAccts > 1)
                            {
                                //check will happen at SaveAcctSelected1
                            }
                            else
                            {
                                frmNuban = getDefaultAccount(req.Msisdn.Trim());
                                decimal cusbal = 0; decimal amtval = 0;
                                amtval = decimal.Parse(s[1]);
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
                                                    addParam("NUBAN", frmNuban, req);
                                                    req.next_op = 124;
                                                    return "0";
                                                }
                                            }
                                        }

                                        if (trnxLimit == 0)
                                        {
                                            //600 for proper registered account
                                            //Minimum ,Maximum per trns, Max per day
                                            Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                                            getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                            if (airtimeTuple.Item2 < amtval)
                                            {
                                                //Reroute to amount field
                                                cnt = 104;
                                                int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                                                addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                                addParam("cnt", cnt.ToString(), req);
                                                req.next_op = 1;
                                                return "2";
                                            }
                                            if (airtimeTuple.Item3 < amtval + Totaldone)
                                            {
                                                //reroute to amount field
                                                cnt = 103;
                                                addParam("cnt", cnt.ToString(), req);
                                                req.next_op = 1;
                                                return "2";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cnt = 102;
                                        addParam("cnt", cnt.ToString(), req);
                                        req.next_op = 1;
                                        return "2";
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
                                                int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                                                addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                                addParam("cnt", cnt.ToString(), req);
                                                req.next_op = 1;
                                                return "2";

                                            }
                                            if (limitedTuple.Item2 < amtval + Totaldone)
                                            {
                                                //reroute to amount field
                                                cnt = 103;
                                                addParam("cnt", cnt.ToString(), req);
                                                req.next_op = 1;
                                                return "2";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cnt = 102;
                                        addParam("cnt", cnt.ToString(), req);
                                        req.next_op = 1;
                                        return "2";
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
                                            enrollType = int.Parse(dr1["EnrollType"].ToString());
                                            activated = int.Parse(dr1["Activated"].ToString());
                                            if (enrollType == 1 && activated == 0)
                                            {
                                                //700 for auto enrolled account
                                                //Minimum ,Maximum per trns, Max per day
                                                Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(700);
                                                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                                if (airtimeTuple.Item3 < amtval + Totaldone)
                                                {
                                                    CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                                    string response = cardws.GetActiveCards(frmNuban);
                                                    if (response.StartsWith("00"))
                                                    {
                                                        addParam("NUBAN", frmNuban, req);
                                                        cnt = 102;
                                                        addParam("Bypass", cnt.ToString(), req);
                                                        addParam("cnt", cnt.ToString(), req);
                                                        req.next_op = 126;
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
                                                //600 for proper registered account
                                                //Minimum ,Maximum per trns, Max per day
                                                Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                                                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                                if (airtimeTuple.Item2 < amtval)
                                                {
                                                    //Reroute to amount field
                                                    cnt = 104;
                                                    int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                                                    addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                                    addParam("cnt", cnt.ToString(), req);
                                                    req.next_op = 1;
                                                    return "2";
                                                }
                                                if (airtimeTuple.Item3 < amtval + Totaldone)
                                                {
                                                    //reroute to amount field
                                                    cnt = 103;
                                                    addParam("cnt", cnt.ToString(), req);
                                                    req.next_op = 1;
                                                    return "2";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            cnt = 102;
                                            //removeParam("Amt", req);
                                            addParam("cnt", cnt.ToString(), req);
                                            req.next_op = 1;
                                            return "2";
                                        }
                                    }
                                    else
                                    {
                                        req.next_op = 4190;
                                        return "0";
                                    }
                                }

                            }
                            addParam("Amt", s[1], req);
                            return "0";
                        }
                        else
                        {
                            if (nuban == s[3].Trim())
                            {
                                // req.next_op = 110;
                                req.next_op = 111;
                                return "1";
                            }
                            else
                            {
                                //req.next_op = 110;
                                return "0";
                            }
                        }
                    }
                }
                catch
                {
                    //req.next_op = 110;
                    return "0";
                }
            }
            else if (activated == 0)
            {
                if (enrollType == 1)
                {
                    char[] delm = { '*', '#' };
                    string msg = req.Msg.Trim();
                    string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        //Check if customer balance is ok
                        int noOfAccts = g.GetNoofAccts(req.Msisdn.Trim());
                        if (noOfAccts > 1)
                        {
                            //check will happen at SaveAcctSelected1
                        }
                        else
                        {
                            frmNuban = getDefaultAccount(req.Msisdn.Trim());
                            decimal cusbal = 0; decimal amtval = 0;
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
                                amtval = decimal.Parse(s[1]);
                                if (cusbal > amtval)
                                {
                                    //proceed
                                    var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                                    DataRow dr1 = ds1.Tables[0].Rows[0];
                                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                                    enrollType = int.Parse(dr1["EnrollType"].ToString());
                                    activated = int.Parse(dr1["Activated"].ToString());
                                    if (enrollType == 1 && activated == 0)
                                    {
                                        //700 for auto enrolled account
                                        //Minimum ,Maximum per trns, Max per day
                                        Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(700);
                                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                        if (airtimeTuple.Item3 < amtval + Totaldone)
                                        {
                                            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                            string response = cardws.GetActiveCards(frmNuban);
                                            if (response.StartsWith("00"))
                                            {
                                                addParam("NUBAN", frmNuban, req);
                                                cnt = 102;
                                                addParam("Bypass", cnt.ToString(), req);
                                                addParam("cnt", cnt.ToString(), req);
                                                req.next_op = 126;
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
                                        //600 for proper registered account
                                        //Minimum ,Maximum per trns, Max per day
                                        Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                        if (airtimeTuple.Item2 < amtval)
                                        {
                                            //Reroute to amount field
                                            cnt = 104;
                                            int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                            addParam("cnt", cnt.ToString(), req);
                                            req.next_op = 1;
                                            return "2";
                                        }
                                        if (airtimeTuple.Item3 < amtval + Totaldone)
                                        {
                                            //reroute to amount field
                                            cnt = 103;
                                            addParam("cnt", cnt.ToString(), req);
                                            req.next_op = 1;
                                            return "2";
                                        }
                                    }
                                }
                                else
                                {
                                    cnt = 102;
                                    //removeParam("Amt", req);
                                    addParam("cnt", cnt.ToString(), req);
                                    req.next_op = 1;
                                    return "2";
                                }
                            }
                            else
                            {
                                req.next_op = 4190;
                                return "0";
                            }
                        }
                        addParam("Amt", s[1], req);
                        return "0";
                    }
                    //return "0";
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

    public string CollectMobile(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter the beneficiary Mobile number";
            return "Collect Beneficiary Mobile:" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 11 digits";
            return "Collect Beneficiary Mobile:" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 11 digits and not alphanumeric";
            return "Collect Beneficiary Mobile:" + resp;
        }
        resp = "Kindly enter the beneficiary Mobile number";
        return "Collect Beneficiary Mobile:" + resp;
    }
    public string SaveMobile(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 11 || req.Msg.Length > 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 171;
            return "9";
        }
        //check if entry is numeric
        try
        {
            long nuban = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 171;
            return "9";
        }
        //check if the account number exist
        string sql = ""; string resp = "0"; string nubanRs = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            try
            {
                addParam("MOBILE", req.Msg, req);
            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            //redirect to account not found
            cnt = -1;
            addParam("status", cnt.ToString(), req);
            req.next_op = 7003;
            return "3";
        }


        return resp;
    }
    public string CollectAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter airtime amount";
            return "Airtime amount:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 100 naira";
            return "Airtime amount:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Airtime amount:%0A" + resp;
        }
        resp = "Enter airtime amount";
        return "Airtime amount:%0A" + resp;
    }

    //airtime self
    public string SaveTheAMt(UReq req)
    {
        int cnt = 0; string resp = "";
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 173;
            return "9";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) <= 100)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 173;
            return "9";
        }
        //save it
        try
        {
            addParam("AMOUNT", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    //****************************************************** new one back
    public string BackToMainMenu(UReq req)
    {
        req.next_op = 0;
        return "1";
    }

    public string GoToPage3(UReq req)
    {
        req.next_op = 200000;
        return "0";
    }

    public string GoToPINReset(UReq req)
    {
        req.next_op = 1;
        return "91";
    }
    public string GetAllActiveBillers(UReq req)
    {
        Gadget g = new Gadget();
        int Tid = int.Parse(req.Msg); int TransRate = 0;
        TransRate = Tid;

        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int lastCnt = 0;
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_GetActiveBillersISW");
            c.AddParam("@TransRate", TransRate);
            DataSet ds = c.Select("rec");
            var tb = ds.Tables[0];
            var query = (from d in tb.AsEnumerable()
                         select new
                         {
                             shortname = d.Field<string>("shortname"),
                             TransRate = d.Field<int>("TransRate")
                         });
            foreach (var r in query)
            {
                lastCnt += 1;
                if (lastCnt == 1)
                {
                    resp = r.TransRate + " " + r.shortname;
                }
                else
                {
                    resp += "%0A" + r.TransRate + " " + r.shortname;
                }
            }
            addParam("lastCnt", lastCnt.ToString(), req);
            return resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Item selected must be within the list displayed";
            return "Re-enter:" + resp;
        }
        return "";
    }

    public string SaveBillerSelected(UReq req)
    {
        string resp = "0"; int msgIntval = 0;
        int cnt = 0; string prms = getParams(req); NameValueCollection prm = splitParam(prms);
        int lastCnt = int.Parse(prm["lastCnt"]);
        msgIntval = int.Parse(req.Msg);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (msgIntval > lastCnt)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 110000;
            return "9";
        }
        //proceed if all is ok
        try
        {
            addParam("Tid", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string DisplayBillerItem(UReq req)
    {
        Gadget g = new Gadget(); int Billerid = 0;
        decimal amt = 0; decimal fee = 0; int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int Tid = 0;
            //go and get the billderid mapped to the Transrate Referenceid
            if (req.next_op == 110001 || req.next_op == 110002)
            {
                removeParam("Tid", req);
                Tid = 1;//DSTV
                addParam("Tid", Tid.ToString(), req);
            }
            else if (req.next_op == 130001 || req.next_op == 130002)
            {
                removeParam("Tid", req);
                Tid = 5;//Gotv
                addParam("Tid", Tid.ToString(), req);
            }
            Billerid = g.GetBillerIDbyTransRate(Tid);

            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            List<BillerItem> lb = g.GetBillersByPage(page.ToString(), Billerid);
            if (lb.Count <= 0)
            {
                removeParam("NXTPAG", req);
                lb = g.GetBillersByPage("0", Billerid);
            }
            //resp = "Select Biller Item";
            foreach (BillerItem item in lb)
            {
                cntfirst += 1;
                amt = decimal.Parse(item.Amount) / 100; fee = decimal.Parse(item.ItemFee) / 100;
                if (cntfirst == 1)
                {
                    //resp = item.TransRate + " " + item.ShortName + ":" + amt.ToString() + " F:" + fee;
                    resp = item.TransRate + " " + item.ShortName + ":" + amt.ToString();
                }
                else
                {
                    //resp += "%0A" + item.TransRate + " " + item.ShortName + ":" + amt.ToString() + " F:" + fee;
                    resp += "%0A" + item.TransRate + " " + item.ShortName + ":" + amt.ToString();
                }
            }
            resp += "%0A99 Next";
        }
        catch
        {
        }
        return resp;
    }
    public int SaveItemSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        if (req.Msg == "99")
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }
        addParam("ITEMSELECT", req.Msg, req);
        return 1;
    }

    public string isUserRegActive(UReq req)
    {
        //check if you are registered and have pin set
        try
        {
            addParam("Tid", req.Msg, req);
            string sql = ""; int activated = -1; string nuban = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());

                if (activated == 0)
                {
                    //redirect to set PIN
                    req.next_op = 115;
                    return "0";
                }
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }
        }
        catch
        {

        }
        return "0";
    }

    public string DisplayDataItem(UReq req)
    {
        Gadget g = new Gadget(); int Billerid = 0;
        decimal amt = 0; decimal fee = 0; int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            //go and get the billderid mapped to the Transrate Referenceid
            int Tid = 0;
            if (req.next_op == 121002 || req.next_op == 121000)
            {
                removeParam("Tid", req);
                Tid = 4;//mtn data
                addParam("Tid", Tid.ToString(), req);
            }
            else if (req.next_op == 122000 || req.next_op == 122002)
            {
                removeParam("Tid", req);
                Tid = 3;//etisalat data
                addParam("Tid", Tid.ToString(), req);
            }
            else if (req.next_op == 123002 || req.next_op == 123000)
            {
                removeParam("Tid", req);
                Tid = 2;//airtel data
                addParam("Tid", Tid.ToString(), req);
            }

            Billerid = g.GetBillerIDbyTransRate(Tid);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            List<BillerItem> lb = g.GetBillersByPage(page.ToString(), Billerid);
            if (lb.Count <= 0)
            {
                removeParam("NXTPAG", req);
                lb = g.GetBillersByPage("0", Billerid);
            }
            //resp = "Select Biller Item";
            foreach (BillerItem item in lb)
            {
                cntfirst += 1;
                amt = decimal.Parse(item.Amount) / 100; fee = decimal.Parse(item.ItemFee) / 100;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.ShortName + ":" + amt.ToString() + " F:" + fee;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.ShortName + ":" + amt.ToString() + " F:" + fee;
                }
            }
            resp += "%0A99 Next";
        }
        catch
        {
        }
        return resp;
    }

    public int SaveDataItemSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        if (req.Msg == "99")
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }
        addParam("ITEMSELECT", req.Msg, req);
        return 1;
    }

    public string SaveSmartCardNum(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("CustDetails", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveMobNum(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("CustDetails", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string DisplaySummaryDetails(UReq req)
    {
        Gadget g = new Gadget(); int Billerid = 0; string shortname = ""; string ItemName = ""; string BillerName = "";
        string resp = ""; decimal amt = 0; decimal fee = 0; string CustDetails = ""; string ITEMSELECT = ""; int Tid = 0;
        string prms = getParams(req); NameValueCollection prm = splitParam(prms);
        string SessionID = req.SessionID;
        CustDetails = prm["CustDetails"];
        //first item selected
        Tid = int.Parse(prm["Tid"]);
        //second item selected
        ITEMSELECT = prm["ITEMSELECT"];
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_GetPaymentDetailsISW");
        c.AddParam("@Tid", Tid);
        c.AddParam("@ItemSelected", int.Parse(ITEMSELECT));
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            BillerName = dr["BillerName"].ToString();
            shortname = dr["shortname"].ToString();
            ItemName = dr["ItemName"].ToString();
            amt = decimal.Parse(dr["amt"].ToString());
            fee = decimal.Parse(dr["fee"].ToString());
            //resp = "Payment for " + BillerName + " amt:" + amt.ToString() + " fee:" + fee.ToString() +  "%0A1.Yes%0A2.No";
            resp = "Payment for " + BillerName + " amt:" + amt.ToString() + "%0A1.Yes%0A2.No";
        }
        return "Confirmation: %0A" + resp;
    }
    public string SaveSummaryDetails(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string DoSubmitBillDataPmt(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = ""; int Tid = 0;
        string nuban = ""; string CustDetails = ""; string ITEMSELECT = "";
        string prms = getParams(req); int? k = 0;
        NameValueCollection prm = splitParam(prms);
        try
        {
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822# to try again";
                return resp;
            }
            ITEMSELECT = prm["ITEMSELECT"];
            Tid = int.Parse(prm["Tid"]);
            PIN = prm["PIN"];
            CustDetails = prm["CustDetails"];

            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getInsertBillsDataPmtISW");
            c.AddParam("@Tid", Tid);
            c.AddParam("@ItemSelected", ITEMSELECT);
            c.AddParam("@sessionid", req.SessionID);
            c.AddParam("@mobile", req.Msisdn);
            c.AddParam("@custauthid", PIN);
            c.AddParam("@customerId", CustDetails);
            c.AddParam("@trans_type", 1);//bill payment
            c.ExecuteProc();
            k = c.returnValue;
        }
        catch (Exception ex)
        {
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
    public string DisplaySummaryDataItem(UReq req)
    {
        Gadget g = new Gadget(); int Billerid = 0; string shortname = ""; string ItemName = ""; string BillerName = "";
        string resp = ""; decimal amt = 0; decimal fee = 0; string CustDetails = ""; string ITEMSELECT = ""; int Tid = 0;
        string prms = getParams(req); NameValueCollection prm = splitParam(prms);
        string SessionID = req.SessionID;
        CustDetails = prm["CustDetails"];
        //first item selected
        Tid = int.Parse(prm["Tid"]);
        //second item selected
        ITEMSELECT = prm["ITEMSELECT"];
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_GetPaymentDetailsISW");
        c.AddParam("@Tid", Tid);
        c.AddParam("@ItemSelected", int.Parse(ITEMSELECT));
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            BillerName = dr["BillerName"].ToString();
            shortname = dr["shortname"].ToString();
            ItemName = dr["ItemName"].ToString();
            amt = decimal.Parse(dr["amt"].ToString());
            fee = decimal.Parse(dr["fee"].ToString());
            resp = "Payment for " + BillerName + " amt:" + amt.ToString() + " fee:" + fee.ToString() + "%0A1.Yes%0A2.No";
        }
        return "Confirmation: %0A" + resp;
    }
    //**************************************** end ******************************************************************


    //******************************* for customer to submit token when their phone number is changed ****************//
    public string DoSubmitOTP(UReq req)
    {
        string resp = "";
        try
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_ApproveChangeonCoreByCustomer");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@otp", req.Msg.Trim());
            c.ExecuteProc();
            int? cnt = c.returnValue;
            if (cnt == 1)
            {
                //call EACBS that Kayode will conclude so you can proceeed and get response to respond to the users
                resp = "Your mobile number has been successfully changed on the core banking system. Thank you for banking with us";
            }
            else
            {
                resp = "The mobile number used to send the code is not known in our system";
            }
        }
        catch
        {
            resp = "An exception occured.  Kindly resend the code again";
        }
        return resp;
    }
    //******************************   end  *************************************************************************//

    public string CollecttxnAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int amtLimit = RunCount(prm["MaxAmtPerTrans"]);

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Transfer Amount:";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 0 naira";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            var firstName = "";
            try
            {
                firstName = GetFirstNameByMobileNo(req.Msisdn.Trim());
                resp = "Dear " + firstName + ", you do not have sufficient balance for this transaction. Please credit your account and try again. Thank you.";
                return resp;
            }
            catch
            {
                resp = "You currently do not have sufficient balance for this transaction.";
            }

            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry you have exceeded your transfer limit for today.";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            if (amtLimit == 0)
            {
                resp = "The amount entered is more than maximum amount allowed per transaction";
            }
            else
            {
                resp = "Please enter an amount lower than N" + amtLimit.ToString("#,##");
            }
            return "Transfer amount:%0A" + resp;
        }

        resp = "Enter Transfer amount";
        return "Transfer amount:%0A" + resp;
    }

    public string SaveThetxnAMt(UReq req)
    {
        int cnt = 0; string resp = "";
        string frmNuban = ""; Gadget g = new Gadget();
        int activated = -1;
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 91100;
            return "9";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) <= 0)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 91100;
            return "9";
        }

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
            amtval = decimal.Parse(req.Msg.Trim());

            //if amount is over 20k check if customer has set pin 2
            if (amtval > 20000)
            {
                var isPin2Set = checkForPin2(frmNuban, req.Msisdn, req);
                if (!isPin2Set)
                {
                    addParam("NUBAN", frmNuban, req);

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
                            req.next_op = 91100;
                            return "9";

                        }
                        if (limitedTuple.Item2 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 91100;
                            return "9";
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    //req.next_op = 92006;
                    //return "9";
                    req.next_op = 91100;
                    return "9";
                }
            }
            else if (frmNuban.StartsWith("11"))
            {
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
                if (bank1.availableBal < amtval)
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 91100;
                    return "9";
                }
                else
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                    if (trnxLimit == 0)
                    {
                        //600 for proper registered account
                        //Minimum ,Maximum per trns, Max per day
                        Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                        if (airtimeTuple.Item2 < amtval)
                        {
                            //Reroute to amount field
                            cnt = 104;
                            int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 91100;
                            return "9";
                        }
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 91100;
                            return "9";
                        }
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
                                req.next_op = 91100;
                                return "9";

                            }
                            if (limitedTuple.Item2 < amtval + Totaldone)
                            {
                                //reroute to amount field
                                cnt = 103;
                                //removeParam("AMOUNT", req);
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 91100;
                                return "9";

                            }
                        }
                    }
                    else
                    {
                        cnt = 102;
                        //removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 91100;
                        return "9";
                    }
                }
                else
                {
                    req.next_op = 4190;
                    return "0";
                }
            }
        }

        //save it
        try
        {
            addParam("AMOUNT", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string CollecttxnAmtInter(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int amtLimit = RunCount(prm["MaxAmtPerTrans"]);

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Transfer Amount:";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 0 naira";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            var firstName = "";
            try
            {
                firstName = GetFirstNameByMobileNo(req.Msisdn.Trim());
                resp = "Dear " + firstName + ", you do not have sufficient balance for this transaction. Please credit your account and try again. Thank you.";
                return resp;
            }
            catch
            {
                resp = "You currently do not have sufficient balance for this transaction.";
            }
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry you have exceeded your transfer limit for today.";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            if (amtLimit == 0)
            {
                resp = "The amount entered is more than maximum amount allowed per transaction";
            }
            else
            {
                resp = "Please enter an amount lower than N" + amtLimit.ToString("#,##");
            }
            return "Transfer amount:%0A" + resp;
        }
        resp = "Enter Transfer amount";
        return "Transfer amount:%0A" + resp;
    }

    public string SaveThetxnAMtInter(UReq req)
    {
        int cnt = 0; string resp = "0";
        string frmNuban = ""; Gadget g = new Gadget();
        int activated = -1;
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 92006;
            return "9";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) <= 0)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 92006;
            return "9";
        }


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
            amtval = decimal.Parse(req.Msg.Trim());

            //if amount is over 20k check if customer has set pin 2
            if (amtval > 20000)
            {
                var isPin2Set = checkForPin2(frmNuban, req.Msisdn, req);
                if (!isPin2Set)
                {
                    addParam("NUBAN", frmNuban, req);

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
                            req.next_op = 92006;
                            return "9";

                        }
                        if (limitedTuple.Item2 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 92006;
                            return "9";
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 92006;
                    return "9";
                }
            }
            else if (frmNuban.StartsWith("11"))
            {
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
                if (bank1.availableBal < amtval)
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 92006;
                    return "9";
                }
                else
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                    if (trnxLimit == 0)
                    {
                        //600 for proper registered account
                        //Minimum ,Maximum per trns, Max per day
                        Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                        if (airtimeTuple.Item2 < amtval)
                        {
                            //Reroute to amount field
                            cnt = 104;
                            int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 92006;
                            return "9";
                        }
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 92006;
                            return "9";
                        }
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
                                req.next_op = 92006;
                                return "9";

                            }
                            if (limitedTuple.Item2 < amtval + Totaldone)
                            {
                                //reroute to amount field
                                cnt = 103;
                                //removeParam("AMOUNT", req);
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 92006;
                                return "9";

                            }
                        }
                    }
                    else
                    {
                        cnt = 102;
                        //removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 92006;
                        return "9";
                    }
                }
                else
                {
                    req.next_op = 4190;
                    return "0";
                }
            }
        }


        //save it
        try
        {
            addParam("AMOUNT", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    //4th Aug 2017************************************************************************************

    //Sterling to Sterling transfer.... From Menu
    public string CheckNoofAccts(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            addParam("A", "1", req);
            req.next_op = 91108;//Redirect to choose how beneficiary is selected
            return "0";
        }
        else
        {
            req.next_op = 95000;
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
                    req.next_op = 96000;
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
                req.next_op = 96000;
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
                req.next_op = 96000;
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
            // string prms = getParams(req);
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
                // string prms = getParams(req);
                // NameValueCollection prm = splitParam(prms);
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
            // NameValueCollection prm = splitParam(prms);
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
            if (bank1.availableBal < amtval)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 91100;
                return 9;
            }
            else
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                DataRow dr1 = ds1.Tables[0].Rows[0];
                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                if (trnxLimit == 0)
                {
                    //600 for proper registered account
                    //Minimum ,Maximum per trns, Max per day
                    Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                    getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                    if (airtimeTuple.Item2 < amtval)
                    {
                        //Reroute to amount field
                        cnt = 104;
                        int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                        removeParam("Amt", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 91100;
                        return 9;
                    }
                    if (airtimeTuple.Item3 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("Amt", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 91100;
                        return 9;
                    }
                }
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
                            decimal maxAmtPerTrans = limitedTuple.Item1;
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

    public string CollectBeneMode(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "1.Input Account No%0A2.Select from saved beneficiary";
            return "Enter Beneficiary:%0A" + resp;
        }
        return resp;
    }
    public string SaveBeneMethod(UReq req)
    {
        string resp = "0";
        try
        {
            if (req.Msg == "1")
            {
                req.next_op = 91110;
            }
            else
            {
                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    //continue: no issues
                    addParam("BeneSelected", "1", req);
                }
                else
                {
                    //redirect to beneficiary selection mode page
                    req.next_op = 91108;
                    resp = "0";
                }
            }
        }
        catch
        {

        }

        return resp;
    }
    public string CollectBeneAcct(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Beneficiary Account:";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Unable to fetch data with name.%0APlease enter beneficiary account:";
            return resp;
        }
        return resp;
    }
    public string CollectBeneName(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Search by Beneficiary Name:";
        }
        else if (cnt == 101)
        {
            resp = "Search by Beneficiary Name:%0APlease ensure your enter only letters.";
        }
        return resp;
    }
    public string FetchBeneIntodb(UReq req)
    {
        string beneName = "";
        var regexItem = new Regex("^[a-zA-Z ]*$");
        if (regexItem.IsMatch(req.Msg) && !string.IsNullOrEmpty(req.Msg))
        {
            //No numbers. Safe to continue
            beneName = req.Msg.Trim();
        }
        else
        {
            //redirect
            addParam("cnt", "101", req);
            req.next_op = 91111;
            return "0";
        }

        //Get Beneficiary list
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetBeneList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        c.AddParam("@beneName", beneName);
        c.AddParam("@bankID", 99);//99 for intra customers
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 91113;
            return "0";
        }
        else
        {
            //redirect
            removeParam("BeneSelected", req);
            addParam("cnt", "101", req);
            req.next_op = 91110;
            return "0";
        }


        return "0";
    }
    public string ListBeneInfo(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            List<BeneList> lb = g.GetBeneListByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("BENENXTPAG", req);
                lb = g.GetBeneListByPage("0", req.SessionID);
            }

            foreach (BeneList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
                else
                {
                    int acctListNo = Convert.ToInt16(item.TransRate);
                    resp += "%0A" + acctListNo + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
            }

            if (cntfirst < 2)
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
        return "Select Beneficiary%0A" + resp;
    }
    public int SaveBeneSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
                page++;
                removeParam("BENENXTPAG", req);
                addParam("BENENXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }

        addParam("BENEITEMSELECT", req.Msg, req);
        addParam("B", "1", req);
        return 1;
    }


    //interbank transfer from menu
    public string CheckNoofAccts1(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            //req.next_op = 92007;
            req.next_op = 93001;
            return "0";
        }
        else
        {
            req.next_op = 801;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb1(UReq req)
    {
        try
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
                        req.next_op = 802;
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
                    req.next_op = 802;
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
                    req.next_op = 802;
                    return "0";
                }
                else
                {
                    return "0";
                }
            }
        }
        catch (Exception ex)
        {
            return "0";

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
        catch (Exception ex)
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
            if (bank1.availableBal < amtval)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 92006;
                return 9;
            }
            else
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                DataRow dr1 = ds1.Tables[0].Rows[0];
                int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                if (trnxLimit == 0)
                {
                    //600 for proper registered account
                    //Minimum ,Maximum per trns, Max per day
                    Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                    getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                    if (airtimeTuple.Item2 < amtval)
                    {
                        //Reroute to amount field
                        cnt = 104;
                        int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                        removeParam("Amt", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 92006;
                        return 9;
                    }
                    if (airtimeTuple.Item3 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("Amt", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 92006;
                        return 9;
                    }
                }
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

    public string CollectBeneMode1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "1.Input Account Number%0A2.Select from saved beneficiary";
            return "Enter Beneficiary:%0A" + resp;
        }
        return resp;
    }
    public string SaveBeneMethod1(UReq req)
    {
        string resp = "0";
        try
        {
            if (req.Msg == "1")
            {
                req.next_op = 92002;
            }
            else
            {
                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    //continue: no issues
                    addParam("BeneSelected", "1", req);

                }
                else
                {
                    //redirect to beneficiary selection mode page
                    req.next_op = 93001;
                    resp = "0";
                }
            }
        }
        catch
        {

        }

        return resp;
    }
    public string CollectBeneAcct1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Beneficiary Account:";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Unable to fetch data with name.%0APlease enter beneficiary account:";
            return resp;
        }
        return resp;
    }
    public string CollectBeneName1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Search by Beneficiary Name:";
        }
        else if (cnt == 101)
        {
            resp = "Search by Beneficiary Name:%0APlease ensure your enter only letters.";
        }
        return resp;
    }
    public string FetchBeneIntodb1(UReq req)
    {
        string beneName = "";
        var regexItem = new Regex("^[a-zA-Z ]*$");
        if (regexItem.IsMatch(req.Msg) && !string.IsNullOrEmpty(req.Msg))
        {
            //No numbers. Safe to continue
            beneName = req.Msg.Trim();
        }
        else
        {
            //redirect
            addParam("cnt", "101", req);
            req.next_op = 93003;
            return "0";
        }

        //Get Beneficiary list
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetBeneList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        c.AddParam("@beneName", beneName);
        c.AddParam("@bankID", 0);//0 for inter bank
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 93005;
            return "0";
        }
        else
        {
            //redirect
            removeParam("BeneSelected", req);
            addParam("cnt", "101", req);
            req.next_op = 92002;
            return "0";
        }


        return "0";
    }
    public string ListBeneInfo1(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            List<BeneList> lb = g.GetBeneListByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("BENENXTPAG", req);
                lb = g.GetBeneListByPage("0", req.SessionID);
            }

            foreach (BeneList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
                else
                {
                    int acctListNo = Convert.ToInt16(item.TransRate);
                    resp += "%0A" + acctListNo + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
            }

            if (cntfirst < 2)
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
        return "Select Beneficiary%0A" + resp;
    }
    public int SaveBeneSelected1(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
                page++;
                removeParam("BENENXTPAG", req);
                addParam("BENENXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }

        addParam("BENEITEMSELECT", req.Msg, req);
        addParam("B", "1", req);
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
            return "Authentication:%0A" + resp;
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
            req.next_op = 91153;
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
            req.next_op = 91153;
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
                //check is bene was selected 
                int flag = RunCount(prm["BeneSelected"]);
                if (flag == 1)
                {
                    //Beneficiary was selected. No reply needed
                    req.next_op = 91152;
                    return "1";
                }
                else
                {
                    //Beneficiary 
                    req.next_op = 91152;
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
            req.next_op = 91153;
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
            req.next_op = 92012;
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
            req.next_op = 92012;
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
                //check is bene was selected 
                int flag = RunCount(prm["BeneSelected"]);
                if (flag == 1)
                {
                    //Beneficiary was selected. No reply needed
                    req.next_op = 92011;
                    return "1";
                }
                else
                {
                    //Beneficiary 
                    req.next_op = 92011;
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
            req.next_op = 92012;
            return "0";
        }

        return resp;
    }

    public string DirectToPinToReset(UReq req)
    {
        try
        {
            long pinSelected = long.Parse(req.Msg);
        }
        catch
        {
            req.next_op = 100001;
            return "3";
        }

        if (RunCount(req.Msg) > 2)
        {
            req.next_op = 100001;
            return "3";
        }

        if (RunCount(req.Msg) == 1)//Redirect to PIN 1
        {
            req.next_op = 1;
            return "91";
        }

        if (RunCount(req.Msg) == 2)//Redirect to PIN 2
        {
            req.next_op = 7020;
            return "0";
        }

        req.next_op = 1;
        return "91";
    }

    //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //We notice your transaction was not concluded. Do you want to continue your previous session?%0A1 Yes%0A2 No //GetPreviousState
    public string CheckPreviousState(UReq req)
    {
        string resp = "0";
        string oldSess = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            if(req.Msg != "1")//redirect to display menu
            {
                req.next_op = 0;
                resp = "1";
            }
            else
            {
                oldSess = prm["PrevSess"];
                //get the old op_id and sub_op_id using old session id
                //This will redirect and show old page before disconnection.
                Tuple<int, int> lastOperationInfo = GetLastOperation(oldSess,req.Msisdn);
                req.next_op = lastOperationInfo.Item1;
                resp = lastOperationInfo.Item2.ToString();
            }

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
}

