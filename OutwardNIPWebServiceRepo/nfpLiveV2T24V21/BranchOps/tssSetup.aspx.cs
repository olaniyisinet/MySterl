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

public partial class BranchOps_tssSetup : System.Web.UI.Page
{
    Gadget g = new Gadget();
    protected void Page_Load(object sender, EventArgs e)
    {
        InfoBar1.Visible = false;
    }
    protected void btn_submit_Click(object sender, EventArgs e)
    {
        PowerUserService psv = new PowerUserService();
        PowerUser u = psv.getUser(User.Identity.Name);
        TransactionService ts = new TransactionService();
        Transaction t = new Transaction();
        string tssacctnum;
        string addedby;
        //validate controls
        if (txtbracode.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the bracode field is not empty";
            return;
        }
        if (txtcusnum.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the cusnum field is not empty";
            return;
        }
        if (!g.validatenum(txtcusnum.Text))
        {
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the cusnum field does not contain alphanumeric";
            return;
        }
        if (txtcurrency.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the currency field is not empty";
            return;
        }
        if (!g.validatenum(txtcurrency.Text))
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the currency field does not contain alphanumeric";
            return;
        }
        if (txtledger.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the Ledger field is not empty";
            return;
        }
        if (!g.validatenum(txtledger.Text))
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the Ledger field does not contain alphanumeric";
            return;
        }
        if (txtsubact.Text == "")
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the subaccount field is not empty";
            return;
        }
        if (!g.validatenum(txtsubact.Text))
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "Make sure that the subaccount field does not contain alphanumeric";
            return;
        }
        //proceed to insert
        tssacctnum =  txtcusnum.Text + txtcurrency.Text + txtledger.Text + txtsubact.Text;
        addedby = u.firstname + ", " + u.lastname;

        t.bra_code = txtbracode.Text;
        t.cusnum = txtcusnum.Text;
        t.curcode = txtcurrency.Text;
        t.ledcode = txtledger.Text;
        t.subacctcode = txtsubact.Text;
        t.Addedby = addedby;

        int c = ts.AddNewTssAccount(t);
        if (c == 1)
        {
            InfoBar1.Visible = true;
            InfoBar1.css = true;
            InfoBar1.msg = "The new TSS account " + tssacctnum + " has been added successfully";
        }
        else
        {
            InfoBar1.Visible = true;
            InfoBar1.css = false;
            InfoBar1.msg = "The New TSS account was not added.  Kindly contact the administrator";
        }
    }
}
