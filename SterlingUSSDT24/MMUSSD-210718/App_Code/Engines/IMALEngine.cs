using BVN;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VTU.ObjectInfo;

public class IMALEngine : BaseEngine
{
    
    public string DoGetBVN(UReq req)
    {
        //*822*4*BVN#
        string resp = "";
        try
        {
            char[] sep = { '*', '#' };
            string[] bits = req.Msg.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string bvn_val = bits[2];
            string mob = req.Msisdn;
            resp = BVN_DAL.DoImal(bvn_val, mob);
        }
        catch
        {
            //resp = ex.Message;
            resp = "ERROR: Request string not supported!%0AFor Sterling Bank customer dial *822*3*BVN#%0AFor NIB customer dial *822*3*BVN#";
        }
        return resp;
    }

    public string GetImalDetailsByNuban11(string nuban)
    {
        string val = "";
        string sql = @"select description as mobile,CURRENCY_CODE as cur_code from imal.amf where ADDITIONAL_REFERENCE ='" + nuban + "'";
        Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
        c.SetSQL(sql);
        try
        {
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                val = dr["mobile"].ToString() + "*" + dr["cur_code"].ToString();
            }
            else
            {
                val = "";
            }
        }
        catch (Exception ex)
        {

        }
        return val;
    }

    public string DoImalReg(string mob, string nuban, string pin,int bypass)
    {
        string resp = ""; string[] bits = null; string mobNum = ""; string curCode = ""; RegCustomers rg = new RegCustomers();
        //string details = GetImalDetailsByNuban(nuban);
        Gadget gg = new Gadget();
        //bits = details.Split('*');
        //mobNum = bits[0];
        //curCode = bits[1];
        ImalDetails imalinfo = GetImalDetailsByNuban(nuban);
        curCode = imalinfo.CurrencyCode;
        var isMobileValid = validateMobileAndNuban(nuban, mob);
        if (!isMobileValid)
        {
            return resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
        }

        if (curCode != "566")
        {
            return resp = "The specified account is not allowed for this service.%0AEnter a valid naira account!";
        }

        //if (mobNum.Length > 11)
        //{
        //    return resp = "Your mobile number is not correct. %0AKindly approach the nearest sterling bank to update your profile. " + mobNum;
        //}
        //mobNum = gg.ConvertMobile234(mobNum.Trim());
        //if (mobNum.Trim() != mob.Trim())
        //{
        //    return resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
        //}

        if (curCode == "566")
        {
            Go_Registered_Account g = new Go_Registered_Account();
            g.Mobile = mob;
            g.NUBAN = nuban;
            g.StatusFlag = "1";
            g.TransactionRefID = DateTime.Now.Ticks.ToString();
            g.RefID = "1986";//identify imal
            int retval = rg.Activate(g, pin, bypass);
            switch (retval)
            {
                case -99:
                    resp = "All connections are currently in use%0A Kindly try again!";
                    break;
                case -1:
                    resp = "You have already been activated for this service%0ADial *822*Amount# to recharge your phone.%0Ae.g *822*500# ";
                    break;
                case 1:
                    //resp = "Congratulations!%0AYou can now proceed with your transaction.";
                    resp = "Great! You are successfully registered for Sterling USSD Service. Please dial *822# to experience the magic…";

                    break;
            }
        }
        else // Non Naira Account 
        {
            // Non Naira account is not allowed for this service
            resp = "The specified account is not allowed for this service.%0AEnter a valid naira account!";
        }
        return resp;
    }

    public string getImalBal(string nuban)
    {
        string rsp = ""; string[] bits = null;
        //first check if the currency is allowed
        //string getcur = GetImalDetailsByNuban(nuban);
        ImalDetails imalinfo = GetImalDetailsByNuban(nuban);
        var curCode = imalinfo.CurrencyCode;
        //bits = getcur.Split('*');
        if (curCode == "566")
        {
            //call the imal balance procedure
        }
        else
        {
            return "Transaction not allowed on ussd";
        }
        return rsp;
    }

    //<CurrencyCode,CustomerName,CustBaseID,AvailableBalance,LedgerCode,AcctStatus
    public ImalDetails GetImalDetailsByNuban(string nuban)
    {
        string sql = @"select * from imal.amf where ADDITIONAL_REFERENCE ='" + nuban + "'";
        Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
        c.SetSQL(sql);
        try
        {
            var ds = c.Select();
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                var customerNumber = dr["cif_sub_no"].ToString();

                string sqlBvn = @"select ADD_STRING1 from imal.cif where cif_no = '" + customerNumber + "'";
                Sterling.Oracle.Connect c2 = new Sterling.Oracle.Connect("conn_imal");
                c2.SetSQL(sqlBvn);
                var ds2 = c2.Select();
                var Bvn = ds2.Tables[0].Rows[0]["ADD_STRING1"].ToString();
                string custBal = GetRealImalBalance(dr["cv_avail_bal"].ToString().Trim());
                return new ImalDetails
                {
                    Status = 1,                    
                    BVN = Bvn,
                    CurrencyCode = dr["currency_code"].ToString(),
                    CustomerName = dr["long_name_eng"].ToString(),
                    CustId = customerNumber,
                    AvailBal = Convert.ToDouble(custBal),
                    LedgerCode = dr["gl_code"].ToString(),
                    AcctStatus = dr["status"].ToString()
                };
            }

            return new ImalDetails
            {
                Status = 0                
            };
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return new ImalDetails
            {
                Status = -1,
                CurrencyCode = "",
                CustomerName = "",
                CustId = "",
                AvailBal = 0,
                LedgerCode = "",
                AcctStatus = ""
            };
        }
    }

    public bool validateMobileAndNuban(string nuban, string mobileNo)
    {
        bool resp = false;
        string sqlMob = @"SELECT CAD.TEL as mob1, CAD.MOBILE as mob2, CIF.TEL as mob3 FROM IMAL.AMF" +
                   " LEFT OUTER JOIN IMAL.CIF CIF ON AMF.CIF_SUB_NO = CIF.CIF_NO" +
                   " LEFT OUTER JOIN IMAL.CIF_ADDRESS CAD ON AMF.CIF_SUB_NO = CAD.CIF_NO AND CAD.DEFAULT_ADD = 1" +
                   " WHERE AMF.ADDITIONAL_REFERENCE = '" + nuban + "'";
        Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
        c.SetSQL(sqlMob);
        var ds = c.Select();
        var mob1 = ConvertMobile234(ds.Tables[0].Rows[0]["mob1"].ToString());
        var mob2 = ConvertMobile234(ds.Tables[0].Rows[0]["mob2"].ToString());
        var mob3 = ConvertMobile234(ds.Tables[0].Rows[0]["mob3"].ToString());

        if (mobileNo == mob1 || mobileNo == mob2 || mobileNo == mob3)
        {
            resp = true;
        }
        return resp;
    }

    public string GetRealImalBalance(string AcctBalance)
    {
        string resp = "";
        if (AcctBalance.StartsWith("-"))
        {            
            resp = AcctBalance.Remove(0, 1);
        }
        else
        {
            AcctBalance = "-" + AcctBalance;
            resp = AcctBalance;
        }
        return resp;
    }
}

