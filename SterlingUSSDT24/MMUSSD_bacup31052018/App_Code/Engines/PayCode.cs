using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// Summary description for PayCode
/// </summary>
public class PayCode : BaseEngine
{
    string EncryptPIN = "";
    Encrypto EnDeP = new Encrypto();
    //****************** set onetime pin ***********************************
    public string SetOneTimePIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your One-time 4 digit PIN for your cardless transaction";
            return "Set one-time PIN:%0A " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "Set one-time PIN:%0A " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "Set one-time PIN:%0A " + resp;
        }
        return "Set one-time PIN:%0A " + resp;
    }
    public string SaveOneTimePIN(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 321;
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
            req.next_op = 321;
            return "9";
        }
        string resp = "0";
        try
        {
            //encrypt at this stage
            //encrypt the value
            req.Msg = EnDeP.Encrypt(req.Msg);
            cnt = 0;
            addParam("oneTimePin", req.Msg, req);
            addParam("cnt", cnt.ToString(), req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    //****************************************************************
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
    public string CollectAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter the amount";
            return "Cardless Withdrawal%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 100 naira and multiples of 1,000 under 20,000 naira";
            return "Cardless Withdrawal%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Cardless Withdrawal%0A" + resp;
        }
        return "";
    }
    public string SaveTheAMt(UReq req)
    {
        int cnt = 0; string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int ITemSelected = int.Parse(prm["ITemSelected"]);
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 302;
            return "9";
        }
        //check to ensure the amount is more than 100
        if (ITemSelected == 1)//For ATM transactions
        {
            if (decimal.Parse(req.Msg) <= 100 || decimal.Parse(req.Msg) > 20000 || decimal.Parse(req.Msg) % 1000 != 0)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 302;
                return "9";
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
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public string DisplayTerminal(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            return "Cardless Withdrawal%0A1 ATM %0A2 POS";
        }
        else if (cnt == 100)
        {
            resp = "Kindly ensure that the item selected is within what was displayed";
            return "Cardless Withdrawal%0A " + resp + " Cardless Withdrawal %0A1 ATM %0A2 POS";
        }
        else if (cnt == 101)
        {
            resp = "Alphanumeric is not allowed.  Kindly select from the item displayed";
            return "Cardless Withdrawal%0A " + resp + " Cardless Withdrawal %0A1 ATM %0A2 POS";
        }
        return "";
    }
    public string SaveTerminalSel(UReq req)
    {
        string resp = "0";
        int cnt = 0;
        //check to ensure the item selected within the list
        if (int.Parse(req.Msg) > 2)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 304;
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
            req.next_op = 304;
            return "9";
        }

        try
        {
            addParam("ITemSelected", req.Msg, req);
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
            req.next_op = 308;
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
            req.next_op = 308;
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
            req.next_op = 308;
            return "9";
        }

        return resp;
    }
    public string DisplaySummary(UReq req)
    {
        string resp = ""; Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        decimal amt = decimal.Parse(prm["AMOUNT"]);
        string frmNuban = g.GetAccountsByMobileNo2(req.Msisdn);
        resp = "Summary%0APaycode will generated for amount: " + getRealMoney(amt) + "%0Amobile: " + req.Msisdn + "%0A1.Yes%0A2.No";
        return resp;
    }
    public string SaveSummary(UReq req)
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
    public string DoSubmit(UReq req)
    {
        //  System.Diagnostics.Debug.Write(typeof(string).Assembly.ImageRuntimeVersion);

        Gadget g = new Gadget(); string paycode = ""; string oneTimePin = "";
        string resp = ""; int summary = 0; int ITemSelected = 0; decimal AMOUNT = 0;
        int k = 0; string debitacct = "";
        try
        {
            string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
            string prms = getParams(req); string apipath = "";
            NameValueCollection prm = splitParam(prms);
            summary = int.Parse(prm["SUMMARY"]);
            ITemSelected = int.Parse(prm["ITemSelected"]);
            AMOUNT = decimal.Parse(prm["AMOUNT"]);
            oneTimePin = EnDeP.Decrypt(prm["oneTimePin"]);
            debitacct = g.GetAccountsByMobileNo2(req.Msisdn);
            if (summary == 2)
            {
                resp = "Transaction was cancelled.  Kindly dial *822*42# to try again";
                return resp;
            }

            //proceed to call the api
            ChannelReq r = new ChannelReq();
            if (ITemSelected == 1)
            {
                r.paymentChannel = "ATM";
            }
            if (ITemSelected == 2)
            {
                r.paymentChannel = "POS";
            }
            r.oneTimePin = oneTimePin;
            r.accountNo = debitacct;
            r.amount = (int)(AMOUNT);
            r.appid = "26";
            r.codeGenerationChannel = "26";
            r.subscriber = req.Msisdn;// "2348124888436";// req.Msisdn;
            r.transactionRef = g.GenerateRndNumber(3) + DateTime.Now.ToString("hhss" + "26");// "1234588";// DateTime.Now.ToString("yyyyMMddhhmmss" + "26");
            r.ttid = r.transactionRef;
            string jsoncontent = "";
            apipath = "api/Paycode/GenerateTokenReq";
            string respcode = ""; string requestJSON = "";

            requestJSON = JsonConvert.SerializeObject(r);
            var result = Task.Run(() => FireService(requestJSON));
            resp = "Transaction sent for processing. %0AYour paycode should be sent via SMS";

            
        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }


        return resp;
    }
    private void FireService(string input)
    {
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(BaseUrl);
            string apipath = "api/Paycode/GenerateTokenReq";
            var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");
           // var result = Task.Run(() => client.PostAsync(apipath, cont));

            var result = client.PostAsync(apipath, cont).Result;
        }
    }

    //******************** cancel token
    public string getTransRef(string mob, string pcode)
    {
        string rsp = "";
        string sql = "select transactionRef from tbl_payode where subscriber =@mob and paycodeGen =@pcode and appid=26";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_pcode");
        c.SetSQL(sql);
        c.AddParam("@mob", mob);
        c.AddParam("@pcode", pcode);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            rsp = dr["transactionRef"].ToString();
        }
        return rsp;
    }
    public string CollectPaycode(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Paycode to cancel";
            return "Cardless Withdrawal%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the Paycode entered is not alphanumeric";
            return "Cardless Withdrawal%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Unable able to locate paycode entered. Kindly enter the correct paycode";
            return "Cardless Withdrawal%0A" + resp;
        }
        return "";
    }

    public string SaveThePaycode(UReq req)
    {
        int cnt = 0; string resp = "";
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 202021;
            return "9";
        }
        //check if the paycode entered is exist
        string ispaycodeExist = getTransRef(req.Msisdn, req.Msg);// getTransRef("2348124888436", req.Msg);// getTransRef(req.Msisdn, req.Msg); 
        if (ispaycodeExist == "" || ispaycodeExist == null)
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 202021;
            return "9";
        }
        //save it
        try
        {
            addParam("Paycode", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string CancelPaycode(UReq req)
    {
        Gadget g = new Gadget(); string resp = ""; string pcode = ""; string paycode = "";
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();
        string prms = getParams(req); string apipath = "";
        NameValueCollection prm = splitParam(prms);
        pcode = prm["Paycode"]; string transactionRef = "";
        //use the pcode and the subscribers number to get the transactionref
        transactionRef = getTransRef(req.Msisdn, pcode);// getTransRef("2348124888436", pcode);// getTransRef(req.Msisdn, pcode);
        CancelRequest r = new CancelRequest();
        r.appid = "26";
        r.transactionRef = transactionRef;

        string jsoncontent = "";
        apipath = "api/Paycode/CancelTokenReq";
        string requestJSON = "";
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(BaseUrl);
            requestJSON = JsonConvert.SerializeObject(r);
            var cont = new StringContent(requestJSON, System.Text.Encoding.UTF8, "application/json");
            var result = client.PostAsync(apipath, cont).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace("\\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                var resp1 = Newtonsoft.Json.JsonConvert.DeserializeObject<CancelPaycodeSuc>(jsoncontent);
                if (resp1.code == "00")
                {
                    resp = "Cashout %0APacode " + pcode + " was cancelled succesfully.  Kindly dial *822*42# to get another token for cashout";
                }
            }
            else
            {
                resp = "Unable to cancel " + pcode + " Kindly try again later";
            }
        }
        return resp;
    }
}