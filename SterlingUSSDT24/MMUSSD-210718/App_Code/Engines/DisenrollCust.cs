using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for DisenrollCust
/// </summary>
public class DisenrollCust : BaseEngine
{
    public string SaveRequest(UReq req)
    {
        //*822*20*0005969437#
        string resp = "0"; string nuban = ""; string ledger_code = ""; int activated = -1; int cnt = 0;
        try
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            //if (s[0] == "1" && s.Length == 1)
            //{
            //    //redirect to enter nuban
            //    req.next_op = 122;
            //    return "0";
            //}
            try
            {
                nuban = s[2];
            }
            catch
            {
                req.next_op = 2002;
                return "0";
            }
            if (nuban.Length != 10)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                return "0";
            }

            try
            {
                long acctno = long.Parse(nuban);
            }
            catch
            {
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                return "0";
            }

            //Check if BankOne Customer
            if (nuban.StartsWith("11"))
            {
                //Treat as BankOne
                BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
                //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
                BankOneClass bank1 = new BankOneClass();
                bank1.accountNumber = nuban;
                string bankOneReq = bank1.createRequestForNameEnq();
                string encrptdReq = EncryptTripleDES(bankOneReq);
                string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
                bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
                if (bank1.status != "True")
                {
                    cnt = 104;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }
                if (bank1.phoneNumber != req.Msisdn.Replace("234", "0"))
                {
                    cnt = 105;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect();
                cc.SetProcedure("spd_getRegisteredUserProfile");
                cc.AddParam("@mobile", req.Msisdn.Trim());
                DataSet bankOne = cc.Select("rec");
                if (bankOne.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = bankOne.Tables[0].Rows[0];
                    activated = int.Parse(dr["Activated"].ToString());
                    nuban = dr["nuban"].ToString();
                    if (activated == 1)
                    {
                        req.next_op = 4040;
                        return "0";
                    }
                    else
                    {
                        //Continue
                        addParam("NUBAN", nuban, req);
                        req.next_op = 124;
                        return "0";
                    }
                }
            }

            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds1 = c.Select("rec");
            if (ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds1.Tables[0].Rows[i];
                    //activated = int.Parse(dr["Activated"].ToString());
                    string regisNuban = dr["nuban"].ToString().Trim();
                    if (nuban != regisNuban)
                    {
                        cnt = -1;
                        continue;
                    }
                    else
                    {
                        //Nuban matches one of customer's profile
                        cnt = 111;
                        addParam("NUBAN", nuban, req);
                        addParam("cnt", cnt.ToString(), req);
                        return "0";
                    }

                }
                if (cnt == -1)
                {
                    cnt = -1;
                    addParam("cnt", cnt.ToString(), req);
                    return "0";
                }
            }
            else
            {
                cnt = 99;
                addParam("cnt", cnt.ToString(), req);
                return "0";
            }

            DataSet ds = new DataSet();
            ds = ws.getAccountFullInfo(nuban);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ledger_code = dr["T24_LED_CODE"].ToString();
                //Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                c.SetProcedure("spd_getRestrictedLedgCode");
                c.AddParam("@led_code", int.Parse(ledger_code));
                DataSet dss = c.Select("rec");
                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    //Acct is restricted 
                    req.next_op = 419;
                    return "0";
                }
                else
                {
                    //Continue
                    addParam("NUBAN", nuban, req);
                    return "0";
                }
            }
            else
            {
                //Invalid nuban
                req.next_op = 4190;
                return "0";
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string DisplaySummary(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0 || cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Do you really want to stop the Magic? %0APress 1 to cancel. %0APress 2 to continue";
            return resp;
        }
        else if (cnt == -1)
        {
            removeParam("cnt", req);
            resp = "The account number you entered is not enrolled for this service. %0AKindly dial *822*20*AccountNo# to restart your request";
            return resp;
        }
        else if (cnt == -2)
        {
            removeParam("cnt", req);
            resp = "Kindly dial *822*20*AccountNo# to restart your request";
            return resp;
        }
        else if (cnt == 99)
        {
            removeParam("cnt", req);
            resp = "Sorry this phone number is not registered for this service";
            return resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the account number is exactly 10 digits. %0ADial *822*20*AccountNo#";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the account number is not alphanumeric. %0ADial *822*20*AccountNo#";
            return resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly select a valid number!";
            return "Do you really want to stop the Magic? %0APress 1 to cancel. Press 2 to continue%0A" + resp;
        }

        resp = "Do you really want to stop the Magic? %0APress 1 to cancel. Press 2 to continue";
        return resp;
    }

    public string SaveSummary(UReq req)
    {
        string resp = "0"; int cnt = 0;
        try
        {
            if (req.Msg.Length > 1)
            {
                cnt = -2;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2000;
                resp = "9";
            }
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "1")
            {
                req.next_op = 2001;
            }
            else
            {
                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    //continue: no issues
                }
                else
                {
                    //redirect to summary page
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2000;
                    resp = "9";
                }
            }
        }
        catch
        {

        }

        return resp;
    }

    public string DisableCustomer(UReq req)
    {
        string resp = ""; int summary = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string smsMessage = "";
        try
        {
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 1)
            {
                resp = "Thank you for keeping the magic going. Dial *822*Amount# to buy credit.";
                return resp;
            }
            else
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                c.SetProcedure("spd_DisenrollCust");
                c.AddParam("@mobile", req.Msisdn.Trim());
                c.AddParam("@nuban", nuban);
                int cn = c.Update();
                if (cn > 0)
                {
                    smsMessage = "You are no longer enrolled for Magic Banking. Stay with the Magic and enjoy instant transfers, airtime top-up and more with *822#. Enquiries? Call 070078375464";
                    //SendSms(smsMessage, req.Msisdn);
                    resp = "Your account is no longer profiled for Magic Banking!";
                    return resp;
                }
                else
                {
                    resp = "We were unable to disenroll your account. Kindly try again later.";
                    return resp;
                }
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex.Message);
            resp = "Error occurred during processing";
        }
        return resp;
    }

    public string CollectNuban(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0 || cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter the account you want to disable on this service";
            return resp;
        }
        else if (cnt == -1)
        {
            removeParam("cnt", req);
            resp = "The account number you entered is not enrolled for this service.";
            return resp;
        }
        else if (cnt == 99)
        {
            removeParam("cnt", req);
            resp = "Sorry this phone number is not enrolled for this service";
            return resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the account number is exactly 10 digits.";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the account number is not alphanumeric. %0ADial *822*20*AccountNo#";
            return resp;
        }
        resp = "Kindly enter the account you want to disable on this service";
        return resp;
    }

    public string SaveNuban(UReq req)
    {
        string nuban = ""; int cnt = 0; string ledger_code = "";
        nuban = req.Msg;

        try
        {
            if (nuban.Length != 10)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2002;
                return "0";
            }

            try
            {
                long acctno = long.Parse(nuban);
            }
            catch
            {
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2002;
                return "0";
            }

            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds1 = c.Select("rec");
            if (ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds1.Tables[0].Rows[i];
                    //activated = int.Parse(dr["Activated"].ToString());
                    string regisNuban = dr["nuban"].ToString().Trim();
                    if (nuban != regisNuban)
                    {
                        cnt = -1;
                        continue;
                    }
                    else
                    {
                        //Nuban matches one of customer's profile
                        cnt = 111;
                        //addParam("NUBAN", nuban, req);
                        addParam("cnt", cnt.ToString(), req);
                        break;
                        //return "0";
                    }

                }
                if (cnt == -1)
                {
                    cnt = -1;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2002;
                    return "0";
                }
            }
            else
            {
                cnt = 99;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2002;
                return "0";
            }

            if (nuban.StartsWith("05"))
            {
                //no restricted codes
            }
            else if (nuban.StartsWith("11"))
            {
                //no restricted codes
            }
            else
            {
                DataSet ds = new DataSet();
                EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                ds = ws.getAccountFullInfo(nuban);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    ledger_code = dr["T24_LED_CODE"].ToString();
                    //Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                    c.SetProcedure("spd_getRestrictedLedgCode");
                    c.AddParam("@led_code", int.Parse(ledger_code));
                    DataSet dss = c.Select("rec");
                    if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                    {
                        //Acct is restricted 
                        req.next_op = 419;
                        return "0";
                    }
                    else
                    {
                        //Continue
                        addParam("NUBAN", nuban, req);
                        req.next_op = 20;
                        return "0";
                    }
                }
                else
                {
                    //Invalid nuban
                    req.next_op = 4190;
                    return "0";
                }
            }

            return "0";

        }
        catch
        {
            return "0";
        }
    }
}
