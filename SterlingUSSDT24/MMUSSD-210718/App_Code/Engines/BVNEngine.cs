using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BVNEngine
/// </summary>
public class BVNEngine : BaseEngine
{
	public BVNEngine()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string DisplayProd(UReq req)
    {
        string sql = ""; DataSet ds = new DataSet();
        sql = "select prodid, prodname from tbl_USSD_Acct_Categ where statusflag = 1";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        ds = cn.Select("rec");
        string resp = "";
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            DataRow dr = ds.Tables[0].Rows[i];
            resp += " %0A " + dr["prodid"].ToString() + ". " + dr["prodname"].ToString();
        }
        return "Select the Type of account you will like to have with us: " + resp;
    }
    public string SaveRequest(UReq req)
    {
        //*822*13*12345678901
        string resp = "0";
        try
        {
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            addParam("BVN", s[2], req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveProduct(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("PROD", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SubmitBVN(UReq req)
    {
        string resp = ""; string bvn = ""; int k = 0; int prod = 0;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            string SessionID = req.SessionID; string Msisdn = req.Msisdn;
            bvn = prm["BVN"];
            prod = int.Parse(prm["PROD"]);
            if(bvn.Length == 11)
            {
                string sql = "insert into tbl_USSD_acct_open(Sessionid,mobile,BVN,Product) " +
                    " values (@sid,@mb,@bv,@pd)";
                Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
                cn.SetSQL(sql);
                cn.AddParam("@sid", SessionID);
                cn.AddParam("@mb", Msisdn);
                cn.AddParam("@bv", bvn);
                cn.AddParam("@pd", prod);
                k = Convert.ToInt32(cn.Insert());
                if (k > 0)
                {
                    resp = "Sterling Bank %0AThe account opening request has been submitted for processing. Your account will be sent to you via SMS";
                }
                else
                {
                    resp = "Unable to submit please try again";
                }
            }
            else
            {
                resp = "The BVN entered is not valid.  Kindly enter a valid BVN";
            }
        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system  " + ex.ToString();
        }

        return resp;
    }
}