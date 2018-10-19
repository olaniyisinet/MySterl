using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for CardControl
/// </summary>
public class CardControl : BaseEngine
{
    //*822*19#
    public void MaskPanRecord(string sid, int panid, string maskpan, string stat)
    {
        string sql = "Update tbl_USSD_Pans set Pans= @pan, Status=@stat where SessionID = @ssi and PanID = @panid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetSQL(sql);
        cn.AddParam("@ssi", sid);
        cn.AddParam("@panid", panid);
        cn.AddParam("@pan", maskpan);
        cn.AddParam("@stat", stat);
        cn.Execute();
    }
    public void deleteRecbySid(string sid, int panid)
    {
        string sql = "delete from tbl_USSD_Pans where SessionID = @ssi and PanID != @panid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetSQL(sql);
        cn.AddParam("@ssi", sid);
        cn.AddParam("@panid", panid);
        cn.Execute();
    }
    public void InsertCardPANs(string SessionID, string msisdn, string Pans, int panid, string operation)
    {
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetProcedure("spd_InsertPan_USSD");
        cn.AddParam("@mobileno", msisdn);
        cn.AddParam("@Pan", Pans);
        cn.AddParam("@SessionID", SessionID);
        cn.AddParam("@panid", panid);
        cn.AddParam("@op", operation);
        cn.ExecuteProc();
    }
    public string FetchSelectedpan(UReq req)
    {
        string pan = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string sql = "select Pans from tbl_USSD_Pans where SessionID = @ssi and msisdn = @msdn and PanID = @panid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@panid", prm["PANid"]);
        cn.AddParam("@msdn", req.Msisdn);
        DataSet ds = cn.Select();
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            pan = ds.Tables[0].Rows[0][0].ToString();
        }
        deleteRecbySid(req.SessionID, int.Parse(prm["PANid"]));
        return pan;
    }
    public string GenerateRndNumber(int cnt)
    {
        string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Random rand1 = new Random();
        string txt = "";
        for (int j = 0; j < cnt; j++)
            txt += key2[rand1.Next(0, 9)];
        return txt;
    }
    public string ResponseDescr(string code)
    {
        string txt = "";
        switch (code)
        {
            case "00": txt = "Approved or completed successfully"; break;
            case "03": txt = "Invalid Sender"; break;
            case "21": txt = "No action taken"; break;
            case "26": txt = "Duplicate record"; break;
            case "27": txt = "Invalid cusnum"; break;
            case "28": txt = "Invalid nuban"; break;
            case "29": txt = "Card not found"; break;
            case "99": txt = "Failure on internal service"; break;
            default: txt = "No response from card service"; break;
        }
        return txt;
    }
    public string smsBody(UReq req)
    {
        string body = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string requesttype = prm["REQUESTTYPE"];

        switch (requesttype)
        {
            case "1112":
                body = "Dear %%Customer%%," + Environment.NewLine + "Your %%Pan%% has been enabled." + Environment.NewLine + " Thank you for banking with Sterling Bank.";
                break;
            case "1111":
                body = "Dear %%Customer%%," + Environment.NewLine + "Your %%Pan%% has been disabled." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1139":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have enabled POS transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1141":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have disabled POS transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1136":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have enabled WEB transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1140":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have disabled WEB transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1118":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have enabled ATM transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1117":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have disabled POS transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1126":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have enabled Foreign transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
            case "1125":
                body = "Dear %%Customer%%," + Environment.NewLine + "You have disabled Foreign transactions on your %%Pan%%." + Environment.NewLine + "Thank you for banking with Sterling Bank.";
                break;
        }
        return body;
    }
    public string getname(string nuban)
    {
        string sql = "select CUS_SHOW_NAME from OverallMapperAcctReplica where NUBAN = @nuban";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetSQL(sql);
        cn.AddParam("@nuban", nuban);
        DataSet ds = cn.Select();
        DataRow dr = ds.Tables[0].Rows[0];
        string custname = dr["CUS_SHOW_NAME"].ToString();
        return custname;
    }
    public string getCustID(string nuban, UReq req)
    {
        int cnt = 0; string pan = ""; string op = "Card Control";
        Gadget g = new Gadget();
        string sql = "select CUSTOMER from OverallMapperAcctReplica where NUBAN = @nuban";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetSQL(sql);
        cn.AddParam("@nuban", nuban);
        DataSet ds = cn.Select();
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            string custid = dr["CUSTOMER"].ToString().Trim();
            CardPinService.CardsSoapClient ws = new CardPinService.CardsSoapClient();
            string pans = ws.GetActiveCardsByCustomer(custid);
            if (pans.StartsWith("00"))
            {
                return "No cards available";
            }
            char[] delm = { '~' };
            string[] payload = pans.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < payload.Length; i++)
            {
                char[] cha = { '|' };
                string str = payload[i];
                var sepa = str.Split(cha, StringSplitOptions.RemoveEmptyEntries);
                string panobj = sepa[0];
                string expdate = sepa[1];
                string seq = sepa[2];
                string exp1 = expdate.Substring(0, 2);
                string exp2 = expdate.Substring(2, 2);
                string exdate = exp2 + exp1;
                string paninfo = panobj + "|" + expdate + "|" + seq;

                cnt += 1;
                if (cnt == 1)
                {
                    pan = cnt.ToString() + " " + g.MaskPan(panobj) + " | " + exdate;
                    InsertCardPANs(req.SessionID, req.Msisdn, paninfo, cnt, op);
                }
                else
                {
                    pan += "%0A" + cnt.ToString() + " " + g.MaskPan(panobj) + " | " + exdate;
                    InsertCardPANs(req.SessionID, req.Msisdn, paninfo, cnt, op);
                }
            }
        }

        return "Select PAN %0A" + pan;
    }
    //private string getPAN(UReq req)
    //{
    //    Gadget g = new Gadget();
    //    string pan = ""; int cnt = 0;
    //    string prms = getParams(req);
    //    NameValueCollection prm = splitParam(prms);
    //    string op = "Card Control";
    //    string nuban = prm["NUBAN"];
    //    Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("CardWSConnectionString");
    //    c.SetProcedure("svc_get_pan_by_acctNum");

    //    if (nuban.Length > 10)
    //    {
    //        c.AddParam("@oldAcctNum", nuban);
    //        c.AddParam("@nuban", nuban);
    //    }
    //    else
    //    {
    //        c.AddParam("@nuban", nuban);
    //        c.AddParam("@oldAcctNum", nuban);
    //    }

    //    DataSet ds = c.Select("rec");
    //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
    //    {
    //        DataRow dr = ds.Tables[0].Rows[i];
    //        cnt += 1;
    //        if (cnt == 1)
    //        {
    //            pan = cnt.ToString() + " " + g.MaskPan(dr["pan"].ToString().Trim());
    //            InsertCardPANs(req.SessionID, req.Msisdn, dr["pan"].ToString().Trim(), cnt, op);
    //        }
    //        else
    //        {
    //            pan += "%0A" + cnt.ToString() + " " + g.MaskPan(dr["pan"].ToString().Trim());
    //            InsertCardPANs(req.SessionID, req.Msisdn, dr["pan"].ToString().Trim(), cnt, op);
    //        }
    //    }


    //    return "Select PAN %0A" + pan;
    //}


    //***************************************************************************************************************
    public string SaveRequest(UReq req)
    {
        //*822*19*Method
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 6)
            {
                addParam("METHOD", req.Msg, req);
            }
            else
            {
                //redirect
                req.next_op = 199;
                return "9";
            }
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = ""; string method = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());
                nuban = dr["nuban"].ToString();
                addParam("NUBAN", nuban, req);
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }

            if (activated == 1)
            {
                char[] delm = { '*' };
                string msg = req.Msg.Trim();
                if (nuban == msg)
                {
                    req.next_op = 122;
                    return "9";
                }
                else
                {
                    string prms = getParams(req);
                    NameValueCollection prm = splitParam(prms);
                    method = prm["METHOD"];

                    switch (method)
                    {
                        case "1":
                            req.next_op = 151;
                            resp = "1";
                            break;
                        case "2":
                            req.next_op = 151;
                            resp = "2";
                            break;
                        case "3":
                            req.next_op = 151;
                            resp = "3";
                            break;
                        case "4":
                            req.next_op = 151;
                            resp = "4";
                            break;
                        case "5":
                            req.next_op = 151;
                            resp = "5";
                            break;

                    }
                    if (Convert.ToInt32(msg) > 5)
                    {
                        req.next_op = 199;
                        resp = "9";
                    }
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
    public string SaveSubMethod(UReq req)
    {
        //*822*19*METHOD*SUBMETHOD
        string resp = "0";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string method = prm["METHOD"];
        var regexItem = new Regex("^[0-9]*$");
        if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
        {
            try
            {
                switch (method)
                {
                    case "1":
                        addParam("SUBMETHOD", req.Msg, req);
                        break;
                    case "2":
                        addParam("SUBMETHOD", req.Msg, req);
                        break;
                    case "3":
                        addParam("SUBMETHOD", req.Msg, req);
                        break;
                    case "4":
                        addParam("SUBMETHOD", req.Msg, req);
                        break;
                    case "5":
                        addParam("SUBMETHOD", req.Msg, req);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
        else
        {
            //redirect
            switch (method)
            {
                case "1":
                    req.next_op = 151;
                    resp = "1";
                    break;
                case "2":
                    req.next_op = 151;
                    resp = "2";
                    break;
                case "3":
                    req.next_op = 151;
                    resp = "3";
                    break;
                case "4":
                    req.next_op = 151;
                    resp = "4";
                    break;
                case "5":
                    req.next_op = 151;
                    resp = "5";
                    break;
            }
        }

        return resp;
    }
    public string SaveSelectPan(UReq req)
    {
        string resp = "0"; Gadget g = new Gadget();
        var regexItem = new Regex("^[0-9]*$");
        if (regexItem.IsMatch(req.Msg))
        {
            try
            {
                addParam("PANid", req.Msg, req);
                string selectedpan = FetchSelectedpan(req);
                char[] cha = { '|' };
                var sepa = selectedpan.Split(cha, StringSplitOptions.RemoveEmptyEntries);
                string panonly = sepa[0];
                string exp = sepa[1];
                string seqnum = sepa[2];
                addParam("ExpDate", exp, req);
                addParam("PAN", panonly, req);
                addParam("SeqNum", seqnum, req);
                MaskPanRecord(req.SessionID, int.Parse(req.Msg), g.MaskPan(panonly) + "|" + exp + "|" + seqnum, "pan saved");
            }
            catch
            {
                req.next_op = 993;
                resp = "0";
            }

        }
        else
        {
            //redirect
            req.next_op = 993;
            resp = "0";
        }
        return resp;
    }
    public string DisplayPans(UReq req)
    {
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string pans = getCustID(nuban, req);
        return pans;
    }
    public string DisplaySubSummary(UReq req)
    {
        Gadget g = new Gadget();
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string submethod = prm["SUBMETHOD"];
        string method = prm["METHOD"];
        string pan = prm["PAN"];
        string maskedpan = g.MaskPan(pan);

        //if (panid != "")
        //{
        switch (method)
        {
            case "1":
                if (submethod == "1")
                {
                    resp = "Are you sure you want to Enable" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                else
                {
                    resp = "Are you sure you want to Disable " + "%0A" + maskedpan + " %0AN10.50 fee applies %0A1. Yes%0A2. No";
                }
                break;
            case "2":
                if (submethod == "1")
                {
                    resp = "Are you sure you want to Enable POS transactions for " + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                else
                {
                    resp = "Are you sure you want to Disable POS transactions for " + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                break;
            case "3":
                if (submethod == "1")
                {
                    resp = "Are you sure you want to Enable WEB transactions for" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                else
                {
                    resp = "Are you sure you want to Disable WEB transactions for" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                break;
            case "4":
                if (submethod == "1")
                {
                    resp = "Are you sure you want to Enable ATM transactions for" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                else
                {
                    resp = "Are you sure you want to Enable ATM transactions for" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                break;
            case "5":
                if (submethod == "1")
                {
                    resp = "Are you sure you want to Enable foreign transactions for" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                else
                {
                    resp = "Are you sure you want to Disable foreign transactions for" + "%0A" + maskedpan + "%0A1. Yes%0A2. No";
                }
                break;

        }

        //}
        //else
        //{
        //    resp = "No response returned from the core";
        //}

        return resp;
    }
    public string SaveSummary(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 9999;
            }
            else
            {
                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                string method = prm["METHOD"];
                string submethod = prm["SUBMETHOD"];

                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    switch (method)
                    {
                        case "1":
                            if (submethod == "1")
                            {
                                addParam("REQUESTTYPE", "1112", req);
                            }
                            else
                            {
                                addParam("REQUESTTYPE", "1111", req);
                            }
                            break;
                        case "2":
                            if (submethod == "1")
                            {
                                addParam("REQUESTTYPE", "1139", req);
                            }
                            else
                            {
                                addParam("REQUESTTYPE", "1141", req);
                            }
                            break;
                        case "3":
                            if (submethod == "1")
                            {
                                addParam("REQUESTTYPE", "1136", req);
                            }
                            else
                            {
                                addParam("REQUESTTYPE", "1140", req);
                            }
                            break;
                        case "4":
                            if (submethod == "1")
                            {
                                addParam("REQUESTTYPE", "1118", req);
                            }
                            else
                            {
                                addParam("REQUESTTYPE", "1117", req);
                            }
                            break;
                        case "5":
                            if (submethod == "1")
                            {
                                addParam("REQUESTTYPE", "1126", req);
                            }
                            else
                            {
                                addParam("REQUESTTYPE", "1125", req);
                            }
                            break;
                    }
                }
                else
                {
                    //redirect to respective summary menu
                    switch (method)
                    {
                        default:
                            req.next_op = 992;
                            resp = "0";
                            break;
                    }

                }
            }
        }
        catch
        {

        }

        return resp;
    }
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
            resp = "The PIN you supplied does not correspond with the PIN you registered.  Kindly enter the correct USSD PIN";
            return "Authentication:%0A" + resp;
        }
        resp = "Enter your USSD PIN";
        return "Authentication:%0A" + resp;
    }
    public string SavetxnPIN(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 6994;
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
            req.next_op = 6994;
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
            req.next_op = 6994;
            return "9";
        }

        return resp;
    }
    public string SubmitReq(UReq req)
    {
        string resp = ""; int summary = 0; string requesttype = ""; string nuban = ""; string pan = "";
        Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Operation cancelled. Please dial *822*19# to try again";
                return resp;
            }

            requesttype = prm["REQUESTTYPE"];
            nuban = prm["NUBAN"];
            pan = prm["PAN"];
            CardCtrltxn trx = new CardCtrltxn();
            trx.AccountNumber = nuban;
            trx.AccountType = "";
            trx.RequestID = GenerateRndNumber(3) + DateTime.Now.ToString("MMddhhmm") + GenerateRndNumber(5);
            trx.RequestType = requesttype;
            trx.Pan = pan;

            string str = trx.CreateReq();
            var result = Task.Run(() => FireService(str, req));

            resp = "Your request is being processed.";
            //trx.SendRequest(str);
            //resp = ResponseDescr(trx.ResponseCode);

            //if (trx.ResponseCode == "00")
            //{
            //    status = "Success";
            //    string custname = getname(nuban);
            //    string smsbody = smsBody(req);
            //    smsbody = smsbody.Replace("%%Customer%%", custname);
            //    pan = g.MaskPan(pan);
            //    smsbody = smsbody.Replace("%%Pan%%", pan);
            //    smsService.SMSINFOBIPSoapClient SMS = new smsService.SMSINFOBIPSoapClient();
            //    //string SMSresp = SMS.insertIntoInfobip(smsbody, req.Msisdn);
            //}
            //else
            //{
            //    status = "Failed";
            //    //continue without sending SMS
            //}
            //MaskPanRecord(req.SessionID, int.Parse(prm["PANid"]), g.MaskPan(pan), status);

            //if (requesttype == "1111")
            //{
            //    g.Insert_Charges(req.SessionID, g.GetAccountsByMobileNo2(req.Msisdn), 5);
            //}
        }

        catch (Exception ex)
        {
            resp = "Error occurred during processing";
            return resp;
        }

        return resp;

    }
    public void FireService(string request, UReq req)
    {
        string resp = ""; string status = ""; string requesttype = ""; string nuban = ""; string pan = "";
        CardCtrltxn trx = new CardCtrltxn();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        Gadget g = new Gadget();
        smsService.SMSINFOBIPSoapClient SMS = new smsService.SMSINFOBIPSoapClient();
        nuban = prm["NUBAN"];
        requesttype = prm["REQUESTTYPE"];
        string custname = getname(nuban);


        try
        {
            trx.SendRequest(request);
            resp = ResponseDescr(trx.ResponseCode);
            pan = prm["PAN"];

            if (trx.ResponseCode == "00")
            {
                status = "Success";

                string smsbody = smsBody(req);
                smsbody = smsbody.Replace("%%Customer%%", custname);
                pan = g.MaskPan(pan);
                smsbody = smsbody.Replace("%%Pan%%", pan);
                //string SMSresp = SMS.insertIntoInfobip(smsbody, req.Msisdn);
            }
            else
            {
                status = "Failed";
                //string SMSresp = SMS.insertIntoInfobip("Dear " + custname + ", your card control operation was unsuccessful. Please try again later.", req.Msisdn);
                //continue without sending SMS
            }

            if (requesttype == "1111")
            {
                g.Insert_Charges(req.SessionID, g.GetAccountsByMobileNo2(req.Msisdn), 5);
            }

            MaskPanRecord(req.SessionID, int.Parse(prm["PANid"]), g.MaskPan(pan), resp);

           
        }
        catch (Exception ex)
        {
            new ErrorLog("Error processing card control with sessionid:" + req.SessionID + Environment.NewLine + ex.Message);
            //string SMSresp = SMS.insertIntoInfobip("Dear " + custname + ", your card control operation was unsuccessful. Please try again later.", req.Msisdn);
        }

    }
}