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

public partial class sync_doNameRequest : System.Web.UI.Page
{

    protected DataSet ds;
    protected void Page_Load(object sender, EventArgs e)
    {
        //run a background name request for a set of transactions
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

        Response.Write("Transaction has been sent!");

    }
    protected void runJob()
    {
        Gadget g = new Gadget();
        TransactionService tsv = new TransactionService();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            Transaction t = tsv.setTrnx(ds.Tables[0].Rows[i]);
            string NewSessionid = g.newSessionId(t.outCust.bankcode);
            TR_SingleNameEnquiry sne = new TR_SingleNameEnquiry();
            sne.SessionID = NewSessionid;// t.sessionid;
            sne.DestinationInstitutionCode = t.outCust.bankcode;
            sne.ChannelCode = Convert.ToInt16(ChannelCode.Bank_Teller).ToString();
            sne.AccountNumber = t.outCust.accountnumber;
            sne.createRequest();

            if (sne.sendRequest()) //successful request
            {
                if (sne.ResponseCode == "00")
                {
                    //update name in database
                    //tsv.updateTrnxStatus(t.sessionid, 2, sne.AccountName);
                    tsv.updateTrnxStatus(2, sne.AccountName,t.Refid);
                }
            }
        }

        //mail cso!
    }

}
