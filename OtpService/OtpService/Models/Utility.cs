using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility
{
    public Utility()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static string ConvertMobile234(string mobile)
    {
        char[] trima = { '+', ' ' };
        mobile = mobile.Trim(trima);
        if (mobile.Length == 13 && mobile.StartsWith("234"))
        {
            return mobile;
        }
        //if (mobile.Length >= 10)
        //{
        //    mobile = "234" + mobile.Substring(mobile.Length - 10, 10);
        //    return mobile;
        //}
        return mobile;
    }
    public int insertIntoInfobipSPECTA(string msg, string msdn, string nuban)
    {
        var sender = ConfigurationManager.AppSettings["Sender"];
        try
        {
            int val = 0; string phone = "";
            phone = ConvertMobile234(msdn);
            string sql = "insert into OTP (phone,sender,text,nuban) values(@phone,@sender,@msg,@nuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_infobip");
            cn.SetSQL(sql);
            cn.AddParam("@phone", phone);
            cn.AddParam("@sender", sender);
            cn.AddParam("@msg", msg);
            cn.AddParam("@nuban", nuban);
            try
            {
                val = cn.Execute();
            }
            catch
            {
                val = 0;
            }
            return val;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);

            return 0;
            //  throw;
        }
    }
    public int insertIntoInfobip(string msg, string msdn, string nuban)
    {
        try
        {
            int val = 0; string phone = ""; string sender = "STERLING";
            phone = ConvertMobile234(msdn);
            string sql = "insert into OTP (phone,sender,text,nuban) values(@phone,@sender,@msg,@nuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_infobip");
            cn.SetSQL(sql);
            cn.AddParam("@phone", phone);
            cn.AddParam("@sender", sender);
            cn.AddParam("@msg", msg);
            cn.AddParam("@nuban", nuban);
            try
            {
                val = cn.Execute();
            }
            catch
            {
                val = 0;
            }
            return val;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return 0;
            //throw;
        }
    }
    public static string mobile234(string mobile)
    {
        if (mobile.StartsWith("0"))
        {
            mobile = mobile.Substring(1, mobile.Length - 1);
            mobile = "234" + mobile;
        }
        if (mobile.StartsWith("+"))
        {
            // do something
        }
        return mobile;
    }

    public string encrypt(string toEncrypt)
    {

        SymmCrypto cryp = new SymmCrypto(SymmCrypto.SymmProvEnum.RC2);
        return cryp.Encrypting(toEncrypt, "12345678");

    }

    public string decrypt(string toDecrypt)
    {

        SymmCrypto cryp = new SymmCrypto(SymmCrypto.SymmProvEnum.RC2);
        return cryp.Decrypting(toDecrypt, "12345678");

    }
    public string encrypt2(string clearText)
    {

        return CryptorEngine.Encrypt(clearText, true);

    }

    public string decrypt2(string cipherText)
    {

        return CryptorEngine.Decrypt(cipherText, true);

    }
    public static string Encrypt(string val)
    {
        MemoryStream ms = new MemoryStream();
        // string rsp = "";
        try
        {
            string sharedkeyval = "_"; string sharedvectorval = "____";
            sharedkeyval = ConfigurationManager.AppSettings["sharedkey"];
            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = ConfigurationManager.AppSettings["sharedvector"];
            sharedvectorval = BinaryToString(sharedvectorval);
            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
            return "";
        }
        return Convert.ToBase64String(ms.ToArray());
    }
    public static string Decrypt(string val)
    {
        MemoryStream ms = new MemoryStream();
        //string rsp = "";
        try
        {
            string sharedkeyval = "_"; string sharedvectorval = "____";

            sharedkeyval = ConfigurationManager.AppSettings["sharedkey"];
            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = ConfigurationManager.AppSettings["sharedvector"];
            sharedvectorval = BinaryToString(sharedvectorval);

            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);


            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public static string BinaryToString(string binary)
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


}