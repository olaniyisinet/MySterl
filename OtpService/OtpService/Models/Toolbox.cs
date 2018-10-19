using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using BankCore;
using BankCore.t24;
using GenerateTransactionCodes;
using Newtonsoft.Json;
using OtpService.Models.Request;
using OtpService.Models.Response;
using RestSharp;

namespace OtpService.Models
{
    public class Toolbox
    {

        Sterling.MSSQL.Connect Dbcon = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");

        public string GenerateRndNumber(int cnt)
        {
            string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Random rand1 = new Random();
            string txt = "";
            for (int j = 0; j < cnt; j++)
                txt += key2[rand1.Next(0, 9)];
            return txt;
        }
        public bool ValidateDuplicateEnrollment(RegisterUser r)
        {
            try
            {
                string sql = " Select * from Users where Bvn=@bvn or Email=@email or Mobile=@Mobile or HandleUsername=@HandleUsername";
                Dbcon.SetSQL(sql);
                Dbcon.AddParam("@bvn", r.bvn);
                Dbcon.AddParam("@email", r.email);
                Dbcon.AddParam("@Mobile", r.mobile);
                Dbcon.AddParam("@HandleUsername", r.HandleUsername);
                DataSet ds = Dbcon.Select();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    //duplicate
                    return true;
                }
                else
                {
                    return false;
                  //  //check for mail duplicated
                  //  string sql2 = " Select * from Users where email=@email or ";
                  //  Dbcon.SetSQL(sql2);
                  ////  Dbcon.AddParam("@bvn", r.bvn);
                  //  Dbcon.AddParam("@email", r.email);
                  //  DataSet ds2 = Dbcon.Select();
                  //  if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                  //  {

                  //      return true;
                  //  }
                  //  else
                  //  {
                  //      return false;
                  //  }

                    //return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public int SaveUserenrollment(RegisterUser req)
        {
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
                string password = Encrypt(req.Password);
                c.SetProcedure("spd_CreatePayliteUser");
                c.AddParam("@FirstName", req.firstname);
                c.AddParam("@LastName", req.lastname);
                c.AddParam("@Email", req.email);
                c.AddParam("@Password", password); //encrypt password before savind
                c.AddParam("@Mobile", req.mobile);
                c.AddParam("@Bvn", req.bvn);
                c.AddParam("@Longitude", req.longitude);
                c.AddParam("@Latitude", req.latitude);
                c.AddParam("@HandleUsername", req.HandleUsername);
                var userID = c.ExecuteProc();
                int? cnt = c.returnValue;

                if (cnt == 1)
                {
                    //create wallet and move on using bvn
                    if (createwallet(req.firstname, req.lastname, "2", DateTime.Now.ToString(), req.email, "mr", req.bvn) == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {

                return -1;

            }

        }
        public bool Login(LoginUser req)
        {
            try
            {
                var password = Encrypt(req.password);
                string sql = " Select * from Users where Email=@Email and Password=@Password and StatusFlag=@StatusFlag";
                Dbcon.SetSQL(sql);
                Dbcon.AddParam("@Email", req.email);
                Dbcon.AddParam("@Password", password);
                Dbcon.AddParam("@StatusFlag", 0);
                DataSet ds = Dbcon.Select();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                return false;
            }

        }
        public bool ForgotPassword(ForgotPassword req)
        {
            //search and make the password field to zero
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
                string sql = @"update Users SET Password=@Password where Email=@Email";
                c.SetSQL(sql);
                c.AddParam("@email", req.email);
                c.AddParam("@Password", Encrypt(req.Password));
                var i = c.Execute();
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    new ErrorLog(c.errmsg.ToString());
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 73, 118, 97, 110, 32, 77, 101, 100, 118, 101, 100, 101, 118 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }
        public bool UpdateFxUser(UpdateUser req)
        {
            try
            {
                //[]
                //,[]
                //,[]
                //,[]
                //,[]
                //,[Password]
                //,[Longitude]
                //,[Latitude]
                //,[]
                //,[StatusFlag]

                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
                string sql = @"update Users SET Mobile = @Mobile, LastName=@LastName, FirstName=@FirstName, Email=@Email, HandleUsername=@HandleUsername where Bvn=@Bvn";
                c.SetSQL(sql);
                c.AddParam("@LastName", req.LastName);
                c.AddParam("@FirstName", req.FirstName);
                c.AddParam("@Mobile", req.Mobile);
                c.AddParam("@Email", req.Email);
                c.AddParam("@HandleUsername", req.HandleUsername);
                c.AddParam("@Bvn", req.Bvn);
                var i = c.Execute();
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    new ErrorLog(c.errmsg.ToString());
                    c.Close();
                    return false;
                }

            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                return false;
            }

        }

        public Tuple<bool, List<LoginResponse>> Loginuser(LoginUser req)
        {
            try
            {
                var password = Encrypt(req.password);


                string sql = " Select userid, firstname, lastname, email, pwd, mobile, bvn, dob,longitude, latitude, industryid, safe_zone_flag  from tbl_user_profile where email=@email and pwd=@pwd and statusflag =@statusflag";
                Dbcon.SetSQL(sql);
                Dbcon.AddParam("@email", req.email);
                Dbcon.AddParam("@pwd", password);
                Dbcon.AddParam("@statusflag", 1); //status flag "1" means the user is active
                DataSet ds = Dbcon.Select();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    //clean the data

                    var list = Helper.ConvertDataTable<LoginResponse>(ds.Tables[0]);


                    return Tuple.Create(true, list);
                }
                else
                {
                    List<LoginResponse> lt = new List<LoginResponse>();
                    return Tuple.Create(false, lt);
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                //DataSet dt = new DataSet();
                List<LoginResponse> lt = new List<LoginResponse>();
                return Tuple.Create(false, lt);
            }

        }

        public Tuple<bool, DataTable> GetFullWalletDetails(string bvn)
        {
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_SpayWallet");
                c.SetProcedure("spd_GetAccountByNuban");
                c.AddParam("@nuban", bvn);

                c.ExecuteProc();
                int? resp = c.returnValue;
                var walletDetails = c.Select("recs");
                if (resp == 0 && walletDetails != null)
                {
                    return Tuple.Create(true, walletDetails.Tables[0]);
                }
                else
                {
                    DataTable r = new DataTable();
                    return Tuple.Create(false, r);

                }
            }
            catch(Exception e)
            {
                DataTable r = new DataTable();
                return Tuple.Create(false, r);
            }
        }

        public Tuple<bool, DataTable> GetUserDetails(string bvn)
        {
            // int SECTOR = 1;
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
            c.SetProcedure("GetUserWalletDetails");
            c.AddParam("@bvn", bvn);

            c.ExecuteProc();
            int? resp = c.returnValue;
            var walletDetails = c.Select("recs");
            if (resp == 0 && walletDetails != null)
            {
                return Tuple.Create(true, walletDetails.Tables[0]);
            }
            else
            {
                DataTable r = new DataTable();
                return Tuple.Create(false, r);

            }
        }

        public Tuple<bool, DataTable> GetbeneficiaryBvn(string handle)
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
            c.SetProcedure("MapHandleToBvn");
            c.AddParam("@HandleName", handle);

            c.ExecuteProc();
            int? resp = c.returnValue;
            var walletDetails = c.Select("recs");
            if (resp == 0 && walletDetails != null)
            {
                return Tuple.Create(true, walletDetails.Tables[0]);
            }
            else
            {
                DataTable r = new DataTable();
                return Tuple.Create(false, r);

            }
        }

        public Tuple<bool, List<RegisterUser>> GetUserDetails(GetUser UserId)
        {
            try
            {
                // var password = Encrypt(req.password);


                string sql = " Select FirstName, LastName, Email, Mobile, Bvn, mobile, Password,Longitude, Latitude, HandleUsername from Users where Bvn=@Bvn and statusflag =@statusflag";
                Dbcon.SetSQL(sql);
                Dbcon.AddParam("@Bvn", UserId.Bvn);
                Dbcon.AddParam("@statusflag", 1); //status flag "1" means the user is active
                DataSet ds = Dbcon.Select();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    //clean the data
                    var list = Helper.ConvertDataTable<RegisterUser>(ds.Tables[0]);
                    return Tuple.Create(true, list);
                }
                else
                {
                    List<RegisterUser> lt = new List<RegisterUser>();
                    return Tuple.Create(false, lt);
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                //DataSet dt = new DataSet();
                List<RegisterUser> lt = new List<RegisterUser>();
                return Tuple.Create(false, lt);
            }
        }

        public Tuple<bool, DataTable> GetUserTrxns(GetUSerTrxnReq req)
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
            c.SetProcedure("GetUserTxn");
            c.AddParam("@SenderID", req.bvn);
            c.AddParam("@ToDate", req.FromDate); //latest date
            c.AddParam("@FromDate", req.ToDate); //closest date

            c.ExecuteProc();
            int? resp = c.returnValue;
            var walletDetails = c.Select("recs");
            if (resp == 0 && walletDetails != null)
            {
                return Tuple.Create(true, walletDetails.Tables[0]);
            }
            else
            {
                DataTable r = new DataTable();
                return Tuple.Create(false, r);

            }
        }


        public void LogPayliteTrxn(SendMoneyViaUserHandleReq req, string BeneficiaryId, int TrxnStatus)
        {
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_Fxmallam");
                c.SetProcedure("TrxnRecordPaylite");
                c.AddParam("@SenderID", req.Bvn);
                c.AddParam("@ReceiverID", BeneficiaryId);
                c.AddParam("@AmountSent", req.Amount);
                c.AddParam("@ReciepientName", req.ReceipientName);
                c.AddParam("@TransFerStatus", TrxnStatus);
                c.AddParam("@PaymentRef", req.PaymentReference);
                c.ExecuteProc();
                int? resp = c.returnValue;
            }
            catch (Exception ex)
            {
                new ErrorLog("errorloging Paylite trxn: Details: " + req.Bvn + " To: " + req.ReceipientHandle + " Amount:" + req.Amount + " Error was:" + ex);
            }
        }

        public int doGenerateOtpByMobile(string mobnum, Int32 Appid, string EMAIL)
        {
            //new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban);
            int status = 0;


            string otp = "";
            //generate the otp
            Gadget g = new Gadget();
            otp = g.newOTP();

            //send the OTP to the customer through infobip
            Utility u = new Utility();

            int cnt = u.insertIntoInfobip(otp, mobnum, mobnum);  // i need to disable this method for now, since i am still developing. will be reenabled after building
            if (cnt > 0)
            {
                InsertRecordByMob(mobnum, otp, EMAIL);
                status = 1;
            }
            else
            {
                status = 2;
            }
            return status;
        }
        public void InsertRecordByMob(string mobile, string otp, string EMAIL)
        {
            string sql = "insert into tbl_otpByMobile(mobile, otp) values(@mobile, @otp)";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
            c.SetSQL(sql);
            c.AddParam("@mobile", mobile);
            c.AddParam("@otp", otp);
            c.Execute();
            try
            {
                SendOtpMail sendOtpMail = new SendOtpMail();
                sendOtpMail.SendOtpViaMail(EMAIL, "Paylite@Sterlingbankng.com", otp, "PayLite App");
            }
            catch (Exception ex)
            {

            }
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
        //public int doGenerateOtp(string nuban, Int32 Appid)
        //{
        //    EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        //    //new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban);
        //    int status = 0; string mobnum = ""; string EMAIL = "";
        //    DataSet ds = ws.getAccountFullInfo(nuban);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        //get the mobnum into the values
        //        DataRow dr = ds.Tables[0].Rows[0];
        //        mobnum = dr["MOB_NUM"].ToString();
        //        EMAIL = dr["EMAIL"].ToString();
        //        mobnum = mobnum.Trim();
        //    }
        //    else
        //    {

        //    }
        //    string otp = "";
        //    //generate the otp
        //    Gadget g = new Gadget();
        //    otp = g.newOTP();

        //    //send the OTP to the customer through infobip
        //    Utility u = new Utility();

        //    int cnt = u.insertIntoInfobip(otp, mobnum, nuban);
        //    if (cnt > 0)
        //    {
        //        InsertRecord(nuban, otp);
        //        try
        //        {
        //            if (Appid == 51 || Appid == 55)
        //            {
        //                SendOtpMail sendOtpMail = new SendOtpMail();
        //                sendOtpMail.SendOtpViaMail(EMAIL, "Switch@Sterlingbankng.com", otp, "Switch Portal");
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        status = 1;
        //    }
        //    else
        //    {
        //        status = 2;
        //    }
        //    return status;
        //}

        //public int SendSms(string msg, string mobnum, Int32 Appid)
        //{
        //    //send the OTP to the customer through infobip
        //    Utility u = new Utility();

        //    int cnt = u.insertIntoInfobip(msg, mobnum, mobnum);
        //    if (cnt > 0) return 1;
        //    return -1;
        //}
        //public string doGenerateOtpAndMail(string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject)
        //{
        //    EACBS.banksSoapClient ws = new EACBS.banksSoapClient(); string rspval = "";
        //    new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban " + nuban);
        //    int status = 0; string mobnum = "";
        //    DataSet ds = ws.getAccountFullInfo(nuban);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        //get the mobnum into the values
        //        DataRow dr = ds.Tables[0].Rows[0];
        //        mobnum = dr["MOB_NUM"].ToString();
        //        mobnum = mobnum.Trim();
        //    }
        //    else
        //    {

        //    }

        //    string otp = "";
        //    //generate the otp
        //    Gadget g = new Gadget();
        //    otp = g.newOTP();

        //    //send the OTP to the customer through infobip
        //    Utility u = new Utility();

        //    int cnt = u.insertIntoInfobipSPECTA(otp, mobnum, nuban);
        //    if (cnt > 0)
        //    {
        //        InsertRecord(nuban, otp);
        //        SendOtpMail sendOtpMail = new SendOtpMail();
        //        sendOtpMail.SendOtpViaMail(destinationEmail, sourceEmail, otp, subject);
        //        status = 1;
        //    }
        //    else
        //    {
        //        if (mobnum == "")
        //        {
        //            //mobile number is empty in T24
        //            status = 3;
        //        }
        //        else
        //        {
        //            status = 2;
        //        }
        //    }
        //    return status.ToString() + '*' + otp + '*' + mobnum;
        //}


        public string doGenerateOtpAndMailByPhoneNumber(string phoneNumber, string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject)
        {

            EACBS.banksSoapClient ws = new EACBS.banksSoapClient(); string rspval = "";

            var mobnum = phoneNumber.Trim();

            string otp = "";
            //generate the otp
            Gadget g = new Gadget();
            otp = g.newOTP();
            int status = 0;
            //send the OTP to the customer through infobip
            Utility u = new Utility();

            int cnt = u.insertIntoInfobipSPECTA(otp, mobnum, nuban);
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

        //for paylite..the nuban will be hte number
        public int verifyOtp(string nuban, string otp, Int32 Appid)
        {
            new ErrorLog("New details received via channel with appid " + Appid.ToString() + " nuban(paylite number) " + nuban + " with otp " + otp);
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

        public void UpdateRecord(string nuban, string otp)
        {
            string sql = "update tbl_otp set statusflag=1 where nuban =@nuban and otp =@otp and statusflag=0";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
            c.SetSQL(sql);
            c.AddParam("@nuban", nuban);
            c.AddParam("@otp", otp);
            c.Execute();
        }
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

        public void UpdateRecordOtpByMobile(string mobile, string otp)
        {
            string sql = "update tbl_otpByMobile set statusflag=1 where mobile =@mobile and otp =@otp and statusflag=0";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_ibsdb");
            c.SetSQL(sql);
            c.AddParam("@mobile", mobile);
            c.AddParam("@otp", otp);
            c.Execute();
        }

        public string doGenerateOtpAndMailByPhoneNumberWithMailBody(string phoneNumber, string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject, string mailBody)
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient(); string rspval = "";
            var mobnum = phoneNumber.Trim();
            string otp = "";
            //generate the otp
            Gadget g = new Gadget();
            otp = g.newOTP();
            int status = 0;
            //send the OTP to the customer through infobip
            Utility u = new Utility();

            int cnt = u.insertIntoInfobipSPECTA(otp, mobnum, nuban);
            if (cnt > 0)
            {
                InsertRecord(nuban, otp);
                SendOtpMail sendOtpMail = new SendOtpMail();
                if (!String.IsNullOrEmpty(mailBody))
                {
                    mailBody = mailBody.Replace("GenOTP", otp);
                    sendOtpMail.SendOtpViaMailNew(destinationEmail, sourceEmail, mailBody, subject);
                }
                else
                {
                    sendOtpMail.SendOtpViaMail(destinationEmail, sourceEmail, otp, subject);
                }
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

        public int InsertWhatsappTransferRequest(string phoneNumber, string Amount, string ReciepientNuban)
        {
            //get sender account and send it
            Whatsappbank.banksSoapClient GetSenderNuban = new Whatsappbank.banksSoapClient();
            var senderDetails = GetSenderNuban.getCustomerAccountsByMobileNo2(phoneNumber);
            string SenderNuban = senderDetails.Tables[0].Rows[0]["NUBAN"].ToString();
            string SenderBalance = senderDetails.Tables[0].Rows[0]["Balance"].ToString();
            if (Convert.ToDecimal(SenderBalance) < Convert.ToDecimal(Amount))
            {
                //insufficcient funds check
                return 2;
            }
            string sql = "insert into tbl_WhatsappTransfer(phoneNumber, Amount, ReciepientNuban, SenderNuban) values(@phoneNumber, @Amount, @ReciepientNuban, @SenderNuban)";
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_twilio");
                c.SetSQL(sql);
                c.AddParam("@phoneNumber", phoneNumber);
                c.AddParam("@Amount", Amount);
                c.AddParam("@ReciepientNuban", ReciepientNuban);
                c.AddParam("@SenderNuban", SenderNuban);

                var r = c.Execute();
                if (r == 1)
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
                //log error
            }
            return 0;
        }

        public int ConsumateWhatsappTransfer(string phonenumber, string TrxnStatus)
        {
            //validate the OTP
            //if successful, push the trxns via ibs service
            //respond to the customer based on trxn status

            var valid = 1;//verifyOtpByMobile(phonenumber, otp, 1);
            if (valid == 1)
            {

                int active = 0;
                string sql = "select top 1  phoneNumber, Amount, DateTimeAdded, ReciepientNuban, TrxnStatus, SenderNuban from tbl_WhatsappTransfer where phoneNumber=@phoneNumber and TrxnStatus=@TrxnStatus order by DateTimeAdded desc";



                //select top 1  phoneNumber, Amount,DateTimeAdded,ReciepientNuban, TrxnStatus, SenderNuban from tbl_WhatsappTransfer where phoneNumber = '08105931866' and TrxnStatus = 0 order by DateTimeAdded desc
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_twilio");
                DataSet ds = new DataSet(); DateTime dt = new DateTime();
                try
                {
                    dt = new DateTime();
                    c.SetSQL(sql);
                    c.AddParam("@phoneNumber", phonenumber);
                    c.AddParam("@TrxnStatus", Convert.ToInt16(TrxnStatus));
                    ds = c.Select("rec");
                }
                catch (Exception ex)
                {

                    new ErrorLog("An exception occured while query the db. while trying to slect trxn to process" + ex.ToString());
                    return 0;
                }
                try
                {
                    if (ds == null || ds.Tables[0].Rows.Count < 1)
                    {
                        return 0; //could not get records
                    }
                    var tb = ds.Tables[0];
                    var query = from d in tb.AsEnumerable()
                                select new
                                {
                                    phoneNumber = d.Field<string>("phoneNumber"),
                                    Amount = d.Field<decimal>("Amount"),
                                    DateTimeAdded = d.Field<DateTime>("DateTimeAdded"),
                                    TrxnStatus = d.Field<int>("TrxnStatus"),
                                    SenderNuban = d.Field<string>("SenderNuban"),
                                    ReciepientNuban = d.Field<string>("ReciepientNuban"),
                                    SessionID = d.Field<string>("SessionID"),
                                    ReferenceID = d.Field<string>("ReferenceID"),
                                    DestionationBankCode = d.Field<string>("DestionationBankCode"),
                                    BeneficiaryName = d.Field<string>("BeneficiaryName"),
                                    PaymentReference = d.Field<string>("PaymentReference")
                                };

                    if (query == null)
                    {
                        return 0;
                    }
                    else
                    {

                        foreach (var q in query)
                        {
                            //process trxn in here
                            var ProcessingResponse = ProcessSterlingTrxn(q.ReciepientNuban, q.SenderNuban, q.Amount, q.TrxnStatus.ToString(), q.SessionID.ToString(), q.ReferenceID.ToString(), q.DestionationBankCode.ToString(), q.BeneficiaryName.ToString(), q.PaymentReference.ToString());
                            if (ProcessingResponse == 1)
                            {
                                new ErrorLog("transaction succesful: " + phonenumber + " amount: " + q.Amount + " recipient:" + q.ReciepientNuban);
                                return 1;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    return 0;
                    //  throw;
                }
            }
            else
            {
                return 0;
            }

            return 0;
        }

        public int ProcessSterlingTrxn(string toNuban, string fromNuban, decimal amount, string status, string SessionID, string ReferenceID, string DestionationBankCode, string BeneficiaryName, string PaymentReference)
        {
            try
            {
                //parameterize the obj
                //pass it to the transfer obj
                //read response
                OBJ_IBS_Transfer_Class req = new OBJ_IBS_Transfer_Class()
                {
                    ReferenceID = ReferenceID,
                    RequestType = "",//hardcode this
                    FromAccount = fromNuban,
                    ToAccount = toNuban,
                    Amount = amount,
                    PaymentReference = PaymentReference,
                    SessionID = SessionID,
                    DestionationBankCode = DestionationBankCode,
                    BeneficiaryName = BeneficiaryName

                };
                //exec transfer----eventually we will route trxns if it's external or internal from here

                var Transferresponse = SterlingBankIntraBank(req);

                //T24Bank debitTeller = new T24Bank();
                //string vtellerFolderName = "dumft";
                //string transtype = "FT";
                //string remarks = "WhatsappTransferTo::" + toNuban;
                //int exploCode = 111;
                //IAccount towhere;
                //ITransactionResult result = null;
                //IAccount ToAccount = debitTeller.GetAccountInfoByAccountNumber(toNuban.Trim());
                //IAccount FromAccount = debitTeller.GetAccountInfoByAccountNumber(fromNuban.Trim());

                //result = debitTeller.Transfer(fromNuban.Trim(), toNuban.Trim(), transtype, amount, remarks, exploCode);

                //result = debitTeller.Transfer(FromAccount, ToAccount, transtype, amount, remarks, exploCode);
                if (Transferresponse != null && Transferresponse.ResponseCode == "00")
                {
                    //update the trxn status a '1' to show that it as been processed
                    status = "1";
                    UpdateWhatsappTrxn(toNuban.Trim(), fromNuban.Trim(), amount, status, req.SessionID);//1 means it has been update
                    return 1;//means succesful trxn
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
                //throw;
            }



        }


        public void UpdateWhatsappTrxn(string toNuban, string fromNuban, decimal amount, string status, string SessionID)
        {
            string sql = "update tbl_WhatsappTransfer set TrxnStatus=@TrxnStatus and DateTimeTreated=@DateTimeTreated where SenderNuban=@SenderNuban and ReciepientNuban=@ReciepientNuban and Amount=@Amount and SessionID=SessionID";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_twilio");
            c.SetSQL(sql);
            c.AddParam("@DateTimeTreated", DateTime.Now);
            c.AddParam("@SenderNuban", fromNuban);
            c.AddParam("@ReciepientNuban", toNuban);
            c.AddParam("@Amount", amount);
            c.AddParam("@TrxnStatus", status);
            c.AddParam("@SessionID", SessionID);
            var response = c.Execute();


        }


        public OBJ_IBS_Transfer_Response_Class SterlingBankIntraBank(OBJ_IBS_Transfer_Class Transfer)
        {

            OBJ_IBS_Transfer_Response_Class finalAcctName = new OBJ_IBS_Transfer_Response_Class();
            try
            {
                // BSServicesSoapClient call = new BSServicesSoapClient();
                IbsServer.BSServicesSoapClient call = new IbsServer.BSServicesSoapClient();
                // Transfer.ReferenceID = GenerateTransactionCodes.CodeFramework.GenerateNumericTransactionCodes(10);
                StringBuilder sbk = new StringBuilder();
                sbk.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sbk.Append("<IBSRequest>");
                sbk.Append("<ReferenceID>" + Transfer.ReferenceID + "</ReferenceID>");
                sbk.Append("<RequestType>" + Transfer.RequestType + "</RequestType>");
                sbk.Append("<FromAccount>" + Transfer.FromAccount + "</FromAccount>");
                sbk.Append("<ToAccount>" + Transfer.ToAccount + "</ToAccount>");
                sbk.Append("<Amount>" + Math.Round(Transfer.Amount, 2) + "</Amount>");
                sbk.Append("<PaymentReference>" + Transfer.PaymentReference + "</PaymentReference>");
                sbk.Append("</IBSRequest>");

                var enq = Utility.Encrypt(sbk.ToString());
                new ErrorLog("Request::" + sbk.ToString());


                var responcode = call.IBSBridge(enq, Convert.ToInt16(ConfigurationManager.AppSettings["AppID"]));
                var finalresponse = Utility.Decrypt(responcode);
                XmlDocument xmlDoc = new XmlDocument();
                // log.Info("Logging Intrabank transfer Response");
                new ErrorLog("Response:" + finalresponse.ToString());
                xmlDoc.LoadXml(finalresponse);
                finalAcctName.ResponseCode = Convert.ToString(xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText);
                finalAcctName.ResponseText = Convert.ToString(xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText);

            }
            catch (Exception ex)
            {
                new ErrorLog("Error Response:" + ex.ToString());
            }
            return finalAcctName;

        }

        public OBJ_IBS_Transfer_Response_Class SterlingBankInterBank(OBJ_IBS_Transfer_Class Transfer)
        {

            OBJ_IBS_Transfer_Response_Class finalAcctName = new OBJ_IBS_Transfer_Response_Class();
            try
            {
                IbsServer.BSServicesSoapClient call = new IbsServer.BSServicesSoapClient();
                //BSServicesSoapClient call = new BSServicesSoapClient();
                // call.IBSBridge()
                StringBuilder sbk = new StringBuilder();
                sbk.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sbk.Append("<IBSRequest>");
                sbk.Append("<SessionID>" + Transfer.SessionID + "</SessionID>");
                sbk.Append("<ReferenceID>" + Transfer.ReferenceID + "</ReferenceID>");
                sbk.Append("<RequestType>" + Transfer.RequestType + "</RequestType>");
                sbk.Append("<FromAccount>" + Transfer.FromAccount + "</FromAccount>");
                sbk.Append("<ToAccount>" + Transfer.ToAccount + "</ToAccount>");
                sbk.Append("<Amount>" + Math.Round(Transfer.Amount, 2) + "</Amount>");
                sbk.Append("<DestinationBankCode>" + Transfer.DestionationBankCode + "</DestinationBankCode>");
                sbk.Append("<BenefiName>" + Transfer.BeneficiaryName + "</BenefiName>");
                sbk.Append("<PaymentReference>" + Transfer.PaymentReference + "</PaymentReference>");
                sbk.Append("</IBSRequest>");

                new ErrorLog("Request::" + sbk.ToString());


                var enq = Utility.Encrypt(sbk.ToString());
                var IbsResponcode = call.IBSBridge(enq, Convert.ToInt16(ConfigurationManager.AppSettings["AppID"]));
                var finalresponse = Utility.Decrypt(IbsResponcode);
                XmlDocument xmlDoc = new XmlDocument();
                new ErrorLog("Request::" + finalresponse.ToString());

                xmlDoc.LoadXml(finalresponse);
                finalAcctName.ResponseCode = Convert.ToString(xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText);
                finalAcctName.ResponseText = Convert.ToString(xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText);

            }
            catch (Exception ex)
            {
                new ErrorLog("Request::" + ex.ToString());
            }
            return finalAcctName;

        }


        public SpayResponse.Rootobject WalletTransfer(string amt, string frmacct, string toacct, string paymentRef, string remarks)
        {
            try
            {
                string BaseUrl = "https://pass.sterlingbankng.com/spay";
                string respon = "";
                string url = BaseUrl + "/api/Spay/SBPMWalletRequest";
                var client = new RestSharp.RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("ContentType", "application/json");
                // request.AddHeader("AppId", "69");
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("AppId", 69, ParameterType.HttpHeader);
                request.AddBody(new
                {
                    Referenceid = DateTime.Now.Ticks,
                    RequestType = 109,
                    Translocation = "10030,4999",
                    amt = amt,
                    tellerid = "1111",
                    frmacct = frmacct,
                    toacct = toacct,
                    exp_code = "101",
                    paymentRef = paymentRef,
                    remarks = remarks

                });
                var s = client.Execute(request);
                var Xxc = JsonConvert.DeserializeObject<dynamic>(s.Content);
                respon = JsonConvert.SerializeObject(Xxc);
                var respJson = new JavaScriptSerializer().Deserialize<SpayResponse.Rootobject>(respon);
                // var test = respJson.data;
                return respJson;
            }
            catch (Exception e)
            {
                SpayResponse.Rootobject ff = new SpayResponse.Rootobject();
                ff.message = "Transaction Failed";
                return ff;
            }
        }

        public SpayResponse.Rootobject SBPT24txnRequest(string amt, string frmacct, string toacct, string paymentRef, string remarks)
        {
            try
            {
                string BaseUrl = "https://pass.sterlingbankng.com/spay";
                string respon = "";
                string url = BaseUrl + "/api/Spay/SBPT24txnRequest";
                var client = new RestSharp.RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("ContentType", "application/json");
                request.AddParameter("AppId", 69, ParameterType.HttpHeader);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    Referenceid = DateTime.Now.Ticks,
                    RequestType = 110,
                    Translocation = "10030,4999",
                    amt = amt,
                    tellerid = "1111",
                    frmacct = frmacct,
                    toacct = toacct,
                    exp_code = "101",
                    paymentRef = paymentRef,
                    remarks = remarks

                });
                var s = client.Execute(request);
                var Xxc = JsonConvert.DeserializeObject<dynamic>(s.Content);
                respon = JsonConvert.SerializeObject(Xxc);
                var respJson = new JavaScriptSerializer().Deserialize<SpayResponse.Rootobject>(respon);
                //  var test = respJson.responsedata;
                return respJson;
            }
            catch (Exception e)
            {
                SpayResponse.Rootobject ff = new SpayResponse.Rootobject();
                ff.message = "Transaction Failed";
                return ff;
            }
        }
        public static string createwallet(string FirstName, string LastName, string Gender, string DateOfBirth, string UserEmail, string title, string PhoneNumber)
        {
            string respon = "";
            try
            {
                if (Gender == "1")
                {
                    Gender = "M";
                    title = "1";
                }
                else
                {
                    title = "2";
                    Gender = "F";
                }
                string BaseUrl = ConfigurationManager.AppSettings["spayBaseURI"].ToString();
                string referenceId = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString();
                string url = BaseUrl + "/Spay/SBPMWalletAccountReq";
                var client = new RestSharp.RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("ContentType", "application/json");
                request.AddHeader("Authorization", "");
                request.AddParameter("AppId", 69, ParameterType.HttpHeader);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    Referenceid = referenceId,
                    RequestType = "117",
                    Translocation = "6.587363,7.494737",
                    FIRSTNAME = FirstName,
                    LASTNAME = LastName,
                    MOBILE = PhoneNumber,
                    GENDER = Gender,
                    BIRTHDATE = DateOfBirth,
                    EMAIL = UserEmail,
                    NATIONALITY = "NIGERIA",
                    TARGET = "1111",
                    SECTOR = "1",
                    ADDR_LINE1 = "Lagos Island",
                    ADDR_LINE2 = "20 marina ",
                    CUST_TYPE = "1",
                    MARITAL_STATUS = "S",
                    TITLE = title,
                    CUST_STATUS = "1",
                    RESIDENCE = "NG",
                    CATEGORYCODE = "10001"
                });
                var s = client.Execute(request);
                var Xxc = JsonConvert.DeserializeObject<dynamic>(s.Content);
                respon = JsonConvert.SerializeObject(Xxc);
                var respJson = new JavaScriptSerializer().Deserialize<SpayResponse.Rootobject>(respon);
                //var test = respJson.data;
                if (respJson.response == "00")
                {
                    return "1";

                }
                else { return "2"; }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                //  return null;
            }
            return respon;
        }

        public SpayResponse.Rootobject AirtimeRecharge(AirtimeRechargeReq airtimeRecharge)
        {
            StringBuilder _sb = new StringBuilder();

            _sb.Append("<?xml version='1.0' encoding='utf-8'?>");

            _sb.Append("<IBSRequest>");

            _sb.Append("<ReferenceID>" + airtimeRecharge.ReferenceID + "</ReferenceID>");

            _sb.Append("<RequestType>" + "932" + "</RequestType>");

            _sb.Append("<Mobile>" + airtimeRecharge.Mobile + "</Mobile>");

            _sb.Append("<Beneficiary>" + airtimeRecharge.Beneficiary + "</Beneficiary>");

            _sb.Append("<Amount>" + airtimeRecharge.Amount + "</Amount>");

            _sb.Append("<NUBAN>" + airtimeRecharge.NUBAN + "</NUBAN>");

            _sb.Append("<NetworkID>" + airtimeRecharge.NetworkID + "</NetworkID>");

            _sb.Append("<Type>" + airtimeRecharge.Type + "</Type>");
            _sb.Append("</IBSRequest>");
            string xmlData = _sb.ToString();
            string url = "https://pass.sterlingbankng.com/spay/api/IBSIntegrator/IBSBridge";

            var Client = new RestSharp.RestClient(url);
            var Request = new RestRequest(Method.POST);
            // string jsonstring = "";
            string Ipval = GetLocalIPAddress().ToString();
            Request.AddHeader("ContentType", "application/json");
            Request.AddHeader("SwitchId", "nabbo247@yahoo.com");
            Request.AddHeader("ipval", Ipval);
            Request.AddParameter("AppId", 69, ParameterType.HttpHeader);
            Request.RequestFormat = DataFormat.Json;
            Request.AddBody(new
            {
                Appid = "69",
                XML = xmlData

            });
            var s = Client.Execute(Request);
            var resp = JsonConvert.DeserializeObject<SpayResponse.Rootobject>(s.Content);
            return resp;
 
        }
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress;
        }
        public class SpayResponse
        {
            public class Rootobject
            {
                public string message { get; set; }
                public string response { get; set; }
                public Data data { get; set; }
            }
            public class Data
            {
                //public string AccountName { get; set; }
                //public string AccountNumber { get; set; }
                public float AccountBalance { get; set; }
                public string status { get; set; }
            }
        }
    }
}