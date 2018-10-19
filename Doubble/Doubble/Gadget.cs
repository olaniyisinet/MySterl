using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Doubble
{
    public class Gadget
    {
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string getStateVal(string state)
        {
            string sql = "select * from State_tbl where StateName=@state";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@state", state);

            DataSet ds = c.Select("rec");
            return ds.Tables[0].Rows[0]["ValId"].ToString();
        }

        public string thAppend(string day)
        {
            if (day.EndsWith("1"))
            {
                return day + "st";
            }
            else if (day.EndsWith("2"))
            {
                return day + "nd";
            }
            else if (day.EndsWith("3"))
            {
                return day + "rd";
            }
            else
            {
                return day + "th";
            }
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
        public String Encrypt(String val)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                string sharedkeyval = ""; string sharedvectorval = "";
                sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "1100010100010100100000001101010101011100000001010000111000000010";
                sharedvectorval = BinaryToString(sharedvectorval);
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
                return "";
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public String Decrypt(String val)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                string sharedkeyval = ""; string sharedvectorval = "";

                sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "1100010100010100100000001101010101011100000001010000111000000010";
                sharedvectorval = BinaryToString(sharedvectorval);

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
                return "";
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }



    }
}