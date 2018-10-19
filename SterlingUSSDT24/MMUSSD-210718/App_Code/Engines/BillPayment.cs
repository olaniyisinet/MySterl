using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for BillPayment
/// </summary>
public class BillPayment :BaseEngine
{


    public string nm;
    public string mob;
    public int bid;
    public int bcid;
    public decimal billamt;
    public decimal billfeecharge;
    public int itemref;
    public string sid;


    public int Categoryid;
    public int BillerID;
    public int ItemRefid;
    public string Name;
    public string Sessionid;
    public string mobile;

    //************************************* GET BILLER GROUPS ATTACHED TO USSD CHANNEL ***************************************************//
    public void Insert_USSD_BillPmt_Group(int CategoryID, string Sessionid, string Name, string mobile, int UssdRef)
    {
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetProcedure("spd_Insert_USSD_BillPmt_Group");
        cn.AddParam("@CategoryID",CategoryID);
        cn.AddParam("@Sessionid",Sessionid);
        cn.AddParam("@Name", Name);
        cn.AddParam("@mobile", mobile);
        cn.AddParam("@UssdRef", UssdRef);
        cn.ExecuteProc();
    }
    public string GetBillersAttached(UReq req)
    {
        int cnt = 0; int val = 0; string Name = ""; int CategoryID = 0; string resp = "";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_bills");
        cn.SetProcedure("spd_USSD_Get_BillPmt_Group");
        cn.AddParam("@channelID", 11);
        DataSet ds = cn.Select();
        cnt = ds.Tables[0].Rows.Count;
        if(cnt > 0)
        {
            for(int i=0; i < cnt; i++)
            {
                val += 1;
                DataRow dr = ds.Tables[0].Rows[i];
                Name = dr["Name"].ToString();
                CategoryID = int.Parse(dr["CategoryID"].ToString());
                resp += "%0A" + val.ToString() + ". " + Name;
                Insert_USSD_BillPmt_Group(CategoryID, req.SessionID, Name, req.Msisdn, val);
            }
        }
        return "Select Group Type: " + resp;
    }
    //***********************************************************************************************************************************//

    //************************************************************************************************************************************//
    public string DisplayBillerCategories(UReq req)
    {
        int billertypeid = 0; int catid = 0;
        billertypeid = int.Parse(req.Msg); //the current item selected by user
        //go to the table tbl_USSD_BillPmt_Group and get the category id based on user input
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_USSD_GetCategoryByUSSDRef");
        c.AddParam("@uref", billertypeid);
        c.AddParam("@sid", req.SessionID);
        c.AddParam("@mob", req.Msisdn);
        DataSet ds1 = c.Select("rec");
        if(ds1.Tables[0].Rows.Count > 0)
        {
            DataRow dr1 = ds1.Tables[0].Rows[0];
            catid = int.Parse(dr1["categoryID"].ToString());
        }

        Utility ut = new Utility();int val = 0;
        string resp = "";int cnt = 0; 
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_bills");
        cn.SetProcedure("spd_USSD_Biller_Category_items");
        cn.AddParam("@catid", catid); 
        cn.AddParam("@channelid", 11); 
        DataSet ds = cn.Select();
        cnt = ds.Tables[0].Rows.Count;
        if(cnt > 0)
        {
            for(int i =0; i < cnt; i++)
            {
                val += 1;
                DataRow dr = ds.Tables[0].Rows[i];
                Categoryid = int.Parse(dr["Categoryid"].ToString());
                BillerID = int.Parse(dr["BillerID"].ToString());
                Name = dr["Name"].ToString();
                resp += "%0A" + val.ToString() + ". " + Name;
                //pass the values to be inserted into the table
                insertPmtCategory(req.SessionID, req.Msisdn, BillerID, Categoryid, Name, val);
                Thread.Sleep(100);
            }
        }
        else
        {
            resp = "An Internal error has occured at this time.  Please Dial *822# to retry again";
        }
        return "Select your Preference: " + resp;
    }

    public void insertPmtCategory(string sessionid, string mobile, int billid, int catid, string billName, int val)
    {
        this.Sessionid = sessionid;
        this.mobile = mobile;
        this.BillerID = billid;
        this.Categoryid = catid;
        this.Name = billName;
        this.ItemRefid = val;        
        Thread worker = new Thread(new ThreadStart(saveLog));
        worker.Start();
    }
    public void saveLog()
    {
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
        cn.SetProcedure("spd_Insert_USSD_BillPmt_Categories");
        cn.AddParam("@Categoryid", this.Categoryid);
        cn.AddParam("@BillerID", this.BillerID);
        cn.AddParam("@Name", this.Name);
        cn.AddParam("@Sessionid", this.Sessionid);
        cn.AddParam("@mobile", this.mobile);
        cn.AddParam("@ItemRefid", this.ItemRefid);
        cn.ExecuteProc();
    }

    public string SaveGroup(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("GroupID", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    //************************************************************************************************************************************//

    //***********************************************************************************************************************************//
    public string getFromAcctBySid(string sid)
    {
        string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getFromAcctNum");
        c.AddParam("@sid", sid);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["nuban"].ToString();
        }
        else
        {
            return "-1";
        }
        return nuban;
    }
    public string getSessionDetails(string sid)
    {
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        string prms = ""; int BillerID = 0; string frmNuban = "";int ItemRef = 0; string name = "";
        string val = ""; string acctName = ""; string mob = ""; int payitemselected = 0; decimal amt = 0; decimal fee = 0; string ItemName = "";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetProcedure("spd_GetReqStateBySID");
        cn.AddParam("@sid", sid);
        DataSet ds = cn.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            prms = dr["params"].ToString();
            mob = dr["msisdn"].ToString();
            NameValueCollection prm = splitParam(prms);

            payitemselected = int.Parse(prm["PayItemSelectedID"]);
            ItemRef = int.Parse(prm["ItemRef"]);
            frmNuban = getFromAcctBySid(sid);
            //DataSet ds1 = ws.getCustomerAccountsByMobileNo(mob);//Get account by mobile
            //if (ds1.Tables[0].Rows.Count > 0)
            //{
            //    DataRow dr2 = ds1.Tables[0].Rows[0];
            //    acctName = dr2["AccountName"].ToString();
            //}

            //go to the table tbl_USSD_BillPmt_PaymentItems and get the details selectd by the customer
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getCustomerSelectedItem");
            c.AddParam("@itemrefid", payitemselected);
            c.AddParam("@mob", mob);
            c.AddParam("@sessionid", sid);
            DataSet ds2 = c.Select("rec");
            if(ds2.Tables[0].Rows.Count > 0)
            {
                DataRow dr2 = ds2.Tables[0].Rows[0];
                amt = decimal.Parse(dr2["BillAmount"].ToString());
                fee = decimal.Parse(dr2["BillCharge"].ToString());
                ItemName = dr2["Name"].ToString();
                BillerID = int.Parse(dr2["BillerID"].ToString());
            }
            Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect();
            c1.SetProcedure("spd_GetPaymentSelectedCategory");
            c1.AddParam("@ItemRefid", ItemRef);
            c1.AddParam("@Sessionid", sid);
            DataSet ds1 = c1.Select("rec");
            if(ds1.Tables[0].Rows.Count > 0)
            {
                DataRow dr1 = ds1.Tables[0].Rows[0];
                name = dr1["Name"].ToString();
            }
            val = mob + "*" + frmNuban + "*" + amt.ToString() + "*" + fee.ToString() + "*" + name + "(" + ItemName + ")" + "*" + BillerID.ToString();
        }
        return val;
    }
    public static string FormatAmt(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    private readonly object _locker = new object();
    public void saveRec()
    {
        lock (_locker)
        {
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
            cn.SetProcedure("spd_Insert_USSD_BillPmt_PaymentItems");
            cn.AddParam("@Sessionid", this.sid);
            cn.AddParam("@mobile", this.mob);
            cn.AddParam("@Name", this.nm);
            cn.AddParam("@BillerID", this.bid);
            cn.AddParam("@BillCategoryid", this.bcid);
            cn.AddParam("@BillAmount", this.billamt);
            cn.AddParam("@BillCharge", this.billfeecharge);
            cn.AddParam("@itemrefid", this.itemref);
            cn.ExecuteProc();
        }
    }
    public void InsertPmtItems(string sessionid, string mobile, string Name, int BillerID, int BillCategoryid, decimal BillAmount, decimal BillCharge, int itemrefid)
    {
        this.sid = sessionid;
        this.mob = mobile;
        this.nm = Name;
        this.bid = BillerID;
        this.bcid = BillCategoryid;
        this.billamt = BillAmount;
        this.billfeecharge = BillCharge;
        this.itemref = itemrefid;

        Thread worker = new Thread(new ThreadStart(saveRec));
        worker.Start();
    }
    public string DisplayPaymtItemByItemRef(UReq req)
    {
        int ItemRef = 0; int Categoryid = 0; int Billerid = 0; string resp = "";
        ItemRef = int.Parse(req.Msg); int cnt = 0; int val=0;
        string Name=""; int billercategoryid=0; decimal theBillamt=0; decimal theBillcharge=0;
         //go to the table tbl_USSD_BillPmt_Group and get the category id based on user input
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getPayment_details_By_ItemRefID");
        c.AddParam("@itemRefid", ItemRef);
        c.AddParam("@Sessionid", req.SessionID);
        c.AddParam("@mobile", req.Msisdn);
        DataSet ds1 = c.Select("rec");
        if(ds1.Tables[0].Rows.Count > 0)
        {
            DataRow dr1 = ds1.Tables[0].Rows[0];
            Categoryid = int.Parse(dr1["Categoryid"].ToString());
            Billerid = int.Parse(dr1["Billerid"].ToString());
        }

        if(Categoryid!=0 || Billerid !=0)
        {
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_bills");
            cn.SetProcedure("spd_USSD_GetBill_Pmt_ItemsByIDs");
            cn.AddParam("@channelid", 11);
            cn.AddParam("@billerid", Billerid);
            cn.AddParam("@billercategoryid", Categoryid);
            DataSet ds = cn.Select();
            cnt = ds.Tables[0].Rows.Count;
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    val+=1;
                    DataRow dr = ds.Tables[0].Rows[i];
                    Name = dr["Name"].ToString();
                    Billerid = int.Parse(dr["Billerid"].ToString());
                    billercategoryid = int.Parse(dr["billercategoryid"].ToString());
                    theBillamt = decimal.Parse(dr["BillAmount"].ToString());
                    theBillcharge = decimal.Parse(dr["BillCharge"].ToString());
                    resp += "%0A" + val.ToString() + ". " + Name + " Amt: " + FormatAmt(theBillamt) + " Fee : " + FormatAmt(theBillcharge);

                    InsertPmtItems(req.SessionID, req.Msisdn, Name, Billerid, billercategoryid, theBillamt, theBillcharge, val);
                    Thread.Sleep(100);
                }
            }
            else
            {
                resp = "An Internal error has occured at this time.  Please Dial *822# to retry again";
            }
        }
        else
        {
            resp = "An Internal error has occured at this time.  Please Dial *822# to retry again";
        }
        return "Select Item to Pay for: " + resp;
    }
   //*******************************************************************************************************************************************************//

    public string DisplaySummary(UReq req)
    {
        string resp = ""; decimal amt = 0; decimal fee = 0;
        string prms = getParams(req); string details = "";
        NameValueCollection prm = splitParam(prms);
        details = getSessionDetails(req.SessionID);
        char[] sep = { '*' };
        string[] bits = details.Split(sep, StringSplitOptions.RemoveEmptyEntries);

        //amt = decimal.Parse(prm["amt"]);
        //fee = decimal.Parse(prm["Fee"]);
        //amt = decimal.Parse(bits[2]);
        //if(amt == 0)
        //{
        //    req.next_op = 71012;
        //    req.sub_op = 0;
        //    return "Enter amount";
        //}
        SaveAmt(req, bits[2]);
        SaveFee(req, bits[3]);
        SaveBillID(req, bits[5]);
        resp = "Your account number " + bits[1] + " will be debited with Amt " + FormatAmt(decimal.Parse(bits[2])) + " fee charge " + FormatAmt(decimal.Parse(bits[3])) + " for " + bits[4] + "%0A1.Yes%0A2.No";
        return "Confirmation: " + resp;
    }
    public string SavePayItemSelectedID(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("PayItemSelectedID", req.Msg, req);
            
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveSubscribNum(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SubscribNum", req.Msg, req);
            //string prms = getParams(req); string details = "";
            //NameValueCollection prm = splitParam(prms);
            //details = getSessionDetails(req.SessionID);
            //char[] sep = { '*' };
            //string[] bits = details.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            //decimal amt = decimal.Parse(bits[2]);
            //if (amt == 0)
            //{
            //    SaveFee(req, bits[3]);
            //    SaveBillID(req, bits[5]);
            //    //req.next_op = 71012;
            //}
            //else
            //{
            //    SaveAmt(req, bits[2]);
            //    SaveFee(req, bits[3]);
            //    SaveBillID(req, bits[5]);
            //}
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveItemRef(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("ItemRef", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveBouquet(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SELECTEDID", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveBillID(UReq req, string billid)
    {
        string resp = "0";
        try
        {
            addParam("billid", billid, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveFee(UReq req, string fee)
    {
        string resp = "0";
        try
        {
            addParam("Fee", fee, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveAmt(UReq req, string amt)
    {
        string resp = "0";
        try
        {
            addParam("amt", amt, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SavePin(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("PIN", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
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
    public string DoSubmitRec(UReq req)
    {

        string resp = "";
        int k = 0; int summary = 0;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 1)
            {
                Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
                cn.SetProcedure("spd_InsertBillPmtReq");
                cn.AddParam("@sessionid", req.SessionID);
                cn.AddParam("@cus_selected", int.Parse(prm["PayItemSelectedID"]));
                cn.AddParam("@mobile", req.Msisdn);
                cn.AddParam("@CuAuth", int.Parse(prm["PIN"]));
                cn.AddParam("@amt", decimal.Parse(prm["amt"]));
                cn.AddParam("@fee", decimal.Parse(prm["Fee"]));
                cn.AddParam("@billid", int.Parse(prm["billid"]));
                cn.AddParam("@CustomerConfirmation", summary);
                cn.AddParam("@SubscribNum", prm["SubscribNum"]);
                cn.AddParam("@Channelid", 11);
                k = Convert.ToInt32(cn.ExecuteProc());
            }
            else
            {
                return "Your request has been terminated.  Kindly dial *822# to try again";
            }
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