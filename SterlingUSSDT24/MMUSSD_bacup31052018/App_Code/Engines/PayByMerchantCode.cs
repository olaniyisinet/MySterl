using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for PayByMerchantCode
/// </summary>
public class PayByMerchantCode:BaseEngine
{
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public string getMerchDetails(String Mcode)
    {
        string resp = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getMerchDetails");
        c.AddParam("@Merchantcode",decimal.Parse(Mcode));
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            resp = dr["mdetails"].ToString();
        }
        else
        {
            resp = "Error Getting Details";
        }
        return resp;
    }
    public string SaveRequest(UReq req)
    {
        //*822*28*1000*5501#
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
                    addParam("AMOUNT", s[2], req);
                    //use the merchant code in s[3] to get the to beneficiary account
                    string Mcode = s[3];
                    string mdetails = getMerchDetails(Mcode);
                    string[] bits = mdetails.Split('*');
                    //bits[0] is the name while bits[1] is the nuban
                    addParam("TONUBAN", bits[1].Trim(), req);
                    addParam("CusName", bits[0].Trim(), req);

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
    public string DisplaySummary(UReq req)
    {
        Gadget g = new Gadget(); int flag = 0; int ITEMSELECT = 0;
        string resp = ""; string ToNuban = ""; string frmNuban = "";
        string prms = getParams(req); string ToName = ""; decimal amt = 0;
        NameValueCollection prm = splitParam(prms);
        frmNuban = prm["NUBAN"];

        addParam("frmNuban", frmNuban, req);
        ToNuban = prm["TONUBAN"];
        //Check if the account is sterling or imal
        if (ToNuban.StartsWith("05"))
        {
            ImalResponses Irsp1 = new ImalResponses();
            ImalRequest Irqt = new ImalRequest();
            Irqt.account = ToNuban;
            Irqt.requestCode = "112";
            Irqt.principalIdentifier = "003";
            Irqt.referenceCode = "#" + Irqt.principalIdentifier + "#" + g.GenerateRndNumber(12);
            //call imal service
            imal1.NIBankingServiceClient ws = new imal1.NIBankingServiceClient();
            //serialize the json
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(Irqt);
            json = g.Encrypt(json);
            try
            {
                resp = ws.process(json, Irqt.principalIdentifier);
                resp = g.Decrypt(resp);
                Irsp1 = js.Deserialize<ImalResponses>(resp);
                ToName = Irsp1.name;
            }
            catch (Exception ex)
            {
                ToName = "";
            }
        }
        else
        {
            amt = decimal.Parse(prm["AMOUNT"]);
            ToName = prm["CusName"];
        }
        if (ToName == "" || ToName == null)
        {
            resp = "An error occured at this time as we are unable to validate the customers details from the core.  Kindly try again later.";
        }
        else
        {
            resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + ToName + "%0A1.Yes%0A2.No";
        }
        return "Confirmation: " + resp;
    }
    public string SaveSummary(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 88;
            }
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
            req.next_op = 78;
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
            req.next_op = 78;
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
            req.next_op = 78;
            return "9";
        }

        return resp;
    }
    public string DoSubmitS2S(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = "";
        int k = 0; string nuban = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {

            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*28*AMT*MerchantCode# to try again";
                return resp;
            }
            PIN = prm["PIN"];
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,4,@sid,@frmNuban)";
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
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            resp = "Transaction has been submitted for processing";
            Gadget g = new Gadget();
            g.Insert_Charges(req.SessionID, g.GetAccountsByMobileNo2(req.Msisdn), 4);
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }

}