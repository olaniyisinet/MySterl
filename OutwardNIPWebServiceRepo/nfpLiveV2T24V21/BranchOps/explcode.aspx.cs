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

public partial class BranchOps_explcode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        Gadget g = new Gadget();
        PowerUserService psv = new PowerUserService();
        PowerUser u = psv.getUser(User.Identity.Name);
        TransactionService ts = new TransactionService();
        Transaction t = new Transaction();
        if (txtexpcode.Text == "")
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Kindly ensure that the explanation code field is not empty";
            return;
        }
        if (!g.validatenum(txtexpcode.Text))
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the explanation code field does not contain alphanumeric";
            return;
        }

        t.expcode = txtexpcode.Text;
        t.Addedby = u.fullname;
        int c = ts.AddNewExpcode(t);
        if (c == 1)
        {
            InfoBar1.css = true;
            InfoBar1.msg = "Explanation Code has been added";
        }
        else
        {
            InfoBar1.css = false;
            InfoBar1.msg = "The Explanation code was not added.  Kindly contact the administrator";
        }
        //proceed

    }
}
