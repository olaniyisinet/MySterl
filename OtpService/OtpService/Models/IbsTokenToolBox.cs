using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace OtpService.Models
{
    public class IbsTokenToolBox
    {
        public int SendTokenCustId(string UserIDD, string mail, string phonenumber)
        {
            try
            {
                //this service actually receives mail and phonenum
                string UserID = UserIDD;
                string phonenum = phonenumber;
                int status = 0;
                //fetch the nuban and mail from the db using custID and LoginID
                //send the token to the number and mail
                //log the token to a table
                //send a response
                string subject = ConfigurationManager.AppSettings["subject"].ToString();
                string sourceEmail = ConfigurationManager.AppSettings["sourceEmail"].ToString();
                new ErrorLog("New token request for:: " + UserID + " and phonenumber::" + phonenumber);
                //generate the OTP
                Gadget g = new Gadget();
                string otp = g.newOTP();

                //send to InfoBip 
                Utility u = new Utility();

                // int cnt = u.insertIntoInfobip("Your One-Time Password is: " + otp, phonenum);
                int cnt = u.insertIntoInfobip("Dear Customer, your One-time Password from Sterling internet banking is: " + otp + " This OTP is valid for 5 minutes.", phonenum, UserIDD);
                if (cnt > 0)
                {
                    InsertRecord(UserID, otp);
                    SendOtpMail sendOtpMail = new SendOtpMail();
                    sendOtpMail.SendOtpViaMail(mail, sourceEmail, otp, subject);
                    new ErrorLog("mail sent to::" + mail);
                    status = 1;
                }
                else
                {
                    if (mail == "")
                    {
                        //mail is empty
                        //mobile number is empty in T24
                        status = 3;
                    }
                    else
                    {
                        SendOtpMail sendOtpMail = new SendOtpMail();
                        sendOtpMail.SendOtpViaMail(mail, sourceEmail, otp, subject);
                        new ErrorLog("Not able to insert into infobip. So send mail only::" + mail);
                        status = 2;
                    }
                }

                new ErrorLog(status.ToString() + '*' + otp + '*' + phonenumber + " Send Token result for mail" + mail);
                return status;
            }
            catch (Exception ex)
            {
                new ErrorLog(" Error while Sending Token:: " + ex.ToString());
                return 0;
            }
        }
        public void InsertRecord(string nuban, string otp)
        {
            try
            {

                string sql = "insert into tbl_otp(nuban,otp) values(@nuban,@otp)";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
                c.SetSQL(sql);
                c.AddParam("@nuban", nuban);
                c.AddParam("@otp", otp);
                int resp = c.Execute();
                new ErrorLog("inserting OTP response:: " + resp.ToString());
            }
            catch (Exception ex)
            {
                new ErrorLog("Error while inserting OTP::" + ex.ToString());
            }
        }

        public int ValidateSentTokenCustId(string LoginID, Int32 AppID, string Token)
        {
            //get the token from the table using the custDI and token
            //if it exists, check the time difference for validity
            //send a success/response message based on it's validity
            int minutes = Int16.Parse(ConfigurationManager.AppSettings["FiveMinutes"].ToString());
            string otp = "";
            otp = Token;
            int active = 0;
            new ErrorLog("IBS OTP submitted for verification with appid " + AppID.ToString() + " phonenumber " + LoginID + " with otp " + otp);
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
            DataSet ds = new DataSet(); DateTime dt = new DateTime();
            string phonenumber = LoginID; //this is the userID ..that is what I am using to validate
            try
            {
                string sql = "select refid,otp,dateadded,statusflag from tbl_otp where statusflag=0 and nuban=@nuban and otp=@otp";
                dt = new DateTime();
                c.SetSQL(sql);
                c.AddParam("@nuban", phonenumber);
                c.AddParam("@otp", otp);
                ds = c.Select("rec");
            }
            catch (Exception ex)
            {
                new ErrorLog("An exception occured while query the db. IBS OTP submitted for verification with appid " + AppID.ToString() + " phonenumber:: " + phonenumber + " with otp " + otp + ". The error is as follows: " + ex);
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                //token does not exist
                return 3;
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
                UpdateRecord(phonenumber, otp);
                new ErrorLog("IBS Otp Validated successfully for phonenumber " + phonenumber + ", otp " + otp + ". The time lasted is " + finalval + " minutes. Status " + active);
            }
            else
            {
                active = 2;
                new ErrorLog("IBS Otp Validated failed for phonenumber " + phonenumber + ", otp " + otp + ". The time lasted is " + finalval + "minutes. Status " + active);
            }

            new ErrorLog("Status of IBS OTP is " + active);

            return active;
        }
        public void UpdateRecord(string nuban, string otp)
        {
            try
            {

                string sql = "update tbl_otp set statusflag=1 where nuban =@nuban and otp =@otp and statusflag=0";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
                c.SetSQL(sql);
                c.AddParam("@nuban", nuban);
                c.AddParam("@otp", otp);
                c.Execute();
            }
            catch (Exception ex)
            {
                new ErrorLog("Error while updating OTP::" + ex.ToString());
            }
        }
    }
}