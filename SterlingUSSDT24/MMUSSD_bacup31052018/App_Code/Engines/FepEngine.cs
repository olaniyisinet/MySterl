using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;

/// <summary>
/// Summary description for FepEngine
/// </summary>
public class FepEngine : BaseEngine
{
    public string pan; public int panid;public string sessionid;
    Gadget g = new Gadget(); private readonly object _locker = new object();
    //check to ensure user is registered, has pin set and redirect to the next screen
    public string IsUserRegis(UReq req)
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
                        return "0";
                    }
                    else
                    {
                        return "0";
                    }
                }
                catch
                {
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
    public string DoCardNotFound(UReq req)
    {
        return "Unable to Card record. %0AWe could not locate your record at this time.";
    }
    public string GetCusPANS(UReq req)
    {
        string resp = "";string customerid = ""; string padCusnum = ""; string cardPAN = ""; char[] sep = { '*' };
        string pan = ""; int val = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        //get the customerid of the registered customer
        customerid = g.GetCusNumByMobileNo(req.Msisdn);
        //
        padCusnum = g.getPaddedCusnum(customerid);
        cardPAN = g.getPAN_Seqnr(customerid, padCusnum);
        if (cardPAN == "")
        {
            cnt = 20;
            addParam("cnt1", cnt.ToString(), req);
            return "No Card Details found.";
        }

        if(cnt == 0)
        {
            //get response
            resp = g.getPAN(cardPAN, req.SessionID);
            // first entrance
            removeParam("cnt", req);
            return "Select PAN to set ATM PIN:%0A" + resp;
        }
        else if(cnt == 100)
        {
            //ensure the number selected forms part of the list
            //go to the table and read from the table based on the current sessionid
            resp = g.getPANBySessionID(sessionid);
            removeParam("cnt", req);
            return "Select PAN to set ATM PIN:%0A Ensure that the number you select is within the pan list%0A" + resp;
        }
        else if (cnt == 101)
        {
            //ensure the number selected forms part of the list
            //go to the table and read from the table based on the current sessionid
            resp = g.getPANBySessionID(sessionid);
            removeParam("cnt", req);
            return "Select PAN to set ATM PIN:%0A Kindly ensure you enter only numeric value from the list displayed %0A" + resp;
        }
        return "";
    }
    public string SavePANID(UReq req)
    {
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt1 = RunCount(prm["cnt1"]);
        if(cnt1 ==20)
        {
            req.next_op = 61;
            return "7";
        }
        int cnt = 0; string resp = "";
        //check if entry is numeric
        try
        {
            long val = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 62;
            return "9";
        }
        //save it
        try
        {
            addParam("PanID", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CollectExpiryDate(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Card Expiry Date in this format yyMM";
            return "Card Expiry Date:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the total length is not less than or more than 4 digits";
            return "Card Expiry Date:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the expiry date entered is not alphanumeric";
            return "Card Expiry Date:%0A" + resp;
        }
        return "";
    }
    public string SaveExpiryDate(UReq req)
    {
        int cnt = 0; string resp = "";
        //check if entry is numeric
        try
        {
            long expirydate = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 64;
            return "9";
        }
        if (req.Msg.Length >4 || req.Msg.Length <4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 64;
            return "9";
        }
        //save it
        try
        {
            addParam("Expdate", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SetATMPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Set ATM PIN";
            return  resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 4 digits";
            return  resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 digits and not alphanumeric";
            return  resp;
        }
        return "Set ATM PIN:%0A" + resp;
    }
    public string SaveCusPINATM(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 66;
            return "9";
        }
        //check if entry is numeric
        try
        {
            int pin = int.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 66;
            return "9";
        }
        string resp = "0";
        try
        {
            Encrypto EnDeP = new Encrypto();
            req.Msg = EnDeP.Encrypt(req.Msg);
            cnt = 0;
            addParam("SaveATMPIN", req.Msg, req);
            addParam("cnt", cnt.ToString(), req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string ConfirmSetATMPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter the same 4 digits";
            return "Confirm your Set ATM PIN%0A: " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "Confirm your Set ATM PIN%0A: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "Confirm your Set ATM PIN%0A: " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The confirmation PIN did not match the intial PIN you entered.  Kindly ensure they are the same";
            return "Confirm your Set ATM PIN%0A: " + resp;
        }
        return "";
    }
    public string SaveConfrimPINATM(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 68;
            return "9";
        }
        //check if entry is numeric
        try
        {
            int pin = int.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 68;
            return "9";
        }
        //do get the display info
        string prms = getParams(req); string resp = "0";
        NameValueCollection prm = splitParam(prms);
        string Initial_PIN = prm["SaveATMPIN"];
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        if (Initial_PIN == req.Msg)
        {
            resp = "0";
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 68;
            return "9";
        }

        try
        {
            addParam("SaveConfrimPIN", req.Msg, req);

        }
        catch
        {
            resp = "0";
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
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 70;
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
            req.next_op = 70;
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
            req.next_op = 70;
            return "9";
        }

        return resp;
    }
    public string DoATMPINChange(UReq req)
    {
        fepservices.fepservicesSoapClient ws = new fepservices.fepservicesSoapClient();
        string resp = "";string pan = "";string exp_date = ""; string pin = ""; string seq_nr = "";
        int k = 0;string pantoSend = ""; string panid = ""; Encrypto EnDeP = new Encrypto();
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            panid = prm["PanID"];
            //use the panid selected and goto the table to fetch the stored pan that matches the panid
            pan = g.getPANbyID(panid, req.SessionID);
            string[] bits = pan.Split('+');

            exp_date = prm["Expdate"];
            pin = EnDeP.Decrypt(prm["SaveATMPIN"]);
            pantoSend = bits[0];
            seq_nr = bits[1];

            resp = ws.ChangePIN(pantoSend, pin, seq_nr, exp_date, "1");
            if (resp == "00")
            {
                g.InsertFInalRecord(req.Msisdn, req.SessionID, pan, seq_nr, exp_date, resp);
                resp = "PIN change was successful.  Kindly proceed to any ATM to carry out your transaction";
            } 
            else if (resp == "01")
            {
                g.InsertFInalRecord(req.Msisdn, req.SessionID, pan, seq_nr, exp_date, resp);
                resp = "PIN change was not successfully. Please ensure the details supplied are correct";
            }
            
        }
        catch
        {
            return resp = "Error occured at this time.  Kindly try again";
        }


        return resp;
    }
}