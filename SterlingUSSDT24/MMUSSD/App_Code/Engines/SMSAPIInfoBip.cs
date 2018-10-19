using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SMSAPIInfoBip
/// </summary>
public class SMSAPIInfoBip
{
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

    public string insertIntoInfobip(string msg, string msdn)
    {
        string resp = ""; string phone = ""; string sender = "STERLING";
        phone = ConvertMobile234(msdn);
        string sql = "insert into proxytable (phone,sender,text) values(@phone,@sender,@msg)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("smsDB");
        c.SetSQL(sql);
        c.AddParam("@phone", phone);
        c.AddParam("@sender", sender);
        c.AddParam("@msg", msg);
        try
        {
            int cn = c.Execute();
            resp = "1 SMS Sent successfully to the GSM " + phone;
        }
        catch
        {
            resp = "-1 SMS was not Sent successfully to  the GSM " + phone;
        }
        return resp;
    }
}