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
using System.Text;
public partial class ITAudit_Reportson : System.Web.UI.Page
{
    public Gadget g = new Gadget();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            calendar1.iDay = DateTime.Now.Day.ToString();
            calendar1.iMonth = DateTime.Now.Month.ToString();
            calendar1.iYear = DateTime.Now.Year.ToString();

            calendar2.iDay = DateTime.Now.Day.ToString();
            calendar2.iMonth = DateTime.Now.Month.ToString();
            calendar2.iYear = DateTime.Now.Year.ToString();

            PowerUserService psv = new PowerUserService();
            PowerUser p = psv.getUser(User.Identity.Name);
            Session["bracode"] = p.bracode;

            if (gvTransactions.Rows.Count == 0)
            {
                btn_export.Enabled = false;
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        lblmsg.Text = "";
        if (txtsessionid.Text == "")
        {
            string sql = "select sessionid,channelcode,paymentRef,mandateRefNum,BillerID, " +
                         "inputedby,inputdate,approvedby,approveddate,BillerName,benebank,benename,amt,bra_code + cus_num + led_code + sub_acct_code as cusnum,accname, " +
                         "Remark,inputdate from tbl_nibbstrans where Approvevalue = 1 and inputdate > @sdate and inputdate < @edate ";
            try
            {
                Connect c = new Connect(sql, true);
                c.addparam("sdate", calendar1.getDate());
                c.addparam("edate", calendar2.getDate().AddDays(1));
                DataSet ds = c.query("recs");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    gvTransactions.Visible = true;
                    btn_export.Enabled = true;
                    gvTransactions.DataSource = ds;
                    gvTransactions.DataBind();
                    lblmsg.Text = "Search Result between " + String.Format("{0:MM/dd/yyyy}", calendar1.getDate()) + " and " + String.Format("{0:MM/dd/yyyy}", calendar2.getDate());
                }
                else
                {
                    gvTransactions.Visible = false;
                    btn_export.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog err = new ErrorLog(ex);
            }
        }
        else
        {

            string sql = "select sessionid,channelcode,paymentRef,mandateRefNum,BillerID, " +
                       "inputedby,inputdate,approvedby,approveddate,BillerName,benebank,benename,amt,bra_code + cus_num + led_code + sub_acct_code as cusnum,accname, " +
                       "Remark,inputdate from tbl_nibbstrans where Approvevalue = 1 and inputdate > @sdate and inputdate < @edate and sessionid = @id ";

            try
            {
                Connect c = new Connect(sql, true);
                c.addparam("id", txtsessionid.Text.Trim());
                c.addparam("sdate", calendar1.getDate());
                c.addparam("edate", calendar2.getDate().AddDays(1));
                DataSet ds = c.query("recs");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    gvTransactions.Visible = true;
                    btn_export.Enabled = true;
                    gvTransactions.DataSource = ds;
                    gvTransactions.DataBind();
                    lblmsg.Text = "Search Result between " + String.Format("{0:MM/dd/yyyy}", calendar1.getDate()) + " and " + String.Format("{0:MM/dd/yyyy}", calendar2.getDate());
                }
                else
                {
                    gvTransactions.Visible = false;
                    btn_export.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                ErrorLog err = new ErrorLog(ex);
            }
        }
    }
    protected void btn_export_Click(object sender, EventArgs e)
    {
        PrepareGridViewForExport(gvTransactions);
        ExportGridView();
    }
    private void ExportGridView()
    {
        string attachment = "attachment; filename=nibssIntReport" + DateTime.Now.ToString() + ".xls";

        Response.ClearContent();

        Response.AddHeader("content-disposition", attachment);

        Response.ContentType = "application/ms-excel";

        StringWriter sw = new StringWriter();

        HtmlTextWriter htw = new HtmlTextWriter(sw);

        gvTransactions.RenderControl(htw);

        Response.Write(sw.ToString());

        Response.End();

    }
    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    private void PrepareGridViewForExport(Control gv)
    {

        LinkButton lb = new LinkButton();

        Literal l = new Literal();

        string name = String.Empty;

        for (int i = 0; i < gv.Controls.Count; i++)
        {

            if (gv.Controls[i].GetType() == typeof(LinkButton))
            {

                l.Text = (gv.Controls[i] as LinkButton).Text;

                gv.Controls.Remove(gv.Controls[i]);

                gv.Controls.AddAt(i, l);

            }

            else if (gv.Controls[i].GetType() == typeof(DropDownList))
            {

                l.Text = (gv.Controls[i] as DropDownList).SelectedItem.Text;

                gv.Controls.Remove(gv.Controls[i]);

                gv.Controls.AddAt(i, l);

            }

            else if (gv.Controls[i].GetType() == typeof(CheckBox))
            {

                l.Text = (gv.Controls[i] as CheckBox).Checked ? "True" : "False";

                gv.Controls.Remove(gv.Controls[i]);

                gv.Controls.AddAt(i, l);

            }

            if (gv.Controls[i].HasControls())
            {

                PrepareGridViewForExport(gv.Controls[i]);

            }

        }

    }
}
