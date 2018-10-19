using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btn_login_Click(object sender, EventArgs e)
    {
        //start login
        PowerUserService psv = new PowerUserService();
        Gadget g = new Gadget();
        string une = txtusername.Text.Trim().ToLower();
        string pwd = g.enkrypt(txtpassword.Text);
        if (une == "" || pwd == "")
        {
            ibox.css = false;
            ibox.msg = "Empty fields";
            return;
        }      
        if (!psv.login(une, pwd))
        {
            ibox.css = false;
            ibox.msg = "Invalid access details";
            return; // invalid user
        }
        FormsAuthentication.RedirectFromLoginPage(une, false);
    }
}
