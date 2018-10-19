using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BVNAcctCreation
{
    class Utility
    {
        string sharedkeyval = "";
        string sharedvectorval = "";
        public String Encrypt(String val)
        {
             sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
             sharedvectorval = "0000000100000010000000110000010100000111000010110000110100010001";
            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = BinaryToString(sharedvectorval);

            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toEncrypt = Encoding.UTF8.GetBytes(val);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        public String Decrypt(String val)
        {
            sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
            sharedvectorval = "0000000100000010000000110000010100000111000010110000110100010001";
            
            sharedkeyval = BinaryToString(sharedkeyval);
            sharedvectorval = BinaryToString(sharedvectorval);

            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);

            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
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

        public int UpdateRec(Int32 refid, string Fname, string Mname, string Lname, DateTime dob, string EnrollBra, DateTime RegDate, string EnrBkcode, int status )
        {
            string sql = "";
            sql = "Update tbl_USSD_acct_open set FirstName =@fn, MiddleName =@mn, LastName=@ln, " +
                " DateOfBirth =@dob, EnrollmentBranch =@Ebra, RegistrationDate =@regd, " +
                " EnrollmentBankcode = @EnrBkcode, statusflag =@status " +
                " where statusflag =0 and refid = @rid ";
            Connect c = new Connect(sql, true);
            c.addparam("@fn", Fname);
            c.addparam("@mn", Mname);
            c.addparam("@ln", Lname);
            c.addparam("@dob", dob);
            c.addparam("@Ebra", EnrollBra);
            c.addparam("@regd", RegDate);
            c.addparam("@EnrBkcode", EnrBkcode);
            c.addparam("@status", status);
            c.addparam("@rid", refid);
            int cn = c.query();
            return cn;
        }

        public int UpdateRec1(Int32 refid, int status)
        {
            string sql = "";
            sql = "Update tbl_USSD_acct_open set statusflag =@status " +
                " where statusflag =0 and refid = @rid ";
            Connect c = new Connect(sql, true);
            c.addparam("@status", status);
            c.addparam("@rid", refid);
            int cn = c.query();
            return cn;
        }
    }
}
