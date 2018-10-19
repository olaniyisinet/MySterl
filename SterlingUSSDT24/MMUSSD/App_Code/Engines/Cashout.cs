using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for Cashout
/// </summary>
public class Cashout : BaseEngine
{
    
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }

    ///*************************************************************************
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
                req.next_op = 230;
                resp = "0";
            }
        }
        catch
        {
            resp = "0";
        }

        return resp;
    }
    public string SaveBeneMobile(UReq req)
    {
        string resp = "0";
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(req.Msg) && req.Msg.Length < 12)
            {
                addParam("BeneMobile", req.Msg, req);
            }
            else
            {
                //redirect
                req.next_op = 232;
                resp = "0";
            }
        }
        catch
        {
            resp = "0";
        }

        return resp;
    }
    public string SaveBeneDetails(UReq req)
    {
        string resp = "0";
        try
        {
            if (req.Msg != null)
            {
                addParam("BeneDetails", req.Msg, req);
            }
            else
            {
                //redirect
                req.next_op = 234;
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
        string beneficiarymob = prm["BeneMobile"];
        decimal amount = decimal.Parse(prm["AMOUNT"]);

        resp = "Are you sure you want to generate a cashout code for %0AN" + getRealMoney(amount) + " to " + beneficiarymob + "%0A1. Yes%0A2. No";

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
                req.next_op = 238;
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
                    req.next_op = 239;
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
            req.next_op = 237;
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
            req.next_op = 237;
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
            req.next_op = 237;
            return "9";
        }

        return resp;
    }
    public string DoCashout(UReq req)
    {
        string resp = ""; int summary = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string benemobile = prm["BeneMobile"];
        string beneinfo = prm["BeneDetails"];
        string amount = getRealMoney(decimal.Parse(prm["AMOUNT"]));
        CashoutReq r = new CashoutReq();

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
                string requestJSON = "";
                r.Amount = decimal.Parse(amount);
                r.Appid = "26";
                r.BeneficiaryDetails = beneinfo;
                r.BeneficiaryMobile = benemobile;
                r.FromNuban = nuban;
                r.OriginatorMobile = req.Msisdn;
                r.currency = 1;
                requestJSON = JsonConvert.SerializeObject(r);
                //Send to cashout service
                var result = Task.Run(() => FireService(requestJSON));
                resp = "Transaction sent for processing. You should receive your half of the cashout code via text";

            }
        }
        catch (Exception ex)
        {
            resp = "Error occurred during processing";
            return resp;
        }

        return resp;
    }

    private void FireService(string input)
    {
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrlCashout"].ToString();
        string apipath = "api/Cashout";
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(BaseUrl);
            var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
            var result = client.PostAsync(apipath, cont).Result;

        }
    }

}

