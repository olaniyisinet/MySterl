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
using System.IO;

public partial class sync_getImages : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(Request.Params["id"]);
        if (id < 1)
        {
            return;
        }
        TransactionService tsv = new TransactionService();
        Transaction t = tsv.getTrnxById(id);

        if (t.Refid < 1)
        {
            return;
        }
        string fle = ConfigurationManager.AppSettings["uploadpath"].ToString();
        fle += t.origin_branch + "\\" + t.docfilename;

        if (File.Exists(fle))
        {
           Response.Redirect("/uploads/" + t.origin_branch + "/" + t.docfilename );        
        }
        else
        {
            Literal1.Text = "<div style='font-size:48px'>NO MANDATE FILE EXISTS</div>";
        }
        
    }
}
