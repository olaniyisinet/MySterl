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

public partial class sync_doReject : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DataSet ds;
        string msg = "";
        PowerUserService ps = new PowerUserService();
        PowerUser p = ps.getUser(User.Identity.Name);
        string tcode = Request.Params["trx"].ToString();
        TransactionService tsv = new TransactionService();
        Transaction t = new Transaction();
        ds = tsv.getTrnxSubRecords(tcode);
        if (ds.Tables[0].Rows.Count > 0)
        {
            //update the table
            t.approvedby = p.fullname;
            t.transactionCode = tcode;

            int cn = tsv.RejectTrans(t);
            if (cn == 1)
            {
                msg = "This transaction has been Rejected.";
                Response.Write(msg);
            }
        }
        else
        {
            msg = "Reject was not successful";
            Response.Write(msg);
        }
    }
}
