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

public partial class sync_doSingleStatusEnquiry : System.Web.UI.Page
{
    protected DataSet ds;
    Gadget g = new Gadget();
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

        if (ds.Tables[0].Rows.Count == 1) //single transaction
        {
            runSingleJob();
            return;
        }
    }
    protected void runSingleJob()
    {
        TR_SingleStatusQuery ssq = new TR_SingleStatusQuery();
        TransactionService tsv = new TransactionService();
        Transaction t = tsv.setTrnx(ds.Tables[0].Rows[0]);
        string msg = "";
        ssq.SessionID = t.sessionid;
        ssq.SourceInstitutionCode = t.outCust.bankcode;
        ssq.ChannelCode = t.channelCode.ToString();
        ssq.createRequest();

        if (ssq.sendRequest())
        {

        }
        else
        {
            msg += "\nNIBSS Error: " + ssq.ResponseCode + ", [" + g.responseCodes(ssq.ResponseCode) + "]";
            Response.Write(msg);
            return;
        }

        if (ssq.ResponseCode == "00")
        {
            msg += "\nNIBSS Response: " + ssq.ResponseCode + ", [" + g.responseCodes(ssq.ResponseCode) + "]";

            //update the table
            tsv.updateTrnxStatusCol(t.sessionid, 4, t.Refid);

            t.approvevalue = 1;
            //proceed to update the table
            t.approveddate = DateTime.Now;
            t.approvedby = User.Identity.Name;
            int count = 0;
            count = tsv.updateTrnxById(t);

            Response.Write(msg);
        }
        else
        {
            msg += "\nError: " + ssq.ResponseCode + ", [" + g.responseCodes(ssq.ResponseCode) + "]";
            Response.Write(msg);
            return;
        }
    }
}
