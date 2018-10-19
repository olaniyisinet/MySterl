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
using System.Text;
using System.IO;
public partial class HeadOps_bulktrans : System.Web.UI.Page
{
    protected swcFormat fm = new swcFormat();
    SqlConnection conn;
    SqlCommand cmd;
    SqlDataReader dr;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        PowerUser p = new PowerUser();
        PowerUserService ps = new PowerUserService();
        p = ps.getUser(User.Identity.Name);

        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["nfp"].ConnectionString);
        string sql = "SELECT distinct batchnumber from tbl_nibbstrans where transnature = 1 and approveddate is null";

        string dtm = DateTime.Now.ToString("yyyyMMdd");
        string dtm2 = DateTime.Now.ToString("d-MMM-yyyy");
        try
        {
            if (Calendar2.SelectedDate.ToString("M/d/yyyy") != "1-Jan-0001")
            {
                dtm = Calendar2.SelectedDate.ToString("yyyyMMdd");
                dtm2 = Calendar2.SelectedDate.ToString("d-MMM-yyyy");
            }
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }

        string binop = Convert.ToString(ddlBinop.SelectedValue);

        int status = Convert.ToInt32(ddlStatus.SelectedValue);
        lblmsg.Text = "All ";
        if (status == 1 || status == 2)
        {
            sql += " and APPROVEVALUE = " + status.ToString();
            lblmsg.Text += fm.getStatus(status.ToString());
        }
        if (status == 0)
        {
            sql += " and APPROVEVALUE in (0) ";
        }
        switch (binop)
        {
            case "any":
                sql += "";
                lblmsg.Text += " transactions ";
                break;
            case "is":
                lblmsg.Text += " transactions created on " + dtm2;
                sql += " and Convert(varchar(50),INPUTDATE,112) between " + dtm + " and " + dtm;
                break;
            case "is before":
                lblmsg.Text += " transactions created before " + dtm2;
                sql += " AND Convert(varchar(50),INPUTDATE,112) < " + dtm + "','mm/dd/yyyy HH:MI:SS PM')) ";
                break;
            case "is after":
                lblmsg.Text += " transactions created after " + dtm2;
                sql += " AND Convert(varchar(50),INPUTDATE,112)" + dtm;
                break;
        }
        sql += " and Orig_bra_code = " + p.bracode;
        sql += " ORDER BY batchnumber DESC";


        cmd = new SqlCommand(sql, conn);
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 0;
        conn.Open();

        dr = cmd.ExecuteReader();
        grddisplay.DataSource = dr;
        grddisplay.DataBind();
        if (dr.HasRows)
        {
            btn_export.Enabled = true;
        }
        else
        {
            btn_export.Enabled = false;
            lblmsg.Visible = true;
            lblmsg.Text = "No record found for the criteria selected. Kindly try another criteria";
        }
        conn.Close();
    }
    private void ExportGridView()
    {
        string attachment = "attachment; filename=nfpreport" + DateTime.Now.ToString() + ".xls";

        Response.ClearContent();

        Response.AddHeader("content-disposition", attachment);

        Response.ContentType = "application/ms-excel";

        StringWriter sw = new StringWriter();

        HtmlTextWriter htw = new HtmlTextWriter(sw);

        grddisplay.RenderControl(htw);

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
    protected void btn_export_Click(object sender, EventArgs e)
    {

    }
}
