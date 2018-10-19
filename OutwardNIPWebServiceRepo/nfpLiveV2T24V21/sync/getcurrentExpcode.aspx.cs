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

public partial class sync_getcurrentExpcode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TransactionService ts = new TransactionService();
        DataSet ds = ts.getCurrentExpcode();
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            Response.Write("The Explanation code currently set is " + dr["expcodeVal"].ToString() + "");
        }
        else
        {
            Response.Write("No Current Explanation code is set");
        }
    }
}
