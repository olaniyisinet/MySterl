using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AccountOpen
/// </summary>
public class AccountOpen :BaseEngine
{
	public AccountOpen()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    //save the original request that came in
    public string SaveRequest(UReq req)
    {
        //*822*0#
        string resp = "0";
        try
        {
            //char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            //string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            addParam("REQ", msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveFirstName(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("FNAME", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveLastName(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("LNAME", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveGender(UReq req)
    {
        string resp = "0";
        try
        {
            if (Convert.ToInt16(req.Msg) > 2)
            {
                req.next_op = req.op - 1;
                return resp;
            }
            addParam("GENDER", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveDOFB(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("DOB", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    private string getGenderDesc(int gend)
    {
        string val = "";
        if(gend == 1)
        {
            val = "Male";
        }
        else
        {
            val = "Female";
        }
        return val;
    }
    public string DisplaySummary(UReq req)
    {
        string resp = ""; string fname = ""; string lname = ""; int gender = 0; DateTime dob;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string SessionID = req.SessionID; string Msisdn = req.Msisdn;
        fname = prm["FNAME"];
        lname = prm["LNAME"];
        gender = int.Parse(prm["GENDER"]);
        dob = Convert.ToDateTime(prm["DOB"]);
        resp = fname + " " + lname + ", " + getGenderDesc(gender) + ", " + dob.ToString("dd-MM-yyyy") + "%0A1.Yes%0A2.No";
        return "Proceed with account creation for " + resp;
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
    public string SubmitRec(UReq req)
    {
        string resp = ""; string fname = ""; string lname = ""; int gender = 0; DateTime dob; int k = 0; int summary = 0;
        string BVN = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            string SessionID = req.SessionID; string Msisdn = req.Msisdn;
            fname = prm["FNAME"];
            lname = prm["LNAME"];
            gender = int.Parse(prm["GENDER"]);
            dob = Convert.ToDateTime(prm["DOB"]);
            summary = int.Parse(prm["SUMMARY"]);
            BVN = prm["BVN"];
            if (summary == 2)
            {
                resp = "Dear " + fname + " " +lname +", You have cancelled the account creation process. Dial *822*0# to try again";
                return resp;
            }
            int prod = 5;//kia kia
            string sql = "insert into tbl_USSD_acct_open(Sessionid,mobile,Product,firstname,lastname, " +
            "Gender,DateOfBirth,BVN ) " +
                " values (@sid,@mb,@pd,@fn,@ln,@gd,@dob,@BVN)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@sid", SessionID);
            cn.AddParam("@mb", Msisdn);
            cn.AddParam("@pd", prod);
            cn.AddParam("@fn", fname.ToUpper());
            cn.AddParam("@ln", lname.ToUpper());
            cn.AddParam("@gd", gender);
            cn.AddParam("@dob", dob);
            cn.AddParam("@BVN", BVN);
            k = Convert.ToInt32(cn.Insert());
            if (k > 0)
            {
                resp = "Sterling Bank %0AThe account opening request has been submitted for processing. Your account will be sent to you via SMS";
            }
            else
            {
                resp = "Unable to submit at this time please try again";
            }

        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system  " + ex.ToString();
        }

        return resp;
    }


    //**********************************  Modified by Chigozie Anyasor on 13th Feb 2017 ***************
    public string CollectBVN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your BVN. You can type 1 to continue if you do not have BVN.";
            return "Bank Verification Number: " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of BVN is not less than or greater than 11 digits. You can type 1 to continue if you do not have BVN.";
            return "Bank Verification Number: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 11 digits and not alphanumeric. You can type 1 to continue if you do not have BVN.";
            return "Bank Verification Number: " + resp;
        }
        resp = "Kindly enter your BVN";
        return "Bank Verification Number: " + resp;
    }
    public string SaveBVN(UReq req)
    {
        int cnt = 0; string resp = "0";
        if (req.Msg == "1")
        {
            //check if entry is numeric
            try
            {
                long BVN = long.Parse(req.Msg);
            }
            catch
            {
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 721;
                return "9";
            }
            
            try
            {
                addParam("BVN", req.Msg, req);

            }
            catch
            {
                resp = "0";
            }
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        else if (req.Msg.Length < 11 || req.Msg.Length > 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 721;
            return "9";
        }
        //check if entry is numeric
        try
        {
            long BVN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 721;
            return "9";
        }
        try
        {
            addParam("BVN", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    //also remember to move in the submtmethod as well
    //*****************************************************************************************************

}