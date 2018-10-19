using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for GetActualBankCode
/// </summary>
public class GetActualBankCode
{
    public string getNewBankCode(string bc)
    {
        string NewBC = "";
        string sql = "SELECT refid, bankcode, old_bankcode FROM  tbl_participatingBanks where old_bankcode =@obc";
        Connect c = new Connect(sql, true);
        c.addparam("@obc", bc);
        DataSet ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            NewBC = dr["bankcode"].ToString();
        }
        return NewBC;
    }
}
