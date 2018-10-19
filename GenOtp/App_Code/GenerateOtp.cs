using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

/// <summary>
/// Summary description for GenerateOtp
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GenerateOtp : System.Web.Services.WebService
{

    [WebMethod]
    public int doGenerateOtp(string nuban, Int32 Appid)
    {
        EACBS.banks ws = new EACBS.banks();
        //new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban);
        int status = 0; string mobnum = ""; string EMAIL = "";
        DataSet ds = ws.getAccountFullInfo(nuban);
        if (ds.Tables[0].Rows.Count > 0)
        {
            //get the mobnum into the values
            DataRow dr = ds.Tables[0].Rows[0];
            mobnum = dr["MOB_NUM"].ToString();
            EMAIL = dr["EMAIL"].ToString();
            mobnum = mobnum.Trim();
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

        int cnt = u.insertIntoInfobip(otp, mobnum);
        if (cnt > 0)
        {
            InsertRecord(nuban, otp);
            try
            {
                if (Appid == 51 || Appid == 55)
                {
                    SendOtpMail sendOtpMail = new SendOtpMail();
                    sendOtpMail.SendOtpViaMail(EMAIL, "Switch@Sterlingbankng.com", otp, "Switch Portal");
                }
                else if (Appid == 1)
                {
                    SendOtpMail sendOtpMail = new SendOtpMail();
                    sendOtpMail.SendOtpViaMail(EMAIL, "Biobank@Sterlingbankng.com", otp, "BioBank OTP");
                }
            }
            catch (Exception ex)
            {

            }
            status = 1;
        }
        else
        {
            status = 2;
        }
        return status;
    }
    [WebMethod]
    public int sendSms(string msg, string mobnum, Int32 Appid)
    {
        //send the OTP to the customer through infobip
        Utility u = new Utility();

        int cnt = u.insertIntoInfobip(msg, mobnum);
        if (cnt > 0) return 1;
        return -1;
    }

    //This method is called by Specta
    [WebMethod]
    public string doGenerateOtpAndMail(string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject)
    {

        EACBS.banks ws = new EACBS.banks(); string rspval = "";
        new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban);
        int status = 0; string mobnum = "";
        DataSet ds = ws.getAccountFullInfo(nuban);
        if (ds.Tables[0].Rows.Count > 0)
        {
            //get the mobnum into the values
            DataRow dr = ds.Tables[0].Rows[0];
            mobnum = dr["MOB_NUM"].ToString();
            mobnum = mobnum.Trim();
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

        int cnt = u.insertIntoInfobipSPECTA(otp, mobnum);
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
        return status.ToString() + '*' + otp + '*' + mobnum;
    }
    [WebMethod]
    public int SendTextToPhoneNumber(string message, string phoneNumber)
    {
        Utility u = new Utility();
        int cnt = u.insertIntoInfobipSPECTA(message, phoneNumber);
        int status = 0;
        if (cnt > 0)
        {

            status = 1;
        }
        else
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                //mobile number is empty in T24
                status = 3;
            }
            else
            {
                status = 2;
            }
        }
        return status;
    }
    [WebMethod]
    public string doGenerateOtpAndMailByPhoneNumber(string phoneNumber, string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject)
    {

        EACBS.banks ws = new EACBS.banks(); string rspval = "";

        var mobnum = phoneNumber.Trim();


        string otp = "";
        //generate the otp
        Gadget g = new Gadget();
        otp = g.newOTP();
        int status = 0;
        //send the OTP to the customer through infobip
        Utility u = new Utility();

        int cnt = u.insertIntoInfobipSPECTA(otp, mobnum);
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
        return status.ToString() + '*' + otp + '*' + mobnum;
    }

    [WebMethod]
    public string doGenerateOtpAndMailByPhoneNumberWithMailBody(string phoneNumber, string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject, string mailBody)
    {
        EACBS.banks ws = new EACBS.banks(); string rspval = "";
        var mobnum = phoneNumber.Trim();
        string otp = "";
        //generate the otp
        Gadget g = new Gadget();
        otp = g.newOTP();
        int status = 0;
        //send the OTP to the customer through infobip
        Utility u = new Utility();

        int cnt = u.insertIntoInfobipSPECTA(otp, mobnum);
        if (cnt > 0)
        {
            InsertRecord(nuban, otp);
            SendOtpMail sendOtpMail = new SendOtpMail();
            sendOtpMail.SendOtpViaMailNew(destinationEmail, sourceEmail, mailBody + " " + otp, subject);
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
        return status.ToString() + '*' + otp + '*' + mobnum;
    }





    [WebMethod]
    public int verifyOtp(string nuban, string otp, Int32 Appid)
    {
        new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban + " with otp " + otp);
        int active = 0;
        string sql = "select refid,otp,dateadded,statusflag from tbl_otp where statusflag=0 and nuban=@nuban and otp=@otp";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        DateTime dt = new DateTime();
        c.SetSQL(sql);
        c.AddParam("@nuban", nuban);
        c.AddParam("@otp", otp);
        DataSet ds = c.Select("rec");
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
        long timestamp = (long)t.TotalSeconds;


        TimeSpan t2 = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
        long timestamp2 = (long)t2.TotalSeconds;

        long finalval = timestamp - timestamp2;

        if (finalval <= 3600)
        {
            active = 1;
            UpdateRecord(nuban, otp);
        }
        else
        {
            active = 2;
            new ErrorLog("Expired OTP for" + nuban + " the otp was" + otp);
        }
        return active;
    }

    [WebMethod]
    public int verifyOtpHigherDelay(string nuban, string otp, Int32 Appid, int minutes)
    {
        new ErrorLog("OTP submitted for verification with appid " + Appid.ToString() + " nuban " + nuban + " with otp " + otp);
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

            new ErrorLog("An exception occured while query the db. OTP submitted for verification with appid " + Appid.ToString() + " nuban " + nuban + " with otp " + otp + ". The error is as follows: " + ex);

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
            new ErrorLog("Otp Validated successfully for Nuban " + nuban + ", otp " + otp + ". The time lasted is " + finalval + " minutes. Status " + active);

        }
        else
        {
            active = 2;
            new ErrorLog("Otp Validated failed for Nuban " + nuban + ", otp " + otp + ". The time lasted is " + finalval + "minutes. Status " + active);

        }
        new ErrorLog("Status of OTP is " + active);

        return active;
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

    public void UpdateRecord(string nuban, string otp)
    {
        string sql = "update tbl_otp set statusflag=1 where nuban =@nuban and otp =@otp and statusflag=0";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@nuban", nuban);
        c.AddParam("@otp", otp);
        c.Execute();
    }

    public void InsertRecordByMob(string mobile, string otp)
    {
        string sql = "insert into tbl_otpByMobile(mobile, otp) values(@mobile, @otp)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@mobile", mobile);
        c.AddParam("@otp", otp);
        c.Execute();
        //try
        //{
        //    SendOtpMail sendOtpMail = new SendOtpMail();
        //    sendOtpMail.SendOtpViaMail(EMAIL, "Switch@Sterlingbankng.com", otp, "Switch Portal");
        //}
        //catch(Exception ex)
        //{

        //}
    }

    [WebMethod]
    public int doGenerateOtpByMobile(string msg, string mobnum, Int32 Appid)
    {
        //new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban);
        int status = 0;


        string otp = "";
        //generate the otp
        Gadget g = new Gadget();
        otp = g.newOTP();

        //send the OTP to the customer through infobip
        Utility u = new Utility();

        int cnt = u.insertIntoInfobip(msg + otp, mobnum);  // i need to disable this method for now, since i am still developing. will be reenabled after building
        if (cnt > 0)
        {
            InsertRecordByMob(mobnum, otp);
            status = 1;
        }
        else
        {
            status = 2;
        }
        return status;
    }

    [WebMethod]
    public int verifyOtpByMobile(string mobile, string otp, Int32 Appid)
    {
        //new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban + " with otp " + otp);
        int active = 0;
        string sql = "select refid,otp,dateadded,statusflag from tbl_otpByMobile where statusflag=0 and mobile=@mobile and otp=@otp";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        DateTime dt = new DateTime();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobile);
        c.AddParam("@otp", otp);
        DataSet ds = c.Select("rec");
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
        long timestamp = (long)t.TotalSeconds;


        TimeSpan t2 = (dt - new DateTime(1970, 1, 1));
        long timestamp2 = (long)t2.TotalSeconds;

        long finalval = timestamp - timestamp2;

        if (finalval <= 120)
        {
            active = 1;
            UpdateRecordOtpByMobile(mobile, otp);
        }
        else
        {
            active = 2;
        }
        return active;
    }

    [WebMethod]
    public string doGenerateOtpToMail(Int32 Appid, string destinationEmail, string sourceEmail, string subject, string mailBody)
    {

        string otp = "";
        //generate the otp
        Gadget g = new Gadget();
        otp = g.newOTP();
        int status = 0;
        try
        {
            InsertMailRecord(destinationEmail, otp);

            SendOtpMail sendOtpMail = new SendOtpMail();
            sendOtpMail.SendOtpViaMailNew(destinationEmail, sourceEmail, mailBody + " " + otp, subject);
            status = 1;
        }
        catch(Exception e)
        {
            new ErrorLog("An exception occured while sending OTP to mail " + Appid.ToString() + ". The error is as follows: " + e);

        }


        return status.ToString();
    }


    [WebMethod]
    public int verifyOtpToMail(string mail, string otp, Int32 Appid, int minutes)
    {

        DateTime span = DateTime.Now;
        new ErrorLog("OTP submitted for verification with appid " + Appid.ToString() + " Email " + mail + " with otp " + otp);
        int active = 0;
        string sql = "select * from tbl_OtpByMail where Status=0 and Email=@mail and Otp=@otp";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        DataSet ds = new DataSet(); DateTime dt = new DateTime();
        try
        {
            dt = new DateTime();
            c.SetSQL(sql);
            c.AddParam("@mail", mail);
            c.AddParam("@otp", otp);
            ds = c.Select("rec");
        }
        catch (Exception ex)
        {

            new ErrorLog("An exception occured while querying the db. OTP submitted for verification with appid " + Appid.ToString() + " mail " + mail + " with otp " + otp + ". The error is as follows: " + ex);

        }

        


        var tb = ds.Tables[0];

        DateTime otpTime = Convert.ToDateTime(tb.Rows[0]["Date"]);

        int diff = (span - otpTime).Minutes;

        if(diff < minutes)
        {
            active = 1;
            UpdateMailRecord(mail, otp, active);
            new ErrorLog("Otp Validated successfully for Email " + mail + ", otp " + otp + ". Status " + active);

        }
        else
        {
            active = 2;
            UpdateMailRecord(mail, otp, active);

            new ErrorLog("Otp Validated failed for Email " + mail + ", otp " + otp + ". Status " + active);

        }

        //var query = from d in tb.AsEnumerable()
        //            select new
        //            {
        //                Datval = d.Field<DateTime>("Date")
        //            };
        //if (query == null)
        //{
        //    //date is not found

        //    return 3;
        //}

        //foreach (var q in query)
        //{
        //    dt = q.Datval;
        //}

        //if (dt.ToString() == "1/1/0001 12:00:00 AM")
        //{
        //    //date is used
        //    return 4;
        //}

        //TimeSpan t = (DateTime.Now - new DateTime(1970, 1, 1));
        //long timestamp = (long)t.TotalMinutes;


        //TimeSpan t2 = (dt - new DateTime(1970, 1, 1));
        //long timestamp2 = (long)t2.TotalMinutes;

        //long finalval = timestamp - timestamp2;

        //if (finalval <= minutes)
        //{
        //    active = 1;
        //    UpdateMailRecord(mail, otp, active);
        //    new ErrorLog("Otp Validated successfully for Nuban " + mail + ", otp " + otp + ". The time lasted is " + finalval + " minutes. Status " + active);

        //}
        //else
        //{
        //    active = 2;
        //    UpdateMailRecord(mail, otp, active);

        //    new ErrorLog("Otp Validated failed for Nuban " + mail + ", otp " + otp + ". The time lasted is " + finalval + "minutes. Status " + active);

        //}
        new ErrorLog("Status of OTP is " + active);

        return active;
    }

    public void UpdateRecordOtpByMobile(string mobile, string otp)
    {
        string sql = "update tbl_otpByMobile set statusflag=1 where mobile =@mobile and otp =@otp and statusflag=0";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@mobile", mobile);
        c.AddParam("@otp", otp);
        c.Execute();
    }

    public void InsertMailRecord(string mail, string otp)
    {
        string sql = "insert into tbl_OtpByMail(Email,Otp,Status,Date) values(@mail,@otp,@status,@date)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@mail", mail);
        c.AddParam("@otp", otp);
        c.AddParam("@status", 0);
        c.AddParam("@date", DateTime.Now);
        c.Execute();
    }

    public void UpdateMailRecord(string mail, string otp, int status)
    {
        string sql = "update tbl_OtpByMail set status=@status where Email=@mail and otp=@otp";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
        c.SetSQL(sql);
        c.AddParam("@mail", mail);
        c.AddParam("@otp", otp);
        c.AddParam("@status", status);
        c.Execute();
    }
}
