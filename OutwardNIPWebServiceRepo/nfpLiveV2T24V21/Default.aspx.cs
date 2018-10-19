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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PowerUserService psv = new PowerUserService();
        PowerUser u = psv.getUser(User.Identity.Name);
        PowerRole[] r = psv.getUserRoles(User.Identity.Name);

        lblwelcome.Text = "Welcome: " + u.fullname;

        string s = "<ul>";
        string roles = ",";
        for (int i = 0; i < r.Length; i++)
        {           

            if (r[i].roleName == "BranchOps")
            {
                s += "<div class='fxnBtn'><li><a href='" + r[i].roleName + "'>" + "<img src=Images/buddy.png border=0 />" + r[i].roleDesc + "</a></li></div>";
            }
            else if (r[i].roleName == "Cso")
            {
                s += "<div class='fxnBtn'><li><a href='" + r[i].roleName + "'>" + "<img src=Images/buddy.png border=0 />" + r[i].roleDesc + "</a></li></div>";
            }
            else if (r[i].roleName == "HeadOps")
            {
                s += "<div class='fxnBtn'><li><a href='" + r[i].roleName + "'>" + "<img src=Images/buddy.png border=0 />" + r[i].roleDesc + "</a></li></div>";
            }
            else if (r[i].roleName == "Admin")
            {
                s += "<div class='fxnBtn'><li><a href='" + r[i].roleName + "'>" + "<img src=Images/buddy.png border=0 />" + r[i].roleDesc + "</a></li></div>";
            }
            else if (r[i].roleName == "Domops")
            {
                s += "<div class='fxnBtn'><li><a href='" + r[i].roleName + "'>" + "<img src=Images/buddy.png border=0 />" + r[i].roleDesc + "</a></li></div>";
            }
            else if (r[i].roleName == "ITAudit")
            {
                s += "<div class='fxnBtn'><li><a href='" + r[i].roleName + "'>" + "<img src=Images/buddy.png border=0 />" + r[i].roleDesc + "</a></li></div>";
            }
            roles += r[i].roleName + ",";
        }
        s += "</ul>";

        Label1.Text = s;
        Session["roles"] = roles;

    }
}
