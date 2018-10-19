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

public partial class BranchOps_assisgnroles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InfoBar1.Visible = false;
    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        PowerUser p = new PowerUser();
        PowerUserService ps = new PowerUserService();
        string username;
        string container ="";
        int count = 0;
        int c=0;
        //first drop users old role if exist
        username = ddlusername.SelectedValue;
        p.username = username;
        ps.DropOldRoles(p);
        for (int i = 0; i < chklist.Items.Count; i++)
        {
            if (chklist.Items[i].Selected)
            {
                p.Usertypeid = Convert.ToInt16(chklist.Items[i].Value);
                c = ps.AssignRoles(p);
                count++;

                container += chklist.Items[i].Value + ",";
            }
        }
        if (count == 0)
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "You need to select a Role for the user.";
        }
        else
        {
            //proceed
            if (c == 1)
            {
                InfoBar1.Visible = true;
                InfoBar1.css = true;
                InfoBar1.msg = "Roles have been assigned successfully to " + username;
            }
            else if (c == 2)
            {
                InfoBar1.Visible = true;
                InfoBar1.css = false;
                InfoBar1.msg = "No role was assigned. Because the user already have the role(s) selected";
            }
        }
    }
}
