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

public partial class BranchOps_managefee : System.Web.UI.Page
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
        if (txtamt1.Text == "")
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the amt1 field is not empty";
            return;
        }
        if (!g.validatenum(txtamt1.Text))
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the amt1 field does not contain alphanumeric";
            return;
        }

        if (txtamt2.Text == "")
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the amt2 field is not empty";
            return;
        }
        if (!g.validatenum(txtamt2.Text))
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the amt2 field does not contain alphanumeric";
            return;
        }

        if (txtamt3.Text == "")
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the amt3 field is not empty";
            return;
        }
        if (!g.validatenum(txtamt3.Text))
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the amt3 field does not contain alphanumeric";
            return;
        }

        t.amt1 = decimal.Parse(txtamt1.Text);
        t.amt2 = decimal.Parse(txtamt2.Text);
        t.amt3 = decimal.Parse(txtamt3.Text);
        t.Addedby = u.fullname;
        int cn = ts.AddNewFees(t);
        if (cn == 1)
        {
            InfoBar1.Visible = true;
            InfoBar1.css = true;
            InfoBar1.msg = "The New Fee " + "amt1: " + txtamt1.Text + " amt2: " + txtamt2.Text + " amt3 " + txtamt3.Text + " has been added successfully";
        }
        else
        {
            InfoBar1.Visible = true;
            InfoBar1.css = true;
            InfoBar1.msg = "Sorry, the new amounts were not added successfully";

        }
    }
}
