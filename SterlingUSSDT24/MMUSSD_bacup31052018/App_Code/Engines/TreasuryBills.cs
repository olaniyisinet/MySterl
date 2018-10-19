using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for TreasuryBills
/// </summary>
public class TreasuryBills : BaseEngine
{
    Gadget g = new Gadget();
    public string getRate(string sid, int Item)
    {
        decimal rate = 0; string rsp = ""; DateTime MatDate = new DateTime();
        string sql = "select * from tbl_USSD_Tbills_CusActions " +
            " where sessionid =@sid and itemselected = @Item";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@sid", sid);
        c.AddParam("@Item", Item);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            rate = decimal.Parse(dr["rateval"].ToString());
            MatDate = Convert.ToDateTime(dr["datetomaturity"].ToString());
        }
        return rsp = rate.ToString() + "*" + MatDate.ToString();
    }
    public void InserRec(string sessionid, string mobile, string itemselected, string rateval, DateTime dt)
    {
        string sql = "insert into tbl_USSD_Tbills_CusActions(sessionid,mobile,itemselected,rateval,datetoMaturity) " +
            " values(@sessionid,@mobile,@itemselected,@rateval,@dt)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@sessionid", sessionid);
        c.AddParam("@mobile", mobile);
        c.AddParam("@itemselected", itemselected);
        c.AddParam("@rateval", rateval);
        c.AddParam("@dt", dt);
        c.Execute();
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
    private int RunBankPage(string s)
    {
        int k = 0;
        try
        {
            k = Convert.ToInt32(s);
        }
        catch
        {
        }
        return k;
    }
    public string DisplayTBills(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = RunBankPage(prm["NXTPAG"]);
            List<TreasuryBillItem> lb = g.GetTbillsByPage(page.ToString());
            if (lb.Count <= 0)
            {
                removeParam("NXTPAG", req);
                lb = g.GetTbillsByPage("0");
            }
            resp = "Treasury Bills";
            foreach (TreasuryBillItem tb in lb)
            {
                resp += "%0A" + tb.TransRate + " " + tb.shortname + " " + tb.DatetoMaturity;
            }
            resp += "%0A99 Next Page";
        }
        catch
        {
        }
        return resp;
    }
    public int SaveTbillsSelected(object obj)
    {
        UReq req = (UReq)obj;
        if (req.Msg == "99")
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int page = g.RunTbillNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }
        addParam("SELECTtbills", req.Msg, req);
        return 1;
    }
    public string GetBuyandSellRate(UReq req)
    {
        string[] bits = null; string Tbills = ""; string rates = "";
        string resp = ""; string prms = getParams(req); int dval = 0;
        NameValueCollection prm = splitParam(prms); DateTime dt = new DateTime();
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            int val = int.Parse(prm["SELECTtbills"]);
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getTbillsBuyandSellRate");
            c.AddParam("@TransRate", val);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Tbills = dr["Tbills"].ToString();
                addParam("Tbills", Tbills, req);
                rates = dr["rates"].ToString();
                dt = Convert.ToDateTime(dr["datetoMaturity"].ToString());
            }
            bits = rates.Split('*');
            for (int i = 0; i < bits.Length; i++)
            {
                dval += 1;
                if (dval == 1)
                {
                    resp = "%0A" + dval.ToString() + ". Buy rate " + bits[i];
                    InserRec(req.SessionID, req.Msisdn, dval.ToString(), bits[i], dt);
                }
                else if (dval == 2)
                {
                    resp += "%0A" + dval.ToString() + ". Sell rate " + bits[i];
                    InserRec(req.SessionID, req.Msisdn, dval.ToString(), bits[i], dt);
                }
            }
            return "Treasury Bills%0ABuy or Sell " + Tbills + ":%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the item selected exist within the list";
            int val = int.Parse(prm["SELECTtbills"]);
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getTbillsBuyandSellRate");
            c.AddParam("@TransRate", val);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Tbills = dr["Tbills"].ToString();
                addParam("Tbills", Tbills, req);
                rates = dr["rates"].ToString();
            }
            bits = rates.Split('*');
            for (int i = 0; i < bits.Length; i++)
            {
                dval += 1;
                if (dval == 1)
                {
                    resp = "%0A" + dval.ToString() + ". Buy rate " + bits[i];
                    InserRec(req.SessionID, req.Msisdn, dval.ToString(), bits[i], dt);
                }
                else if (dval == 2)
                {
                    resp += "%0A" + dval.ToString() + ". Sell rate " + bits[i];
                    InserRec(req.SessionID, req.Msisdn, dval.ToString(), bits[i], dt);
                }
            }
            return "Treasury Bills%0ABuy or Sell " + Tbills + ":%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the item selected is not Alphanumeric";
            int val = int.Parse(prm["SELECTtbills"]);
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getTbillsBuyandSellRate");
            c.AddParam("@TransRate", val);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Tbills = dr["Tbills"].ToString();
                addParam("Tbills", Tbills, req);
                rates = dr["rates"].ToString();
            }
            bits = rates.Split('*');
            for (int i = 0; i < bits.Length; i++)
            {
                dval += 1;
                if (dval == 1)
                {
                    resp = "%0A" + dval.ToString() + "Buy rate " + bits[i];
                    InserRec(req.SessionID, req.Msisdn, dval.ToString(), bits[i], dt);
                }
                else if (dval == 2)
                {
                    resp += "%0A" + dval.ToString() + "Sell rate" + bits[i];
                    InserRec(req.SessionID, req.Msisdn, dval.ToString(), bits[i], dt);
                }
            }
            return "Treasury Bills%0ABuy or Sell " + Tbills + ":%0A" + resp;
        }
        return "";
    }
    public string SaveBuyandSellItem(UReq req)
    {
        int cnt = 0;
        //check to ensure the item selected within the list
        if (req.Msg.Length < 1 || req.Msg.Length > 2)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 204;
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
            req.next_op = 204;
            return "9";
        }

        string resp = "0";
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
    public string CollectAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        int ITemSelected = 0;
        NameValueCollection prm = splitParam(prms);
        ITemSelected = int.Parse(prm["ITemSelected"]);
        string txt = "";
        if (ITemSelected == 1)
        {
            txt = "buy";
        }
        else if (ITemSelected == 2)
        {
            txt = "sell";
        }
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter the amount you are willing to " + txt;
            return "Treasury Bills%0A" + "Item to " + txt + " " + prm["Tbills"] + ":%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 100 naira";
            return "Treasury Bills%0A" + "Item to " + txt + " " + prm["Tbills"] + ":%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Treasury Bills%0A" + "Item to " + txt + " " + prm["Tbills"] + ":%0A" + resp;
        }
        return "";
    }
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
            req.next_op = 206;
            return "9";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) <= 100)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 206;
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
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public string DisplaySummary(UReq req)
    {
        StringBuilder rqt = new StringBuilder(); DateTime today = new DateTime(); string[] bits = null;
        string resp = ""; decimal face_value = 0; string prms = getParams(req); decimal debitAmt = 0;
        NameValueCollection prm = splitParam(prms); int ITemSelected = 0; decimal rate = 0; string result = "";
        face_value = decimal.Parse(prm["AMOUNT"]); DateTime MatDate = new DateTime(); decimal numofDays = 0;
        ITemSelected = int.Parse(prm["ITemSelected"]); string Tbills = ""; int numofDaysInt = 0;
        Tbills = prm["Tbills"];
        //use the itemselected to fetch the actual amt for buy or sell
        if (ITemSelected == 1)
        {
            //buy
            //do a select to the table and get the tbl_USSD_Tbills_CusActions for the itemselected and sessionid 
            result = getRate(req.SessionID, ITemSelected);
            bits = result.Split('*');
            rate = decimal.Parse(bits[0]);
            rate = rate / 100;
            MatDate = Convert.ToDateTime(bits[1]);
            TimeSpan span = MatDate.Subtract(DateTime.Now);
            numofDays = (decimal)span.TotalDays;
            numofDaysInt = (int)Math.Ceiling(numofDays);
            //get amount to debit
            debitAmt = face_value - (face_value * rate * numofDaysInt) / 365;
            rqt.Append("Confirmation:%0A");
            rqt.Append("Your are buying Tbills:%0A" + Tbills);
            rqt.Append("%0AFace Value: " + getRealMoney(debitAmt));
            rqt.Append("%0A1.Yes%0A2.No");

        }
        else if (ITemSelected == 2)
        {
            //sell
        }
        //resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + ToName + "%0A1.Yes%0A2.No";
        return rqt.ToString();
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
            req.next_op = 210;
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
            req.next_op = 210;
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
            req.next_op = 210;
            return "9";
        }

        return resp;
    }

    public string DoSubmit(UReq req)
    {

        string resp = ""; int summary = 0; int ITemSelected = 0; decimal AMOUNT = 0;
        int k = 0; string debitacct = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            summary = int.Parse(prm["SUMMARY"]);
            ITemSelected = int.Parse(prm["ITemSelected"]);
            AMOUNT = decimal.Parse(prm["AMOUNT"]);
            debitacct = g.GetAccountsByMobileNo2(req.Msisdn);
            if (summary == 2)
            {
                resp = "Transaction was cancelled by you.  Kindly restart again";
                return resp;
            }

            string sql = "Insert into tbl_USSD_Tbills_txn (sessionid,mobile,debitamt,debitacct,tbillsName,tbillsType) " +
                " values(@sid,@mb,@amt,@dba,@tbN,@tbT)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@amt", AMOUNT);
            cn.AddParam("@dba", debitacct);
            cn.AddParam("@tbN", prm["Tbills"]);
            cn.AddParam("@tbT", ITemSelected);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            resp = "Transaction has been submitted for processing";
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
}
