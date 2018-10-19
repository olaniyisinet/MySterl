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
using System.Threading;
public partial class sync_doReverse : System.Web.UI.Page
{
    protected DataSet ds;
    protected void Page_Load(object sender, EventArgs e)
    {
        string tcode = Request.Params["trx"].ToString();
        TransactionService tsv = new TransactionService();
        ds = tsv.getTrnxSubRecords(tcode);
        if (ds.Tables[0].Rows.Count < 1)
        {
            Response.Write("Invalid Transaction Code");
            return;
        }

        Thread worker = new Thread(new ThreadStart(runJob));
        worker.Start();

        Response.Write("Reversal has been done Successfully!");
    }
    protected void runJob()
    {
        TransactionService tsv = new TransactionService();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            Transaction t = tsv.setTrnx(ds.Tables[0].Rows[i]);

            AccountService acs = new AccountService();
             acs.authorizeTrnxReversal(t);
             if (acs.Respreturnedcode1 == "0")
             {
                 tsv.UpdateTrnxReversal(0, t.Refid);//(t.sessionid, 0, t.nameresponse,t.Refid);
                 //;
             }
             else
             {
                 //Response.Write("Reversal was not Successfully!");
             }
        }
    }
}
