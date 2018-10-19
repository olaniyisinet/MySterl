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

public partial class BranchOps_createNewuser : System.Web.UI.Page
{
    PowerUser p = new PowerUser();
    PowerUserService ps = new PowerUserService();
    protected void Page_Load(object sender, EventArgs e)
    {
        InfoBar1.Visible = false;
    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        //validate
        if (txtbracode.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the bracode field is not empty";
            return;
        }
        if (txtusername.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the username field is not empty";
            return;
        }
        if (txttellerid.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the Tellerid field is not empty";
            return;
        }
        if (txtemail.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the e-mail field is not empty";
            return;
        }
        if (txtfirstname.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the firstname field is not empty";
            return;
        }
        if (txtlastname.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the lastname field is not empty";
            return;
        }
        if (ddlstarttime.Text == "0")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that you select a start time for the user";
            return;
        }
        if (ddlendtime.Text == "0")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that you select an end time for the user";
            return;
        }
        //proceed to insert once all conditions are certified
        p.bracode = txtbracode.Text.Trim();
        p.username = txtusername.Text.Trim();
        p.tellerId = txttellerid.Text.Trim();
        p.email = txtemail.Text.Trim();
        p.firstname = txtfirstname.Text.Trim();
        p.lastname = txtlastname.Text.Trim();
        p.starttime = ddlstarttime.SelectedValue;
        p.endtime = ddlendtime.SelectedValue;
        PowerUser u = ps.getUser(User.Identity.Name);
        p.updatedby = u.firstname + ", " + u.lastname;
        p.lastupdated = DateTime.Now;

        int c = ps.AddNewUser(p);
        if (c == 0)
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Unable to create user";
        }
        else if (c == 1)
        {
            InfoBar1.Visible = true;
            InfoBar1.css = true;
            InfoBar1.msg = "User successfully created. Kindly click on <a href='assisgnroles.aspx'> Assign Roles</a> to proceed";
        }
        else if (c == 2)
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "The user you want to create already exist.  kindly click on assign role to assign roles to the user";
        }
            
    }
}
