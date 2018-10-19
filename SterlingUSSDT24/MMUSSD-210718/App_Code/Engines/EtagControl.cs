using com.sbp.instantacct.service;
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
using System.Web;
using System.Xml;
using BankCore;
using BankCore.t24;
using System.Threading.Tasks;

/// <summary>
/// Summary description for EtagControl
/// </summary>
public class EtagControl : BaseEngine
{
    //*822*15# 
    public String Encrypt(String val, Int32 Appid)
    {
        MemoryStream ms = new MemoryStream();
        Gadget g = new Gadget();
        string rsp = "";
        try
        {
            string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
            sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqibs");
            c.SetSQL(sql);
            c.AddParam("@Appid", Appid);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                sharedkeyval = dr["SharedKey"].ToString();
                sharedkeyval = g.BinaryToString(sharedkeyval);

                sharedvectorval = dr["Sharedvector"].ToString();
                sharedvectorval = g.BinaryToString(sharedvectorval);
            }

            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }
        catch
        {
            //new ErrorLog("There is an issue with the xml received " + val + " Invalid xml");
            //rsp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSResponse><ResponseCode>57</ResponseCode><ResponseText>Transaction not permitted to sender</ResponseText></IBSResponse>";
            //rsp = Encrypt(rsp, Appid);
            //return rsp;
        }
        return Convert.ToBase64String(ms.ToArray());
    }
    public String Decrypt(String val, Int32 Appid)
    {
        MemoryStream ms = new MemoryStream();
        Gadget g = new Gadget();
        string rsp = "";
        try
        {

            string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
            sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqibs");
            c.SetSQL(sql);
            c.AddParam("@Appid", Appid);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                sharedkeyval = dr["SharedKey"].ToString();
                sharedkeyval = g.BinaryToString(sharedkeyval);

                sharedvectorval = dr["Sharedvector"].ToString();
                sharedvectorval = g.BinaryToString(sharedvectorval);
            }

            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();
        }
        catch
        {
            //new ErrorLog("There is an issue with the xml received " + val);
            //rsp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSResponse><ResponseCode>57</ResponseCode><ResponseText>Transaction not permitted to sender</ResponseText></IBSResponse>";
            //rsp = Encrypt(rsp, Appid);
            //return rsp;
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }
    public static void UpdateRecord(int status, string ssid)
    {
        string sql = "update tbl_USSD_trnx_chrgTaken set statusflag =@status where sessionid=@ssid ";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@status", status);
        c.AddParam("@ssid", ssid);
        c.Execute();
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
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public static void InsertTransfer(UReq req, string frmAcct, decimal amount, string pin, string toAcct)
    {
        int k = 0;
        string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,5,@sid,@frmNuban)";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@mb", req.Msisdn);
        cn.AddParam("@ta", toAcct);
        cn.AddParam("@am", amount);
        cn.AddParam("@pn", pin);
        cn.AddParam("@sid", req.SessionID);
        cn.AddParam("@frmNuban", frmAcct);
        k = Convert.ToInt32(cn.Insert());

        if (k > 0)
        {
            Gadget g = new Gadget();
            g.Insert_Charges(req.SessionID, g.GetAccountsByMobileNo2(req.Msisdn), 7);
        }
        else
        {
        }
    }
    public static void InsertLCCtrns(string ssid, string mobileno)
    {
        string sql = "Insert into tbl_USSD_LCCtrns (SessionID,Mobile,DateAdded) values (@ssid,@mobile,@date) ";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobileno);
        c.AddParam("@ssid", ssid);
        c.AddParam("@date", DateTime.Now);
        c.Execute();
    }
    public static void UpdateLCCresp(string ssid, string mobileno, string etagno, string custauthid, string request, string response)
    {
        string sql = "Update tbl_USSD_LCCtrns set EtagNo=@etag, LccResponse=@resp, CustAuthID=@pin, LccRequest=@req, DateProcessed=@date where SessionID=@ssid and Mobile=@mobile";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobileno);
        c.AddParam("@ssid", ssid);
        c.AddParam("@date", DateTime.Now);
        c.AddParam("@pin", custauthid);
        c.AddParam("@req", request);
        c.AddParam("@resp", response);
        c.AddParam("@etag", etagno);
        c.Execute();

    }
    public static void UpdateLCCtrns_IBS(string ssid, string mobileno, string etagno, string frmAcct, string toAcct, decimal amt, string custauthid, string ibsrespcode)
    {
        string sql = "Update tbl_USSD_LCCtrns set EtagNo=@etag, FromAccount=@frmacct, ToAccount=@toacct, Amount=@amt, CustAuthID=@pin, IbsResponseCode=@ibscode, DateProcessed=@date where SessionID=@ssid and Mobile=@mobile";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobileno);
        c.AddParam("@ssid", ssid);
        c.AddParam("@etag", etagno);
        c.AddParam("@frmacct", frmAcct);
        c.AddParam("@toacct", toAcct);
        c.AddParam("@amt", amt);
        c.AddParam("@pin", custauthid);
        c.AddParam("@ibscode", ibsrespcode);
        c.AddParam("@date", DateTime.Now);
        c.Execute();
    }
    public static void UpdateLCCtrns_Etag(string ssid, string mobileno, string etagno, string custauthid, string lccreq, string lccreqid, string lccresp, string lccrespcode, string respdesc)
    {
        string sql = "Update tbl_USSD_LCCtrns set EtagNo=@etag, CustAuthID=@pin, LccRequest=@lccreq, LccResponse=@lccresp, LccRequestID=@lccreqid,LccResponseCode=@respcode,LccResponseDesc=@respdesc, DateProcessed=@date where SessionID=@ssid and Mobile=@mobile";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobileno);
        c.AddParam("@ssid", ssid);
        c.AddParam("@etag", etagno);
        c.AddParam("@pin", custauthid);
        c.AddParam("@lccreq", lccreq);
        c.AddParam("@lccresp", lccresp);

        if (lccreqid == "" || lccreqid == null)
        {
            c.AddParam("@lccreqid", DBNull.Value);
        }
        else
        {
            c.AddParam("@lccreqid", lccreqid);
        }

        if (lccrespcode == "" || lccrespcode == null)
        {
            c.AddParam("@respcode", DBNull.Value);
        }
        else
        {
            c.AddParam("@respcode", lccrespcode);
        }

        if (respdesc == "" || respdesc == null)
        {
            c.AddParam("@respdesc", DBNull.Value);
        }
        else
        {
            c.AddParam("@respdesc", respdesc);
        }
        c.AddParam("@date", DateTime.Now);
        c.Execute();
    }
    //***********************************************************************************************
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
                //if (ds.Tables[0].Rows.Count > 1)
                //{
                //    if (req.Msg == "2")
                //    {
                //        req.next_op = 15050;
                //        return "0";
                //    }
                //    else
                //    {
                //        req.next_op = 15040;
                //        return "0";
                //    }

                //}
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());
                nuban = dr["nuban"].ToString();
                addParam("NUBAN", nuban, req);

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
            resp += "%0A99 Next";
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
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
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
        addParam("A", "2", req);
        return 1;
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
            resp += "%0A99 Next";
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
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
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
        addParam("A", "2", req);
        return 1;
    }
    public string SaveEtagno(UReq req)
    {
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9-]*$");
            if (regexItem.IsMatch(req.Msg) && 6 < req.Msg.Length && req.Msg.Length < 10)
            {
                addParam("EtagNo", req.Msg, req);
            }
            else
            {
                //redirect
                switch (req.op)
                {
                    case 15017:
                        req.next_op = 15016;
                        break;
                    case 15031:
                        req.next_op = 15030;
                        break;
                        //case 15051:
                        //    req.next_op = 15050;
                        //    break;
                }
                resp = "0";
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveAmount1(UReq req)
    {
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(req.Msg))
            {
                addParam("AMOUNT", req.Msg, req);
            }
            else
            {
                //redirect
                req.next_op = 15018;
                resp = "0";
            }
        }
        catch
        {
            resp = "0";
        }

        return resp;
    }
    public string DisplayTopUpSummary(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string etagno = prm["EtagNo"];
        resp = "Do you want to top up " + etagno + " with N" + getRealMoney(decimal.Parse(prm["AMOUNT"])) + "%0AN105.00 fee applies  %0A1. Yes%0A2. No  ";
        return resp;
    }
    public string DisplayBalSummary(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string etagno = prm["EtagNo"];

        resp = "Do you want to check the balance on this card:%0A" + etagno + "%0AN21.00 fee applies %0A1. Yes%0A2. No";

        return resp;
    }
    public string SaveBalSummary(UReq req)
    {
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 15036;
            }
            else if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
            {
                resp = "0";
            }
            else
            {
                req.next_op = 15032;
            }
        }
        catch
        {
            req.next_op = 15032;
        }

        return resp;
    }
    public string SaveTopUpSummary(UReq req)
    {
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 15024;
            }
            else if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
            {
                resp = "0";
            }
            else
            {
                req.next_op = 15020;
            }
        }

        catch
        {
            req.next_op = 15020;
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
            switch (req.op)
            {
                case 15022:
                    req.next_op = 15023;
                    return "9";
                case 15034:
                    req.next_op = 15035;
                    return "9";
                    //case 15054:
                    //    req.next_op = 15055;
                    //    return "9";
            }
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
            switch (req.op)
            {
                case 15022:
                    req.next_op = 15023;
                    return "9";
                case 15034:
                    req.next_op = 15035;
                    return "9";
                case 15054:
                    req.next_op = 15055;
                    return "9";
            }
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
            switch (req.op)
            {
                case 15022:
                    req.next_op = 15023;
                    return "9";
                case 15034:
                    req.next_op = 15035;
                    return "9";
                case 15054:
                    req.next_op = 15055;
                    return "9";
            }
        }

        return resp;
    }
    public string DoTopUp(UReq req)
    {
        StringBuilder rqt = new StringBuilder();
        StringBuilder rsp = new StringBuilder(); Gadget g = new Gadget();
        string resp = ""; int summary = 0; string apipath = ""; string requestJSON = ""; string frmNuban = "";
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrlEtag"].ToString();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string amount = prm["AMOUNT"];
        string etagno = prm["EtagNo"];

        try
        {
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Operation cancelled. Please dial *822*15# to try again";
                return resp;
            }

            //Insert record for reference
            InsertLCCtrns(req.SessionID, req.Msisdn);

            EtagDoTopReq r = new EtagDoTopReq();
            //REQUEST SENT TO LCC FOR PROCESSING
            apipath = "api/Etag/DoTopUp";
            r.amount = int.Parse(amount);
            r.etag = etagno;
            r.appid = "26";
            requestJSON = JsonConvert.SerializeObject(r);

            //Handles debit and Etag call
            var result = Task.Factory.StartNew(() => FireService(requestJSON, apipath, req, "TopUp"));
            resp = "Transaction sent for processing.";
        
        }
        catch (Exception ex)
        {
            new ErrorLog(ex.Message);
            resp = "Error occurred during processing";
        }

        return resp;
    }
    public string DoBalCheck(UReq req)
    {
        Gadget g = new Gadget();
        string resp = ""; int summary = 0; string apipath = ""; string requestJSON = ""; string frmNuban = "";
        string jsoncontent = ""; int flag = 0; int ITEMSELECT = 0;
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrlEtag"].ToString();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            //flag = int.Parse(prm["A"]);
            //if (flag == 1)
            //{
            //    frmNuban = prm["NUBAN"];
            //}
            //else if (flag == 2)
            //{
            //    ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
            //    frmNuban = g.GetNubanByListID(ITEMSELECT, req);
            //}
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Operation cancelled. Please dial *822*15# to try again";
                return resp;
            }

            //Insert LCC record for reference
            InsertLCCtrns(req.SessionID, req.Msisdn);

            string etagno = prm["EtagNo"];
            EtagReq r = new EtagReq();
            r.etag = etagno;
            r.appid = "26";
            apipath = "api/Etag/GetCustomerBal";

            requestJSON = JsonConvert.SerializeObject(r);
            var result = Task.Run(() => FireService(requestJSON, apipath, req, "CheckBalance"));
            resp = "Transaction sent for processing.";
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = "Error occurred during processing";
        }
        return resp;
    }

    private void FireService(string input, string apipath, UReq req, string method)
    {
        StringBuilder rqt = new StringBuilder();
        SMSAPIInfoBip sms = new SMSAPIInfoBip();
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrlEtag"].ToString();
        string prms = getParams(req); Gadget g = new Gadget();
        string sid = req.SessionID;
        NameValueCollection prm = splitParam(prms);
        string respcode = ""; string desc = ""; string lccreqid = ""; string resp = "";
        string jsoncontent = ""; string etagno = prm["EtagNo"]; string pin = prm["PIN"]; string frmNuban = prm["NUBAN"];

        switch (method)
        {
            case "TopUp":
                string amount = prm["AMOUNT"];
                decimal amt = Convert.ToDecimal(amount);
                string toAccount = ConfigurationManager.AppSettings["EtagtoAccount"].ToString();
                string VendorAccount = ConfigurationManager.AppSettings["EtagVendor"].ToString();

                ibs1.BSServicesSoapClient ws = new ibs1.BSServicesSoapClient();
                rqt.Clear();
                rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                rqt.Append("<IBSRequest>");
                rqt.Append("<ReferenceID>" + GenerateRndNumber(6) + DateTime.Now.ToString("HHmmss") + "</ReferenceID>");
                rqt.Append("<RequestType>" + "102" + "</RequestType>");
                rqt.Append("<FromAccount>" + frmNuban + "</FromAccount>");
                rqt.Append("<ToAccount>" + toAccount + "</ToAccount>");
                rqt.Append("<Amount>" + amt.ToString() + "</Amount>");
                rqt.Append("<PaymentReference>" + "Amount taken for Etag topup on USSD " + sid + "</PaymentReference>");
                rqt.Append("</IBSRequest>");
                string str = "";
                str = rqt.ToString();
                string IBSreq = rqt.ToString();
                str = Encrypt(str, 254);
                resp = ws.IBSBridge(str, 254);
                try
                {
                    resp = Decrypt(resp, 254);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(resp);
                    string responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                    string responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                    //Insert response from ibs
                    UpdateLCCtrns_IBS(req.SessionID, req.Msisdn, etagno, frmNuban, toAccount, Convert.ToDecimal(amount), pin, responseCode);

                    if (responseCode == "00")
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(BaseUrl);
                            var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
                            var result = client.PostAsync(apipath, cont).Result;
                            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                jsoncontent = result.Content.ReadAsStringAsync().Result;
                                jsoncontent = jsoncontent.Replace(@"\", "");
                                jsoncontent = jsoncontent.Replace("\"{", "{");
                                jsoncontent = jsoncontent.Replace("}\"", "}");
                                UpdateLCCresp(req.SessionID, req.Msisdn, etagno, pin, input, jsoncontent);
                                try
                                {
                                    var resp1 = Newtonsoft.Json.JsonConvert.DeserializeObject<EtagTopRespSucc>(jsoncontent);
                                    string status = resp1.status;
                                    string paymentref = resp1.reference;
                                    respcode = resp1.responsecode;
                                    if (respcode == "00")
                                    {
                                        desc = "Successful transaction";
                                        InsertTransfer(req, toAccount, Convert.ToDecimal(amount), pin, VendorAccount);
                                        //update statusflag to 1 to be treated 
                                        UpdateRecord(1, req.SessionID);
                                        string smsStatus = sms.insertIntoInfobip("Your E-tag account " + etagno + " was successfully credited with N" + Convert.ToDecimal(amount), req.Msisdn);
                                    }
                                    else if (respcode == "01" || respcode == null)
                                    {
                                        desc = "Failed transaction";
                                        string smsStatus = sms.insertIntoInfobip("We were unable to credit your Etag account. Please try again later.", req.Msisdn);
                                    }
                                    lccreqid = resp1.requestid;
                                    UpdateLCCtrns_Etag(req.SessionID, req.Msisdn, etagno, pin, input, lccreqid, jsoncontent, respcode, desc);
                                }
                                catch
                                {
                                    var content = Newtonsoft.Json.JsonConvert.DeserializeObject<EtagBalResFail>(jsoncontent);
                                    string status = content.status;
                                    lccreqid = content.requestid;
                                    respcode = content.responsecode;
                                    if (respcode == "01")
                                    {
                                        desc = "Failed Transaction";                                        
                                    }
                                    string smsStatus = sms.insertIntoInfobip("An error occurred processing your request. Please try again later.", req.Msisdn);
                                    UpdateLCCtrns_Etag(req.SessionID, req.Msisdn, etagno, pin, input, lccreqid, jsoncontent, respcode, desc);
                                }

                            }
                            else
                            {
                                jsoncontent = result.Content.ReadAsStringAsync().Result;
                                jsoncontent = jsoncontent.Replace(@"\", "");
                                jsoncontent = jsoncontent.Replace("\"{", "{");
                                jsoncontent = jsoncontent.Replace("}\"", "}");
                                UpdateLCCresp(req.SessionID, req.Msisdn, etagno, pin, input, jsoncontent);
                                respcode = "99";
                                desc = "Internal communication error";
                                string smsStatus = sms.insertIntoInfobip("An error occurred processing your request. Please try again later.", req.Msisdn);
                                UpdateLCCtrns_Etag(req.SessionID, req.Msisdn, etagno, pin, input, lccreqid, jsoncontent, respcode, desc);
                            }
                        }

                    }
                    else
                    {
                        new ErrorLog("Unable to debit account for Etag top up USSD sessionid: " + sid);
                    }
                }
                catch (Exception ex)
                {
                    new ErrorLog("Error occured while processing the transaction with Session id " + sid);                    
                }
                                
                break;
            case "CheckBalance":
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
                    var result = client.PostAsync(apipath, cont).Result;

                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        jsoncontent = result.Content.ReadAsStringAsync().Result;
                        jsoncontent = jsoncontent.Replace(@"\", "");
                        jsoncontent = jsoncontent.Replace("\"{", "{");
                        jsoncontent = jsoncontent.Replace("}\"", "}");
                        UpdateLCCresp(req.SessionID, req.Msisdn, etagno, pin, input, jsoncontent);
                        try
                        {
                            var resp1 = JsonConvert.DeserializeObject<EtagBalResSucc>(jsoncontent);
                            string bal = resp1.Data.balance;
                            string baldate = resp1.Data.balance_date;
                            string status = resp1.status;
                            lccreqid = resp1.requestId;
                            respcode = resp1.responsecode;
                            if (respcode == "00")
                            {
                                desc = "Successful transaction";
                                string smsStatus = sms.insertIntoInfobip("The balance on your E-tag account " + etagno + " is " + bal, req.Msisdn);
                            }                            
                            //resp = "Status: " + status + "%0ABalance: " + bal + "%0ABalance date: " + baldate + "";
                            UpdateLCCtrns_Etag(req.SessionID, req.Msisdn, etagno, pin, input, lccreqid, jsoncontent, respcode, desc);
                            g.Insert_Charges(req.SessionID, frmNuban, 6);
                        }
                        catch
                        {
                            var content = JsonConvert.DeserializeObject<EtagBalResFail>(jsoncontent);
                            string status = content.status;
                            //string errormsg = content.error;
                            lccreqid = content.requestid;
                            respcode = content.responsecode;
                            if (respcode == "01")
                            {
                                desc = "Failed transaction";
                            }
                            string smsStatus = sms.insertIntoInfobip("An error occurred processing your request. Please try again later.", req.Msisdn);
                            UpdateLCCtrns_Etag(req.SessionID, req.Msisdn, etagno, pin, input, lccreqid, jsoncontent, respcode, desc);
                            //resp = "Status: " + status + "";
                        }
                    }
                    else
                    {
                        jsoncontent = result.Content.ReadAsStringAsync().Result;
                        UpdateLCCresp(req.SessionID, req.Msisdn, etagno, pin, input, jsoncontent);
                        respcode = "99";
                        desc = "Internal communication error";
                        UpdateLCCtrns_Etag(req.SessionID, req.Msisdn, etagno, pin, input, lccreqid, jsoncontent, respcode, desc);
                        string smsStatus = sms.insertIntoInfobip("An error occurred processing your request. Please try again later.", req.Msisdn);
                        //resp = "Error occurred during processing";
                    }
                }
                break;
        }


    }
}