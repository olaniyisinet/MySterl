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
        if (mobile.Length >= 10)
        {
            mobile = "234" + mobile.Substring(mobile.Length - 10, 10);
            return mobile;
        }
        return mobile;
    }
    public int insertIntoInfobipSPECTA(string msg, string msdn)
    {
        var sender = ConfigurationManager.AppSettings["Sender"];
        int val = 0; string phone = "";
        phone = ConvertMobile234(msdn);
       

        string sql = "insert into [dbo].[NonTxnNotif] (phone,sender,text) values(@phone,@sender,@msg)";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_infobip_specta");
        cn.SetSQL(sql);
        cn.AddParam("@phone", phone);
        cn.AddParam("@sender", sender);
        cn.AddParam("@msg", msg);
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
    public int insertIntoInfobip(string msg, string msdn)
    {
        int val = 0; string phone = ""; string sender = "STERLING";
        phone = ConvertMobile234(msdn);
        string sql = "insert into proxytable (phone,sender,text) values(@phone,@sender,@msg)";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_infobip");
        cn.SetSQL(sql);
        cn.AddParam("@phone", phone);
        cn.AddParam("@sender", sender);
        cn.AddParam("@msg", msg);
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
}