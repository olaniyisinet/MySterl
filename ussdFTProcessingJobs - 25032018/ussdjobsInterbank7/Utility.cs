using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ussdjobsInterbank7
{
    class Utility
    {
        public void InsertPINTrails(string mob)
        {
            string sql = "insert into tbl_USSD_PINTRIALS(mobile) values (@mob) ";
            Connect c = new Connect(sql, true);
            c.addparam("@mob", mob);
            c.query();
        }
        public int getPinTrails(string mob)
        {
            int cnt = 0;
            string sql = "select count(*) as cnt from tbl_USSD_PINTRIALS where mobile =@mob " +
                 " and convert(varchar(50), dateadded,112) = convert(varchar(50), getdate(),112)";
            Connect c = new Connect(sql, true);
            c.addparam("@mob", mob);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                cnt = int.Parse(dr["cnt"].ToString());
            }
            return cnt;
        }
        public decimal Totaldone; public int totlcnt;
        public bool getTotalTransDonePerday(decimal amt, string mobile, string sessionid, decimal Maxperday)
        {
            DataSet ds = new DataSet();
            bool ok = false;
            string sql = "select ISNULL(SUM(amt),0) as totalTOday,ISNULL(count(amt),0) as cnt from tbl_USSD_transfers " +
                " where mobile =@mob " +
                " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and statusflag = 1 ";
            Connect c = new Connect(sql, true);
            c.addparam("@mob", mobile);
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Totaldone = decimal.Parse(dr["totalTOday"].ToString());
                totlcnt = int.Parse(dr["cnt"].ToString());
                if (Totaldone + amt > Maxperday)
                {
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
            else
            {
                ok = false;
            }

            return ok;
        }
        public DataSet getMINMAXamt()
        {
            string sql = "select minamt,maxamt from tbl_USSD_AMTRANGE where statusflag=1";
            Connect c = new Connect(sql, true);
            DataSet ds = c.query("rec");
            return ds;
        }
        public bool CardAuthenticate(string cardacct, string pin, string Nuban)
        {
            bool found = false;
            string sql = "select custauthid from Go_Registered_Account where nuban = @nu and custauthid = @pin";
            Connect c = new Connect(sql, true);
            c.addparam("@nu", Nuban);
            c.addparam("@pin", pin);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                found = true;
            }
            else
            {
                found = false;
            }

            //string sql = "select * from tbl_USSD_card_auth where cardacct =@cat and last4carddigits =@l4d ";
            //Connect c = new Connect(sql, true);
            //c.addparam("@cat", cardacct);
            //c.addparam("@l4d", last4digits);
            //DataSet ds = c.query("rec");
            //if(ds.Tables[0].Rows.Count > 0)
            //{
            //    found = true;
            //}
            //else
            //{
            //    //cehc with the nuban this time
            //    string sql1 = "select * from tbl_USSD_card_auth where cardacct =@cat and last4carddigits =@l4d ";
            //    Connect c1 = new Connect(sql1, true);
            //    c1.addparam("@cat", Nuban);
            //    c1.addparam("@l4d", last4digits);
            //    DataSet ds1 = c1.query("rec");
            //    if (ds1.Tables[0].Rows.Count > 0)
            //    {
            //        found = true;
            //    }
            //    else
            //    {
            //        found = false;
            //    }
            //}
            return found;
        }
        public static string ConvertMobile080(string mobile)
        {
            char[] trima = { '+', ' ' };
            mobile = mobile.Trim(trima);
            if (mobile.Length == 11 && mobile.StartsWith("0"))
            {
                return mobile;
            }
            if (mobile.Length >= 10)
            {
                mobile = "0" + mobile.Substring(mobile.Length - 10, 10);
                return mobile;
            }
            return mobile;
        }
        public static string ConvertMobile234(string mobile)
        {
            char[] trima = { '+', ' ' };
            mobile = mobile.Trim(trima);
            if (mobile.Length == 13 && mobile.StartsWith("234"))
            {
                return mobile;
            }
            if (mobile.Length >= 10)
            {
                mobile = "234" + mobile.Substring(mobile.Length - 10, 10);
                return mobile;
            }
            return mobile;
        }
        public int insertIntoInfobip(string msg, string msdn)
        {
            int val = 0; string phone = ""; string sender = "STERLING";
            phone = ConvertMobile234(msdn);
            string sql = "insert into proxytable (phone,sender,text) values(@phone,@sender,@msg)";
            Connect c = new Connect(sql, true, true, true);
            c.addparam("@phone", phone);
            c.addparam("@sender", sender);
            c.addparam("@msg", msg);
            try
            {
                val = c.query();
            }
            catch
            {
                val = 0;
            }
            return val;
        }
        public string GenerateRndNumber(int cnt)
        {
            string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Random rand1 = new Random();
            string txt = "";
            for (int j = 0; j < cnt; j++)
                txt += key2[rand1.Next(0, 9)];
            return txt;
        }
        public string newSessionId(string bankcode)
        {
            Thread.Sleep(50);
            return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12);
        }
        public string BinaryToString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentNullException("binary");

            if ((binary.Length % 8) != 0)
                throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string section = binary.Substring(i, 8);
                int ascii = 0;
                try
                {
                    ascii = Convert.ToInt32(section, 2);
                }
                catch
                {
                    throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
                }
                builder.Append((char)ascii);
            }
            return builder.ToString();
        }
        public String Encrypt(String val, Int32 Appid)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {
                string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
                sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
                Connect c = new Connect(sql, true, true);
                c.addparam("@Appid", Appid);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    sharedkeyval = dr["SharedKey"].ToString();
                    sharedkeyval = BinaryToString(sharedkeyval);

                    sharedvectorval = dr["Sharedvector"].ToString();
                    sharedvectorval = BinaryToString(sharedvectorval);

                    //sharedkeyval = "\a\v\r\r\v\a\b\f\a\v\r";// dr["SharedKey"].ToString();" \ a \ v \ r \ r \ v \ a \ b \ f \ a \ v \ r "
                    //sharedvectorval = "\a\v\r";// dr["Sharedvector"].ToString();"\a\v\r"
                }

                byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
                cs.Write(toEncrypt, 0, toEncrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                //new ErrorLog("There is an issue with the xml received " + val + " Invalid xml");
                //rsp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSResponse><ResponseCode>57</ResponseCode><ResponseText>Transaction not permitted to sender</ResponseText></IBSResponse>";
                //rsp = Encrypt(rsp, Appid);
                //return rsp;
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public String Decrypt(String val, Int32 Appid)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {

                string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
                sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
                Connect c = new Connect(sql, true, true);
                c.addparam("@Appid", Appid);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    sharedkeyval = dr["SharedKey"].ToString();
                    sharedkeyval = BinaryToString(sharedkeyval);

                    sharedvectorval = dr["Sharedvector"].ToString();
                    sharedvectorval = BinaryToString(sharedvectorval);
                }

                byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toDecrypt = Convert.FromBase64String(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
                cs.Write(toDecrypt, 0, toDecrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                //new ErrorLog("There is an issue with the xml received " + val);
                //rsp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSResponse><ResponseCode>57</ResponseCode><ResponseText>Transaction not permitted to sender</ResponseText></IBSResponse>";
                //rsp = Encrypt(rsp, Appid);
                //return rsp;
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        //public string getBankcode(string sid)
        //{
        //    string bcode = "";
        //    try
        //    {
        //        string sql = "SELECT reqID,sessionid,req_msisdn,req_type,req_msg FROM tbl_USSD_trnx " +
        //            "  where sessionid=@sid and resp_msg like '%Select Acct to Transfer%' ";
        //        Connect c = new Connect(sql, true);
        //        c.addparam("@sid", sid);
        //        DataSet ds = c.query("rec");
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            DataRow dr = ds.Tables[0].Rows[0];
        //            bcode = dr["req_msg"].ToString();
        //        }
        //        else
        //        {
        //            bcode = "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new Errorlog("Error occured " + ex);
        //    }
        //    return bcode;
        //}


        //newly added////////////////////////////////
        public string getTheBankCode(string bcode)
        {
            string bcodeval = "";
            try
            {
                string sql = "select bankcode from tbl_participatingBanks where transrate =@id ";
                Connect c = new Connect(sql, true, true, true, true);
                c.addparam("@id", bcode);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    bcodeval = dr["bankcode"].ToString();
                }
            }
            catch { }
            return bcodeval;
        }
        public string getBankcode(string sid)
        {
            string bcode = "";
            try
            {
                string sql = "SELECT * FROM tbl_USSD_transfers " +
                    "  where sessionid=@sid ";
                Connect c = new Connect(sql, true);
                c.addparam("@sid", sid);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    bcode = dr["bankcode"].ToString();
                    //go to bcode and get the real bankcode
                    bcode = getTheBankCode(bcode);
                }
                else
                {
                    bcode = "";
                }
            }
            catch (Exception ex)
            {
                new Errorlog("Error occured " + ex);
            }
            return bcode;
        }
        public string getNubanByMobile(string mobile)
        {
            string nuban = "";
            try
            {
                string sql = "select nuban from Go_Registered_Account where mobile =@mb " +
                     " and Activated = 1 ";
                Connect c = new Connect(sql, true);
                c.addparam("@mb", mobile);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    nuban = dr["nuban"].ToString();
                }
            }
            catch (Exception ex)
            {
                new Errorlog("Error occured getting NUBAN " + ex);
            }
            return nuban;
        }
        public int GetTrnxLimitStatus(string mobile, string nuban)
        {
            int trnxLimit = 0;

            try
            {
                //string sql = "select nuban from Go_Registered_Account where mobile =@mb " +
                //     " and Activated = 1 ";
                string sql = "select TrnxLimit from Go_Registered_Account where Mobile = @mobile and NUBAN = @nuban and Activated = 1";
                Connect c = new Connect(sql, true);
                c.addparam("@mobile", mobile);
                c.addparam("@nuban", nuban);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    trnxLimit = int.Parse(dr["TrnxLimit"].ToString());
                }
                else
                {
                    trnxLimit = -1;
                }
            }
            catch (Exception ex)
            {
                new Errorlog("Error occured getting transaction limit status for Nuban: " + nuban + " Mobile No:" + mobile + " " + ex);
            }

            return trnxLimit;
        }
        public DataSet GetLimitMINMAXamt()
        {
            string sql = "select minamt,maxamt from tbl_USSD_AMTRANGE_Bypass where EnrollProp=1";
            Connect c = new Connect(sql, true);
            DataSet ds = c.query("rec");
            return ds;
        }
        public bool getTotalAirtimeDonePerday(decimal amt, string mobile, string sessionid, decimal Maxperday)
        {
            DataSet ds = new DataSet();
            bool ok = false;
            string sql = "select isnull((select SUM(Amount) from [dbo].[Go_Request] where Convert(date,[RequestDate]) = Convert(date,GETDATE()) AND Mobile  = @Mobile AND RequestStatus <> 2),0)";
            Connect c = new Connect(sql, true);
            c.addparam("@Mobile", mobile);
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                //DataRow dr = ds.Tables[0].Rows[0][0];
                Totaldone += decimal.Parse(ds.Tables[0].Rows[0][0].ToString());
                //Totaldone += decimal.Parse(dr["totalTOday"].ToString());
                //totlcnt += int.Parse(dr["cnt"].ToString());
                if (Totaldone + amt > Maxperday)
                {
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
            else
            {
                ok = false;
            }

            return ok;
        }
        ////////////////////////////////////////////
    }
}
