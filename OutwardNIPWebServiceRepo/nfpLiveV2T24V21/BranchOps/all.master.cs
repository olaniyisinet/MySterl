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

public partial class BranchOps_all : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["roles"] != null)
        {
            string allowed = "BranchOps";
            string roles = Convert.ToString(Session["roles"]);
            if (roles.Contains(allowed))
            {
                //proceed
            }
            else
            {
                Response.Redirect("../login.aspx");
            }

        }
        else
        {
            Response.Redirect("../login.aspx");
        }
    }
}
