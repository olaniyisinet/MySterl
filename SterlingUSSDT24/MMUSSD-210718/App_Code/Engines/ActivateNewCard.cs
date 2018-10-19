using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for ActivateNewCard
/// </summary>
public class ActivateNewCard : BaseEngine
{
    public void MaskPanRecord(string sid, int panid, string maskpan, string status)
    {
        string sql = "Update tbl_USSD_Pans set Pans= @pan, Status = @stat where SessionID = @ssi and PanID = @panid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetSQL(sql);
        cn.AddParam("@ssi", sid);
        cn.AddParam("@panid", panid);
        cn.AddParam("@pan", maskpan);
        cn.AddParam("@stat", status);
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
    public void InsertCardPANs(string sessionid, string msisdn, string pans, int panid, string operation)
    {
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetProcedure("spd_InsertPan_USSD");
        cn.AddParam("@mobileno", msisdn);
        cn.AddParam("@Pan", pans);
        cn.AddParam("@SessionID", sessionid);
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
    public string getCustID(string nuban, UReq req)
    {
        int cnt = 0; string pan = ""; string op = "Card Activation";
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
            string pans = ws.GetValidNewCardsByCustomer(custid);
            if (pans.StartsWith("00"))
            {
                return "No cards available to activate";
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
    //*********************************************************************************************************
    public string SaveRequest(UReq req)
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
    public string DisplayPans(UReq req)
    {
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string pans = getCustID(nuban, req);
        return pans;
    }
    public string SavePan(UReq req)
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
                req.next_op = 250;
                resp = "0";
            }
            
        }
        else
        {
            //redirect
            req.next_op = 250;
            resp = "0";
        }

        return resp;
    }
    public string SaveNewPin(UReq req)
    {
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(req.Msg) && req.Msg.Length == 4)
            {
                addParam("NewPin", req.Msg, req);
            }
            else
            {
                //redirect
                req.next_op = 252;
                resp = "0";
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
        Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);

        //string panid = prm["PANid"];
        //string selectedpan = FetchSelectedpan(req);
        //char[] cha = { '|' };
        //var sepa = selectedpan.Split(cha, StringSplitOptions.RemoveEmptyEntries);
        //string panonly = sepa[0];
        //string exp = sepa[1];
        //string seqnum = sepa[2];
        //addParam("ExpDate", exp, req);
        //addParam("PAN", panonly, req);
        //addParam("SeqNum", seqnum, req);
        string panonly = prm["PAN"];
        string maskedpan = g.MaskPan(panonly);

        resp = "Are you sure you want to activate %0A" + maskedpan + "%0A1. Yes%0A2. No";

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
                req.next_op = 258;
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
                    req.next_op = 254;
                    resp = "0";
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
            req.next_op = 257;
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
            req.next_op = 257;
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
            req.next_op = 257;
            return "9";
        }

        return resp;
    }
    public string DoSubmit(UReq req)
    {
        Gadget g = new Gadget();
        string resp = ""; int summary = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);

        string pan = prm["PAN"];
        string seqnum = prm["SeqNum"];
        string newPin = prm["NewPin"];
        string exDate = prm["ExpDate"];
        string status = "";
        try
        {
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Operation cancelled. Please dial *822# to try again.";
                return resp;
            }
            else
            {
                //string pan = prm["PAN"];
                //string seqnum = prm["SeqNum"];
                //string newPin = prm["NewPin"];
                //string exDate = prm["ExpDate"];
                //string status = "";

                CardPinService.CardsSoapClient ws = new CardPinService.CardsSoapClient();
                //List<string> response = new List<string>();

                string[] response = ws.ActivateNewCard(pan, seqnum, exDate, newPin);
                if (response[0].StartsWith("00"))
                {
                    resp = "Card successfully activated";
                    status = "Success";
                }
                else
                {
                    resp = "Card could not be activated. %0A An error occurred.";
                    status = "Failed";
                }
                MaskPanRecord(req.SessionID, int.Parse(prm["PANid"]), g.MaskPan(pan) + "|" + exDate + "|" + seqnum, status);
            }
        }
        catch (Exception ex)
        {
            MaskPanRecord(req.SessionID, int.Parse(prm["PANid"]), g.MaskPan(pan) + "|" + exDate + "|" + seqnum, "Error occurred");
            new ErrorLog(ex.Message);
            resp = "Error occurred during processing";
            return resp;
        }

        return resp;
    }
}