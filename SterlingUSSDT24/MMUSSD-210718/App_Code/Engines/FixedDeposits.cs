using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FixedDeposits
/// </summary>
public class FixedDeposits : BaseEngine
{
	public FixedDeposits()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string SaveRequest(UReq req)
    {
        //*822*7*0005969437#
        string resp = "0";
        try
        {
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            addParam("NUBAN", s[2], req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string DoSubmitFD(UReq req)
    {

        string resp = "";
        int k = 0;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            string sql = "Insert into tbl_USSD_FD (mobile,nuban,pin,sessionid) " +
                " values(@mb,@nu,@pn,@sid)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@nu", prm["NUBAN"]);
            cn.AddParam("@pn", req.Msg);
            cn.AddParam("@sid", req.SessionID);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            resp = "Transaction has been submitted for processing";
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
}