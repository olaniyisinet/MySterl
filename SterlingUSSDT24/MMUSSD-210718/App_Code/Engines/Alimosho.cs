using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// Summary description for Alimosho
/// </summary>
public class Alimosho : BaseEngine
{
    public string baseUrl = ConfigurationManager.AppSettings["AlimoshoUrl"];
    public string password = ConfigurationManager.AppSettings["AlimoshoPassword"];
    public string username = ConfigurationManager.AppSettings["AlimoshoUser"];

    public string GetSignature(string value)
    {
        string result = string.Empty;
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        HashAlgorithm hashAlgorithm = null;
        switch ("512")
        {
            //case SignatureMethod.SHA1: hashAlgorithm = new SHA1Managed(); break;
            //case SignatureMethod.SHA256: hashAlgorithm = new SHA256Managed(); break;
            case "512": hashAlgorithm = new SHA512Managed(); break;
        }
        if (hashAlgorithm != null)
            result = Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
        return result;
    }

    public long LogAliRequest(string sessionid, string fullrequest)
    {
        long refid = 0;
        string sql = "insert into tbl_Alimosho_Logs (SessionID,Request) values (@ssid,@req)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@ssid", sessionid);
        c.AddParam("@req", fullrequest);
        try
        {
            refid = Convert.ToInt64(c.Insert());
        }
        catch
        {
            refid = -1;
        }

        return refid;
    }

    public void LogAliResponse(long refid, string fullResponse, string responseCode)
    {
        string sql = "Update tbl_Alimosho_Logs  set ResponseCode=@respcode, Response= @resp, DateProcessed=@date where RefID = @refid";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@resp", fullResponse);
        c.AddParam("@respcode", responseCode ?? "");
        c.AddParam("@date", DateTime.Now);
        c.AddParam("@refid", refid);
        c.Update();
    }

    public string SaveRequest(UReq req)
    {
        string resp = "0"; string amount = "";

        try
        {
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            //string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            //amount = s[2].Trim();

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

                addParam("NUBAN", nuban, req);
                // addParam("Amt", amount, req);
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }

            if (activated == 1)
            {
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

    public string CollectBillNum(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //int cnt = RunCount(prm["cnt"]);
        try
        {
            int cnt = RunCount(prm["cnt"]);
            if (cnt == 0)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "Kindly enter your TIN Number:";
                return resp;
            }
            else if (cnt == 100)
            {
                removeParam("cnt", req);
                resp = "Kindly ensure you enter digits only";
                return "Collect TIN Number:%0A" + resp;
            }
            else if (cnt == 101)
            {
                removeParam("cnt", req);
                resp = "Kindly ensure you enter a number less than 18 digits";
                return "Collect TIN Number:%0A" + resp;
            }
            else if (cnt == 102)
            {
                removeParam("cnt", req);
                resp = "Kindly ensure you enter a valid bill number";
                return "Collect TIN Number:%0A" + resp;
            }
            else if (cnt == 103)
            {
                removeParam("cnt", req);
                resp = "An error occurred validating your details. Please try again later";
                return resp;
            }

        }
        catch
        {
            resp = "Kindly enter your TIN Number:";
        }

        resp = "Kindly enter your TIN Number:";
        return resp;
    }

    public string SaveBillNum(UReq req)
    {
        string resp = "0"; int cnt = 0; string jsoncontent = "";
        try
        {
            string billNumber = req.Msg.Trim();

            //Check if it is number only
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(billNumber))
            {
                //continue: no issues
            }
            else
            {
                //redirect to Collect bill number page
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 55000;
                resp = "0";
            }

            //Ensure number is less than 18 digits
            if (billNumber.Length > 18)
            {
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 55000;
                resp = "0";
            }
            //else
            //{
            //    req.next_op = 99999;
            //    return resp;
            //}

            string apipath = "api/Alimosho/ValidationInquire";
            AlimoPaymentValidReq r = new AlimoPaymentValidReq();
            r.billNumber = billNumber;
            r.appid = "26";
            string input = JsonConvert.SerializeObject(r);
            long refid = LogAliRequest(req.SessionID, input);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var cont = new StringContent(input, Encoding.UTF8, "application/json");
                var result = client.PostAsync(apipath, cont).Result;

                try
                {
                    jsoncontent = result.Content.ReadAsStringAsync().Result;
                    jsoncontent = jsoncontent.Replace(@"\", "");
                    jsoncontent = jsoncontent.Replace("\"{", "{");
                    jsoncontent = jsoncontent.Replace("}\"", "}");
                    var resp1 = JsonConvert.DeserializeObject<AlimoPaymentValidResp>(jsoncontent);
                    LogAliResponse(refid, jsoncontent, resp1.validationResponse.statusCode);
                    if (string.IsNullOrEmpty(resp1.validationResponse.statusCode) || resp1.validationResponse.statusCode != "00")
                    {
                        cnt = 102;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 55000;
                        resp = "0";
                    }

                    string aliSession = resp1.validationResponse.sessionid;
                    string amtDue = resp1.validationResponse.amtPaid;
                    string name = resp1.validationResponse.CustomerName;
                    //Assign param values to use later
                    addParam("AliSession", aliSession, req);
                    addParam("AmtDue", amtDue, req);
                    addParam("BillNum", billNumber, req);
                    addParam("Name", name, req);

                }
                catch (Exception ex)
                {
                    new ErrorLog("ALIMOSHO ERROR getting Payment validation " + ex);
                    LogAliResponse(refid, jsoncontent, "");
                    cnt = 103;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 55000;
                    resp = "0";
                }

                return resp;
            }

        }
        catch
        {

        }

        return resp;
    }

    public string DisplayItems(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            int cnt = RunCount(prm["cnt"]);
            string amtDue = prm["AmtDue"];
            if (cnt == 100)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "Please enter a valid amount";
                return "Payment to Alimosho LG%0A" + resp;
            }
            else if (cnt == 101)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "Please enter an amount lower than or equal to N" + getRealMoney(Convert.ToDecimal(amtDue));
                return "Payment to Alimosho LG%0A" + resp;
            }

            
            resp = "Payment to Alimosho LG%0AYour amount due is N" + getRealMoney(Convert.ToDecimal(amtDue)) + "%0APlease enter the amount you want to pay";

        }
        catch
        {
            resp = "Error occurred treating your request";
        }

        return resp;
    }

    public string SaveItem(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            string amountPaid = req.Msg.Trim();

            //Check if it is number only
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(amountPaid) && Convert.ToDecimal(amountPaid) > 0)
            {
                //continue: no issues
            }
            else
            {
                //redirect to Collect bill number page
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 55002;
                resp = "0";
            }

            string amtDue = prm["AmtDue"];
            if(decimal.Parse(amountPaid) > decimal.Parse(amtDue))
            {
                //redirect to Collect bill number page
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 55002;
                resp = "0";
            }


            addParam("AmtToPay", amountPaid, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string CheckNoofAccts(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 55008;
            return "0";
        }
        else
        {
            req.next_op = 55005;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb(UReq req)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getAcctList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 55006;
            return "0";
        }
        else
        {
            return "0";
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
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "99")
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
                        //Customer doesnt need pin to buy airtime for self
                    }
                }
            }
        }
        else
        {
            //customer is okay to continue
        }

        string amount = prm["Amt"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);
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

            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 1;
                return 2;
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
                req.op = 1;
                return 2;
            }
            else
            {
                //proceed

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

                }
                else
                {
                    cnt = 102;
                    removeParam("Amt", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 1;
                    return 2;
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

    public string DisplaySummary(UReq req)
    {
        string resp = "";
        Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string frmNuban = ""; int ITEMSELECT = 0; int flag = 0;

        flag = int.Parse(prm["A"]);
        if (flag == 1)
        {
            frmNuban = getDefaultAccount(req.Msisdn);
        }
        else if (flag == 2)
        {
            ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
            frmNuban = g.GetNubanByListID(ITEMSELECT, req);
        }

        string amtToPay = prm["AmtToPay"];

        addParam("frmNuban", frmNuban, req);
        resp = "Are you sure you want to pay N" + getRealMoney(Convert.ToDecimal(amtToPay)) + " from " + frmNuban + " to Alimosho LG(Rates and Levies) %0A1. Yes%0A2. No";


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
                req.next_op = 55012;
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
                    req.next_op = 55008;
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
            resp = "The USSD PIN you provided is incorrect. Please check and try again.";
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
            req.next_op = 55010;
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
            req.next_op = 55010;
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
            req.next_op = 55010;
            return "0";
        }

        return resp;
    }

    public string DoSubmit(UReq req)
    {
        string resp = ""; int summary = 0; string amtToPay = ""; string aliSession = ""; string billNumber = ""; string frmNuban = ""; string custname = "";
        Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {

            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Operation cancelled. Please dial *822*55# to try again";
                return resp;
            }

            amtToPay = prm["AmtToPay"];
            aliSession = prm["AliSession"];
            billNumber = prm["BillNum"];
            frmNuban = prm["frmNuban"];
            custname = prm["Name"];

            //Submit request to table to treated
            string sql = "Insert into tbl_Alimosho_trans (Appid,UniqueRef,AliSession,Amount,BillNumber,FrmAccount,PhoneNo,CustName) values (@appid,@uniqref,@sessid,@amt,@billno,@acct,@mob,@name)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconnAli");
            cn.SetSQL(sql);
            cn.AddParam("@appid", "26");
            cn.AddParam("@uniqref", req.SessionID);
            cn.AddParam("@sessid", aliSession);
            cn.AddParam("@amt", amtToPay);
            cn.AddParam("@billno", billNumber);
            cn.AddParam("@acct", frmNuban);
            cn.AddParam("@mob", req.Msisdn);
            cn.AddParam("@name", custname);
            int k = Convert.ToInt32(cn.Insert());
            if (k > 0)
            {
                resp = "Transaction has been submitted for processing";
            }
            else
            {
                resp = "Unable to submit please try again";
            }

        }
        catch
        {
            resp = "An error occurred treating your request. Please try again later.";
        }

        return resp;
    }

}