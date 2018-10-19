using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for OtpService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class OtpService : System.Web.Services.WebService
{

    [WebMethod]
    public int GenSoftToken(string nuban, Int32 Appid)
    {
        string subject = ConfigurationManager.AppSettings["subject"].ToString();
        string sourceEmail = ConfigurationManager.AppSettings["sourceEmail"].ToString();
        EACBS.banks ws = new EACBS.banks();
        string destinationEmail = "";
        new ErrorLog("New token details received via into (OtpService.asmx) channel with appid " + Appid.ToString() + " nuban " + nuban);
        int status = 0; string mobnum = "";
        DataSet ds = ws.getAccountFullInfo(nuban);
        if (ds.Tables[0].Rows.Count > 0)
        {
            //get the mobnum into the values
            DataRow dr = ds.Tables[0].Rows[0];
            mobnum = dr["MOB_NUM"].ToString();
            mobnum = mobnum.Trim();
            destinationEmail = dr["Email"].ToString();
        }
        else
        {

        }

        string otp = "";
        //generate the otp
        Gadget g = new Gadget();
        otp = g.newOTP();

        //send the OTP to the customer through infobip
        Utility u = new Utility();
        int cnt = u.insertIntoInfobip("Your One-Time Password is: " + otp, mobnum);
        if (cnt > 0)
        {
            InsertRecord(nuban, otp);
            SendOtpMail sendOtpMail = new SendOtpMail();
            sendOtpMail.SendOtpViaMail(destinationEmail, sourceEmail, otp, subject);
            status = 1;
        }
        else
        {
            if (mobnum == "")
            {
                //mobile number is empty in T24
                status = 3;
            }
            else
            {
                status = 2;
            }
        }
        new ErrorLog(status.ToString() + '*' + otp + '*' + mobnum);
        return status;// status.ToString() + '*' + otp + '*' + mobnum;
    }
    [WebMethod]
    public int ValidateSoftToken(string nuban, string Token, Int32 Appid)
    {
        string otp = "";
        otp = Token;
        int minutes = Int16.Parse(ConfigurationManager.AppSettings["FiveMinutes"].ToString());
        new ErrorLog("IBS OTP submitted for verification with appid " + Appid.ToString() + " nuban " + nuban + " with otp " + otp);
        int active = 0;
        string sql = "select refid,otp,dateadded,statusflag from tbl_otp where statusflag=0 and nuban=@nuban and otp=@otp";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        DataSet ds = new DataSet(); DateTime dt = new DateTime();
        try
        {
            dt = new DateTime();
            c.SetSQL(sql);
            c.AddParam("@nuban", nuban);
            c.AddParam("@otp", otp);
            ds = c.Select("rec");
        }
        catch (Exception ex)
        {
            new ErrorLog("An exception occured while query the db. IBS OTP submitted for verification with appid " + Appid.ToString() + " nuban " + nuban + " with otp " + otp + ". The error is as follows: " + ex);
        }
        var tb = ds.Tables[0];
        var query = from d in tb.AsEnumerable()
                    select new
                    {
                        Datval = d.Field<DateTime>("dateadded")
                    };
        if (query == null)
        {
            //date is not found
            return 3;
        }

        foreach (var q in query)
        {
            dt = q.Datval;
        }

        if (dt.ToString() == "1/1/0001 12:00:00 AM")
        {
            //date is used
            return 4;
        }

        TimeSpan t = (DateTime.Now - new DateTime(1970, 1, 1));
        long timestamp = (long)t.TotalMinutes;


        TimeSpan t2 = (dt - new DateTime(1970, 1, 1));
        long timestamp2 = (long)t2.TotalMinutes;

        long finalval = timestamp - timestamp2;

        if (finalval <= minutes)
        {
            active = 1;
            UpdateRecord(nuban, otp);
            new ErrorLog("IBS Otp Validated successfully for Nuban " + nuban + ", otp " + otp + ". The time lasted is " + finalval + " minutes. Status " + active);
        }
        else
        {
            active = 2;
            new ErrorLog("IBS Otp Validated failed for Nuban " + nuban + ", otp " + otp + ". The time lasted is " + finalval + "minutes. Status " + active);
        }
        new ErrorLog("Status of IBS OTP is " + active);

        return active;
    }

    [WebMethod]
    public int ValidateSoftTokenByMobile(string mobile, string Token, Int32 Appid)
    {
        string otp = "";
        otp = Token;
        int minutes = Int16.Parse(ConfigurationManager.AppSettings["FiveMinutes"].ToString());
        new ErrorLog("IBS OTP submitted for verification with appid " + Appid.ToString() + " mobile " + mobile + " with otp " + otp);
        int active = 0;
        string sql = "select refid,otp,dateadded,statusflag from tbl_otpByMobile where statusflag=0 and mobile=@mobile and otp=@otp";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        DataSet ds = new DataSet(); DateTime dt = new DateTime();
        try
        {
            dt = new DateTime();
            c.SetSQL(sql);
            c.AddParam("@mobile", mobile);
            c.AddParam("@otp", otp);
            ds = c.Select("rec");
        }
        catch (Exception ex)
        {
            new ErrorLog("An exception occured while query the db. IBS OTP submitted for verification with appid " + Appid.ToString() + " mobile " + mobile + " with otp " + otp + ". The error is as follows: " + ex);
        }
        var tb = ds.Tables[0];
        var query = from d in tb.AsEnumerable()
                    select new
                    {
                        Datval = d.Field<DateTime>("dateadded")
                    };
        if (query == null)
        {
            //date is not found
            return 3;
        }

        foreach (var q in query)
        {
            dt = q.Datval;
        }

        if (dt.ToString() == "1/1/0001 12:00:00 AM")
        {
            //date is used
            return 4;
        }

        TimeSpan t = (DateTime.Now - new DateTime(1970, 1, 1));
        long timestamp = (long)t.TotalMinutes;


        TimeSpan t2 = (dt - new DateTime(1970, 1, 1));
        long timestamp2 = (long)t2.TotalMinutes;

        long finalval = timestamp - timestamp2;

        if (finalval <= minutes)
        {
            active = 1;
            UpdateRecordForMobile(mobile, otp);
            new ErrorLog("IBS Otp Validated successfully for mobile " + mobile + ", otp " + otp + ". The time lasted is " + finalval + " minutes. Status " + active);
        }
        else
        {
            active = 2;
            new ErrorLog("IBS Otp Validated failed for mobile " + mobile + ", otp " + otp + ". The time lasted is " + finalval + "minutes. Status " + active);
        }
        new ErrorLog("Status of IBS OTP is " + active);

        return active;
    }
    public void UpdateRecord(string nuban, string otp)
    {
        string sql = "update tbl_otp set statusflag=1 where nuban =@nuban and otp =@otp and statusflag=0";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@nuban", nuban);
        c.AddParam("@otp", otp);
        c.Execute();
    }
    public void UpdateRecordForMobile(string mobile, string otp)
    {
        string sql = "update tbl_otpByMobile set statusflag=1 where mobile =@mobile and otp =@otp and statusflag=0";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@mobile", mobile);
        c.AddParam("@otp", otp);
        c.Execute();
    }
    public void InsertRecord(string nuban, string otp)
    {
        string sql = "insert into tbl_otp(nuban,otp) values(@nuban,@otp)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@nuban", nuban);
        c.AddParam("@otp", otp);
        c.Execute();
    }
    public void InsertRecordMobile(string mobile, string otp)
    {
        string sql = "insert into tbl_otpByMobile(mobile,otp) values(@mobile,@otp)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@mobile", mobile);
        c.AddParam("@otp", otp);
        c.Execute();
    }
    [WebMethod]
    public int GenSoftTokenMobile(string MobileNum, Int32 Appid)
    {
        string subject = ConfigurationManager.AppSettings["subject"].ToString();
        string sourceEmail = ConfigurationManager.AppSettings["sourceEmail"].ToString();
        EACBS.banks ws = new EACBS.banks();
        string destinationEmail = "";
        new ErrorLog("New token details received via into (OtpService.asmx) channel with appid " + Appid.ToString() + " MobileNum " + MobileNum);
        int status = 0; string mobnum = "";
        mobnum = MobileNum;
        mobnum = mobnum.Trim();
        string otp = "";
        //generate the otp
        Gadget g = new Gadget();
        otp = g.newOTP();

        //send the OTP to the customer through infobip
        Utility u = new Utility();
        int cnt = u.insertIntoInfobip("Your One-Time Password is: " + otp, mobnum);
        if (cnt > 0)
        {
            InsertRecordMobile(MobileNum, otp);
            SendOtpMail sendOtpMail = new SendOtpMail();
            sendOtpMail.SendOtpViaMail(destinationEmail, sourceEmail, otp, subject);
            status = 1;
        }
        else
        {
            if (mobnum == "")
            {
                //mobile number is empty in T24
                status = 3;
            }
            else
            {
                status = 2;
            }
        }
        new ErrorLog(status.ToString() + '*' + otp + '*' + mobnum);
        return status;// status.ToString() + '*' + otp + '*' + mobnum;
    }
}
