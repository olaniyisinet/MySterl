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

public partial class Domops_trnxSummary : System.Web.UI.Page
{
    Gadget g = new Gadget();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public string getBankName(string code)
    {
        return g.GetBankNames(code);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        lblmsg.Text = "";
        string sql = "select senderbankcode as BankName,count(senderbankcode) as Total from tbl_WStrans " +
                     "where Approvevalue = 1 and inputdate > @sdate and inputdate < @edate group by senderbankcode ";
        try
        {
            Connect c = new Connect(sql, true);
            c.addparam("sdate", calendar1.getDate());
            c.addparam("edate", calendar2.getDate().AddDays(1));
            DataSet ds = c.query("recs");
            gvTransactions.DataSource = ds;
            gvTransactions.DataBind();
            lblmsg.Text = "Search Result between " + String.Format("{0:MM/dd/yyyy}", calendar1.getDate()) + " and " + String.Format("{0:MM/dd/yyyy}", calendar2.getDate());
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
    }
}
