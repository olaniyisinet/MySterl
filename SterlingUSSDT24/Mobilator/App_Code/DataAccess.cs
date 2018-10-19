using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for DataAccess
/// </summary>
public class DataAccess
{

    public static string GetUrl(string msgType)
    {
        string url = "";
        string sql = " select * from USSD_App where msgType = @msgType";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@msgType", msgType);
        DataSet ds = cn.Select();
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count > 0)
        {
            url = dt.Rows[0]["AppUrl"].ToString();
        }
        return url;
    }

    public static string GetUrl(string SessionId, string Msisdn)
    {
        string url = "";
        string sql = " select a.AppUrl from USSD_App a left join tbl_USSD_reqstate r on a.AppId = r.AppId where Msisdn = @Msisdn AND SessionId = @SessionId";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@SessionId", SessionId);
        cn.AddParam("@Msisdn", Msisdn);
        DataSet ds = cn.Select();
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count > 0)
        {
            url = dt.Rows[0]["AppUrl"].ToString();
        }
        return url;
    }
}