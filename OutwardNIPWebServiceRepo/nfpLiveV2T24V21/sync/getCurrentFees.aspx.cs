using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class sync_getCurrentFees : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TransactionService ts = new TransactionService();
        DataSet ds = ts.getCurrentFees();
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            Response.Write("The Amounts. Amt1 " + dr["amt1"].ToString() + " Amt2 " + dr["amt2"].ToString() + " Amt3 " + dr["amt3"].ToString() + "");
        }
        else
        {
            Response.Write("No Current AMT is set");
        }
    }
}
