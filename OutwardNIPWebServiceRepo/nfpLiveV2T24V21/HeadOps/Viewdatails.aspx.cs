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
using Oracle.DataAccess.Client;
using System.Net;
using System.Net.Mail;
using System.Xml;
using System.Text;

public partial class HeadOps_Viewdatails : System.Web.UI.Page
{
    public int id;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            id = Convert.ToInt32(Request.QueryString["id"]);
            AccountService asv = new AccountService();
            Gadget g = new Gadget();

            TransactionService tsv = new TransactionService();
            Transaction t = new Transaction();
            t = tsv.getTrnxById(id);
            if (t.Refid > 0)
            {
                //set transaction values
                lblbranchcode.Text = t.origin_branch;
                lblcusname.Text = t.inCust.cus_sho_name;
                lblcusacct.Text = t.inCust.fullaccount();
                lblinputername.Text = t.inputedby;
                lbldocument.Text = t.formnum;
                lblbenname.Text = t.outCust.cusname;
                lblbenacctnum.Text = t.outCust.accountnumber;
                Lblbenbank.Text = t.outCust.bankname;
                lblamt.Text = string.Format("{0:#.##}", t.amount);
                lblamtinword.Text = t.amountWords;
                lbltransdate.Text = t.inputdate.ToString();
                Session["branch_email_CustomerServe"] = t.csoemail;              

                
                //check concession
                asv.checkConsession(t.inCust);                                
                t.feecharge = asv.calculateFee(t.amount);            
                lblFee.Text = t.feecharge.ToString();

                t.inCust = asv.getBalance(t.inCust);

                lblcurbal.Text = g.printMoney(t.inCust.cle_bal);

                if (t.approvevalue > 0)
                {
                    btn_approve.Enabled = false;
                    btn_reject.Enabled = false;
                }

                
                hypblob1.Target = "_blank";
                hypblob1.NavigateUrl = "../sync/getImages.aspx?id=" + id.ToString();
            }
            else
            {
                //
            }
        }

    }

    protected void btn_approve_Click(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(Request.QueryString["id"]);
        PowerUserService ps = new PowerUserService();
        PowerUser p = ps.getUser(User.Identity.Name);
        string ledger = "";
        Gadget g = new Gadget();

        TransactionService tsv = new TransactionService();
        Transaction t = new Transaction();
        t = tsv.getTrnxById(id);

        AccountService asv = new AccountService();
        //go and get the operational fee/income ledger from database
        DataSet dsledger = tsv.getCurrentIncomeAcct();
        if (dsledger.Tables[0].Rows.Count > 0)
        {
            DataRow drledger = dsledger.Tables[0].Rows[0];
            ledger = drledger["ledcode"].ToString();
        }
        //ensure that the 489 and 8720 for the approving branch exist
        if (!asv.checkTSSandFeesAccount(ledger, p.bracode))
        {
            //accounts are not okay
            lblmsg.Text = "kindly Contact the Branch Operation that Ledger:8720 has not been activated.  This transaction will not be processed.";
            return;
        }
        if (!asv.tssACCok) //is TSS ok?
        {
            //print err
            lblmsg.Text = "Kindly verify that your Branch's Clearing Account is active";
            return;//stop
        }

        if (!asv.feeACCok) //is Fee ok?
        {
            //print err
            lblmsg.Text = "Kindly verify that your Branch's Nibbs Income Account is active";
            return;//stop
        }

        //if the two accounts exist then proceed
        //Second Check::go back to BANKS and find out if the customer balance is still enough for this transaction               

        t.tellerID = p.tellerId;
        //compute the amount to pay
        t.feecharge = asv.calculateFee(t.amount);
       // t.totalamttopay = t.amount + t.feecharge;

        //get customer balance
        t.inCust = asv.getBalance(t.inCust);

        lblcurbal.Text = g.printMoney(t.inCust.cle_bal);

        //Check to see that the Amount(totalamttopay) does not exceed the Approval limit for the approving HOP
        bool limit = asv.checkTellerLimit(p.bracode, p.tellerId, t.amount);
        if (!limit)
        {
            lblmsg.Text = "Transaction has been aborted!<br/>Reason:The amount your are trying to approve ";
            lblmsg.Text += "is above your approval limit.  Amount: " + t.amount.ToString("#,###.00");
            lblmsg.Text += " your limit: " + asv.thelimitval.ToString("#,###.00");
            lblmsg.ForeColor = System.Drawing.Color.Red;
            return;
        }

        if (t.inCust.cle_bal < t.amount + t.feecharge)
        {
            lblmsg.Text = "This process did not complete.  Customer's Balance is insufficient for this transaction.  Kindly advice the customer to fund his/her account";
            return;
        }
       
        //debit the customer for principal and charge
        asv.authorizeTrnxFromSterling(t);
        if(asv.Respreturnedcode1 != "0")
        {            
            ibox.css = false;
            ibox.msg = "Unable to debit customer's account for Principal";
            return;
        }

        ibox.css = true;
        ibox.msg = "Customer has been debitted!";

        if (asv.Respreturnedcode2 != "0")
        {
            ibox.msg += " <br/> But fees where not taken";
        }
        
        //start building the transfer request and send to NIBBS
        TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();
        sft.SessionID = t.sessionid;
        sft.DestinationInstitutionCode = t.outCust.bankcode;
        sft.ChannelCode = t.channelCode.ToString();
        sft.BeneficiaryAccountName = t.outCust.cusname;
        sft.BeneficiaryAccountNumber = t.outCust.accountnumber;
        sft.OriginatorAccountName = t.inCust.cus_sho_name;
        sft.Narration = t.Remark;
        sft.PaymentReference = t.paymentRef;
        sft.Amount = t.amount.ToString();
        sft.createRequest();

        if (!sft.sendRequest())
        {
            ibox.css = false;
            ibox.msg += "<br />Error: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            return;
        }

        if (sft.ResponseCode != "00")
        {
            ibox.css = false;
            ibox.msg += "<br />Error: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            return;
        }

        ibox.css = true;
        ibox.msg += "<br />Response: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
        
        
        //proceed to update the table
        t.approveddate = DateTime.Now;
        t.approvedby = User.Identity.Name;
        t.approvevalue = 1;
        int count = 0;
        count = tsv.updateTrnxById(t);

        Response.Redirect("hopapprovesuccesful.aspx");
    }

    protected void btn_reject_Click(object sender, EventArgs e)
    {
        Response.Redirect("hopreject.aspx?id=" + id);
    }
    
    
}
