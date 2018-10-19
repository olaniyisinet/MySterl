using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TakeUssdChargesBalEnq
{
    class Gadget
    {
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
        public String Encrypt(String val, Int32 Appid)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {
                string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
                sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqibs");
                c.SetSQL(sql);
                c.AddParam("@Appid", Appid);
                DataSet ds = c.Select("rec");
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
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqibs");
                c.SetSQL(sql);
                c.AddParam("@Appid", Appid);
                DataSet ds = c.Select("rec");
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
    }
}
