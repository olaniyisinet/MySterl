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

public partial class HeadOps_bulkdetails : System.Web.UI.Page
{
    public Gadget g = new Gadget();
    string batchcode;
    SqlConnection conn;
    SqlCommand cmd;
    SqlDataReader dr;
    SqlDataReader dr2;
    protected void Page_Load(object sender, EventArgs e)
    {
        batchcode = Convert.ToString(Request.QueryString["id"]);
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["nfp"].ConnectionString);
        string sql = "select * from tbl_nibbstrans where transnature = 1  AND APPROVEVALUE=0 and BatchNumber =" + batchcode;
        cmd = new SqlCommand(sql, conn);
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 0;
        conn.Open();

        dr = cmd.ExecuteReader();

        if (dr.HasRows)
        {
            lblmsg.Visible = false;
            grddisplay.Visible = true;
            grddisplay.DataSource = dr;
            grddisplay.DataBind();

        }
        else
        {
            grddisplay.Visible = false;
            lblmsg.Text = "No Batch Record Found.";
        }
        dr.Close();
        dr2 = cmd.ExecuteReader();
        if (dr2.Read())
        {
            if (Convert.ToInt16(dr2["statusflag"]) == 0)
            {
                btn_approve.Enabled = false;
            }
            else if (Convert.ToInt16(dr2["statusflag"]) == 1)
            {
                btn_approve.Enabled = true;
                btn_verify.Enabled = false;
                btn_reject.Enabled = false;
            }
            else if (Convert.ToInt16(dr2["statusflag"]) == 2)
            {
                btn_verify.Enabled = false;
                btn_approve.Enabled = true;
                btn_reject.Enabled = false;
            }
            else if (Convert.ToInt16(dr2["statusflag"]) == 3)
            {
                btn_verify.Enabled = false;
                btn_approve.Enabled = false;
                btn_reject.Enabled = false;
            }
        }
        else
        {

        }
        conn.Close();
    }
    protected void btn_approve_Click(object sender, EventArgs e)
    {
        Transaction t = new Transaction();
        TransactionService ts = new TransactionService();
        TR_BulkFundTransferDC ft = new TR_BulkFundTransferDC();
        int Refid;
        int count = grddisplay.Rows.Count;
        Refid = Convert.ToInt16(grddisplay.Rows[0].Cells[0].Text);
        t = ts.getTrnxById(Refid);

        //first check if the request has been verified already by NIBSS
        int ck = ts.CheckTraStatus(t);
        if (ck == 2)
        {
            ibox.css = false;
            ibox.msg = "This Transaction batch has been already been approved by You";
            btn_approve.Enabled = false;
            return;
        }

        //prepare the xml header
        ft.DestinationBankCode = t.outCust.bankcode;
        ft.NumberOfRecords = count.ToString();
        ft.ChannelCode = t.channelCode.ToString();
        ft.BatchNumber = t.BatchNumber;
        ft.createRequest();

        //prepare the xml body
        foreach (GridViewRow row in grddisplay.Rows)
        {
            ft.addRecord(row.Cells[0].Text, row.Cells[4].Text, row.Cells[3].Text, row.Cells[6].Text, row.Cells[7].Text, row.Cells[8].Text, row.Cells[9].Text);
        }
        ft.makeRequest();
        if (ft.ResponseCode == "09")
        {
            int c = ts.UpdateBulkFundTransfer(t);
            if (c == 1)
            {
                ibox.css = true;
                ibox.msg = "Request processing in progress. Update to database was successful";
            }
            else
            {
                ibox.css = false;
                ibox.msg = "Update was not successful";
            }
        }
    }
    protected void btn_reject_Click(object sender, EventArgs e)
    {

    }
    protected void btn_verify_Click(object sender, EventArgs e)
    {
        Transaction t = new Transaction();
        TransactionService ts = new TransactionService();
        TR_BulkNameEnquiry bn = new TR_BulkNameEnquiry();
        
        int Refid;
        int count = grddisplay.Rows.Count;

        Refid = Convert.ToInt16(grddisplay.Rows[0].Cells[0].Text);
        t = ts.getTrnxById(Refid);
        
        //first check if the request has been verified already by NIBSS
        int ck = ts.CheckTraStatus(t);
        if (ck == 1)
        {
            ibox.css = false;
            ibox.msg = "This Transaction batch has been verified by NIBSS and it is still been processed";
            btn_verify.Enabled = false;
            return;
        }
        //prepare the xml header
        bn.DestinationBankCode = t.outCust.bankcode;
        bn.NumberOfRecords = count.ToString();
        bn.ChannelCode = t.channelCode.ToString();
        bn.BatchNumber = t.BatchNumber;
        bn.createRequest();

        //Prepare the xml body
        foreach (GridViewRow row in grddisplay.Rows)
        {
            bn.addRecord(row.Cells[0].Text, row.Cells[4].Text);
            bn.sendRequest();
        }
        if (bn.ResponseCode == "09") //this confirms that NIBBS has gotten the bulk request and we await a response from NIBSS
        {
            int c = ts.spd_UpdateBulkAprroved09Txn(t);
            if (c == 1)
            {
                ibox.css = true;
                ibox.msg = "Request processing in progress. Update to database was successful";
            }
            else
            {
                ibox.css = false;
                ibox.msg = "Update was not successful";
            }
        }
        else
        {
            ibox.css = false;
            ibox.msg = "Error: " + bn.ResponseCode + ", [" + g.responseCodes(bn.ResponseCode) + "]";
        }
    }
}
