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

public partial class sync_doAuthorize : System.Web.UI.Page
{
    protected DataSet ds;
    protected TransactionSchedule ts = new TransactionSchedule();
    protected TransactionBatch[] tb;
    protected Transaction[] tx;

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
        /* to authorize-------------------------
         * Debit customer's account
         * credit payable
         * send message to NFP
         */

        if (ds.Tables[0].Rows.Count == 1) //single transaction
        {
            DataRow dr = ds.Tables[0].Rows[0];
            if (dr["mandateRefNum"].ToString() == "0")
            {
                runSingleJob();
                return;
            }
            else
            {
                runSingleJob_dd();
                return;
            }
        }

        if (ds.Tables[0].Rows.Count > 1) // batch transaction
        {
            //Thread worker = new Thread(new ThreadStart(runBulkJob));
            //worker.Start();
            //runBulkJob();
        }
    }

    protected void runSingleJob()
    {
        PowerUserService ps = new PowerUserService();
        PowerUser p = ps.getUser(User.Identity.Name);
        Gadget g = new Gadget();
        TransactionService tsv = new TransactionService();
        Transaction t = tsv.setTrnx(ds.Tables[0].Rows[0]);
        AccountService asv = new AccountService();
        string msg = "";
        string ledger = "";
        t.approvevalue = 0;

        //go and get the operational fee/income ledger from database
        DataSet dsledger = tsv.getCurrentIncomeAcct();
        if (dsledger.Tables[0].Rows.Count > 0)
        {
            DataRow drledger = dsledger.Tables[0].Rows[0];
            ledger = drledger["ledcode"].ToString();
        }
        //ensure that the TSS and Income ledger for the approving branch exist
        if (!asv.checkTSSandFeesAccount(ledger, p.bracode))
        {
            //accounts are not okay
            msg = "ERROR! \nKindly Contact the Branch Operation that Ledger:8720 has not been activated.  This transaction will not be processed.";
            Response.Write(msg);
            return;
        }
        if (!asv.tssACCok) //is TSS ok?
        {
            //print err
            msg = "ERROR! \nKindly verify that your Branch's Clearing Account is active";
            Response.Write(msg);
            return; //stop
        }

        if (!asv.feeACCok) //is Fee ok?
        {
            //print err
            msg = "Kindly verify that your Branch's Nibss Income Account is active";
            Response.Write(msg);
            return; //stop
        }

        //if the two accounts exist then proceed
        //Second Check::go back to BANKS and find out if the customer balance is still enough for this transaction               

        t.tellerID = p.tellerId;
        //compute the amount to pay
        t.feecharge = asv.calculateFee(t.amount);
        // t.totalamttopay = t.amount + t.feecharge;

        //get customer balance
        t.inCust = asv.getBalance(t.inCust);

        //lblcurbal.Text = g.printMoney(t.inCust.cle_bal);

        //Check to see that the Amount(totalamttopay) does not exceed the Approval limit for the approving HOP
        bool limit = asv.checkTellerLimit(p.bracode, p.tellerId, t.amount);
        if (!limit)
        {
            msg = "Transaction has been aborted!<br/>Reason:The amount your are trying to approve ";
            msg += "is above your approval limit.  Amount: " + t.amount.ToString("#,###.00");
            msg += " your limit: " + asv.thelimitval.ToString("#,###.00");
            //lblmsg.ForeColor = System.Drawing.Color.Red;
            Response.Write(msg);
            return;
        }

        //if (t.inCust.cle_bal < t.amount + t.feecharge)
        if (t.inCust.avail_bal < t.amount + t.feecharge)
        {
            msg = "This process did not complete. Customer's Balance is insufficient for this transaction.  Kindly advice the customer to fund his/her account";
            Response.Write(msg);
            return;
        }
        string NewSessionid = g.newSessionId(t.outCust.bankcode);
        t.sessionid = NewSessionid;
        //debit the customer for principal and charge
        asv.authorizeTrnxFromSterling(t);

        if (asv.Respreturnedcode1 != "0")
        {
            //ibox.css = false;
            msg = "Unable to debit customer's account for Principal";
            Response.Write(msg);
            return;
        }

        //ibox.css = true;
        msg = "Customer has been debitted!";

        if (asv.Respreturnedcode2 != "0")
        {
            msg += "\nBut fees where not taken!";
            t.feecharge = 0;
        }

        //string NewSessionid = g.newSessionId(t.outCust.bankcode);
        //start building the transfer request and send to NIBBS
        TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();
        sft.SessionID = NewSessionid;// t.sessionid;
        sft.DestinationInstitutionCode = t.outCust.bankcode;
        sft.ChannelCode = t.channelCode.ToString();
        sft.BeneficiaryAccountName = t.outCust.cusname;
        sft.BeneficiaryAccountNumber = t.outCust.accountnumber;
        sft.OriginatorAccountName = t.inCust.cus_sho_name;
        sft.Narration = t.Remark;
        sft.PaymentReference = t.paymentRef;
        sft.Amount = t.amount.ToString("#,##0.00");
        sft.createRequest();

        if (sft.sendRequest())
        {
            //transaction has reached nibss
            //tsv.updateTrnxStatus(t.sessionid, 3);
            tsv.updateTrnxStatusCol(NewSessionid, 3, t.Refid);
        }
        else
        {
            //ibox.css = false;
            msg += "\nNIBSS Error: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            tsv.updateTrnxStatusCol("", 31, t.Refid);
            Response.Write(msg);
            return;
        }

        if (sft.ResponseCode == "00")
        {
            msg += "\nNIBSS Response: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            tsv.updateTrnxStatusCol(NewSessionid, 4, t.Refid);

            t.approvevalue = 1;
            Response.Write(msg);
        }
        else
        {
            msg += "\nError: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            tsv.updateTrnxStatusCol("", 32, t.Refid);
            Response.Write(msg);
            return;
        }


        t.sessionid = NewSessionid;
        //proceed to update the table
        t.approveddate = DateTime.Now;
        t.approvedby = User.Identity.Name;
        int count = 0;
        count = tsv.updateTrnxById(t);

        //mail cso!
        //mail hop
        // Thread worker = new Thread(new ThreadStart(runSingleJob));
        //worker.Start();
    }

    protected void runSingleJob_dd()
    {
        PowerUserService ps = new PowerUserService();
        PowerUser p = ps.getUser(User.Identity.Name);
        Gadget g = new Gadget();
        TransactionService tsv = new TransactionService();
        Transaction t = tsv.setTrnx(ds.Tables[0].Rows[0]);
        AccountService asv = new AccountService();
        string msg = "";
        string ledger = "";
        t.approvevalue = 0;

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
            msg = "ERROR! \nKindly Contact the Branch Operation that Ledger:8720 has not been activated.  This transaction will not be processed.";
            Response.Write(msg);
            return;
        }
        if (!asv.tssACCok) //is TSS ok?
        {
            //print err
            msg = "ERROR! \nKindly verify that your Branch's Clearing Account is active";
            Response.Write(msg);
            return; ;//stop
        }

        if (!asv.feeACCok) //is Fee ok?
        {
            //print err
            msg = "Kindly verify that your Branch's Nibss Income Account is active";
            Response.Write(msg);
            return; ;//stop
        }

        //if the two accounts exist then proceed
        //Second Check::go back to BANKS and find out if the customer balance is still enough for this transaction               

        t.tellerID = p.tellerId;
        //compute the amount to pay
        t.feecharge = asv.calculateFee(t.amount);
        // t.totalamttopay = t.amount + t.feecharge;

        //get customer balance
        t.inCust = asv.getBalance(t.inCust);

        //lblcurbal.Text = g.printMoney(t.inCust.cle_bal);

        //Check to see that the Amount(totalamttopay) does not exceed the Approval limit for the approving HOP
        bool limit = asv.checkTellerLimit(p.bracode, p.tellerId, t.amount);
        if (!limit)
        {
            msg = "Transaction has been aborted!<br/>Reason:The amount your are trying to approve ";
            msg += "is above your approval limit.  Amount: " + t.amount.ToString("#,###.00");
            msg += " your limit: " + asv.thelimitval.ToString("#,###.00");
            //lblmsg.ForeColor = System.Drawing.Color.Red;
            Response.Write(msg);
            return;
        }

        if (t.inCust.cle_bal < t.amount + t.feecharge)
        {
            msg = "This process did not complete. Customer's Balance is insufficient for this transaction.  Kindly advice the customer to fund his/her account";
            Response.Write(msg);
            return;
        }
        string NewSessionid = g.newSessionId(t.outCust.bankcode);
        t.sessionid = NewSessionid;
        //debit the customer for principal and charge
        asv.authorizeTrnxFromSterling(t);

        if (asv.Respreturnedcode1 != "0")
        {
            //ibox.css = false;
            msg = "Unable to debit customer's account for Principal";
            Response.Write(msg);
            return;
        }

        //ibox.css = true;
        msg = "Customer has been debitted!";

        if (asv.Respreturnedcode2 != "0")
        {
            msg += "\nBut fees where not taken!";
            t.feecharge = 0;
        }


        //start building the transfer request and send to NIBBS
        TR_SingleFundTransferDD sft = new TR_SingleFundTransferDD();
        sft.SessionID = NewSessionid;// t.sessionid;
        sft.DestinationBankCode = t.outCust.bankcode;
        sft.ChannelCode = t.channelCode.ToString();
        sft.AccountName = t.outCust.cusname;
        sft.AccountNumber = t.outCust.accountnumber;
        sft.BillerName = t.billerName;
        sft.BillerID = t.billerId;
        sft.Narration = t.Remark;
        sft.PaymentReference = t.paymentRef;
        sft.MandateReferenceNumber = t.mandateRefNum;
        sft.Amount = t.amount.ToString();
        sft.createRequest();

        if (sft.sendRequest())
        {
            //transaction has reached nibss
            //tsv.updateTrnxStatus(t.sessionid, 3);
            tsv.updateTrnxStatusCol(NewSessionid, 3, t.Refid);
        }
        else
        {
            //ibox.css = false;
            msg += "\nNIBSS Error: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            tsv.updateTrnxStatusCol("", 31, t.Refid);
            Response.Write(msg);
            return;
        }

        if (sft.ResponseCode == "00")
        {
            msg += "\nNIBSS Response: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            tsv.updateTrnxStatusCol(NewSessionid, 4, t.Refid);

            t.approvevalue = 1;
            Response.Write(msg);
        }
        else
        {
            msg += "\nError: " + sft.ResponseCode + ", [" + g.responseCodes(sft.ResponseCode) + "]";
            tsv.updateTrnxStatusCol("", 32, t.Refid);
            Response.Write(msg);
            return;
        }


        t.sessionid = NewSessionid;
        //proceed to update the table
        t.approveddate = DateTime.Now;
        t.approvedby = User.Identity.Name;
        int count = 0;
        count = tsv.updateTrnxById(t);

        //mail cso!
        //mail hop
        // Thread worker = new Thread(new ThreadStart(runSingleJob));
        //worker.Start();
    }

    //protected void runBulkJob()
    //{
    //    Gadget g = new Gadget();
    //    DataTable dtTrx = ds.Tables[0];
    //    TransactionService tsv = new TransactionService();
    //    AccountService asv = new AccountService();
    //    string msg = "";
    //    //sum batch
    //    decimal totalamt = 0;
    //    for (int i = 0; i < dtTrx.Rows.Count; i++)
    //    {
    //        Transaction t = tsv.setTrnx(dtTrx.Rows[i]); //new Transaction();
    //        totalamt += t.amount + t.feecharge;
    //    }

    //    Transaction xt = tsv.setTrnx(dtTrx.Rows[0]); //new Transaction();

    //    Account fund = new Account();
    //    fund = asv.getBalance(xt.inCust);

    //    //chcek account status
    //    if (fund.status == 1)
    //    {
    //        msg = "Customer's Account is not active";
    //        Response.Write(msg);
    //        return;
    //    }

    //    //check if the customer can fund the account
    //    if (fund.cle_bal < totalamt)
    //    {
    //        msg = "Customer's Account cannot fund this transaction batch";
    //        msg += "\nCustomer: " + g.printMoney(fund.cle_bal);
    //        msg += "\nTransaction: " + g.printMoney(totalamt);

    //        Response.Write(msg);
    //        return;
    //    }

    //    //start debitting the account
    //    for (int i = 0; i < dtTrx.Rows.Count; i++)
    //    {
    //        Transaction t = tsv.setTrnx(dtTrx.Rows[i]); //new Transaction();
    //        asv.authorizeTrnxFromSterling(t);
    //        t.approvedby = User.Identity.Name;
    //        t.approveddate = DateTime.Now;
    //        tsv.updateTrnxById(t);
    //    }

    //    Response.Write("Transaction batch is processing!");
    //    //authorize batch
    //    //bool done = true;

    //    string curBankcode = "x";
    //    int batchCnt = 0;
    //    int trnxCnt = 0;
    //    tb = new TransactionBatch[1];
    //    for (int i = 0; i < dtTrx.Rows.Count; i++)
    //    {
    //        string bankcode = dtTrx.Rows[i]["benebankcode"].ToString();
    //        string batch = dtTrx.Rows[i]["batchNumber"].ToString();
    //        if (bankcode != curBankcode)
    //        {
    //            TransactionBatch b = new TransactionBatch();
    //            curBankcode = bankcode;
    //            b.bankcode = bankcode;
    //            b.code = batch;
    //            batchCnt++;
    //            Array.Resize(ref tb, batchCnt);
    //            tb[batchCnt - 1] = b;
    //            trnxCnt = 0;
    //            tx = new Transaction[1];
    //        }
    //        Transaction t = tsv.setTrnx(dtTrx.Rows[i]); //new Transaction();            
    //        trnxCnt++;
    //        Array.Resize(ref tx, trnxCnt);
    //        tx[trnxCnt - 1] = t;
    //        tb[batchCnt - 1].transactions = tx;
    //    }

    //    ts.batches = tb;
    //    foreach (TransactionBatch b in ts.batches)
    //    {
    //        //let's send the batches to nibss
    //        TR_BulkFundTransferDC bft = new TR_BulkFundTransferDC();
    //        bft.BatchNumber = b.code;
    //        bft.ChannelCode = Convert.ToInt16(ChannelCode.Bank_Teller).ToString();
    //        bft.DestinationBankCode = b.bankcode;
    //        bft.NumberOfRecords = b.transactions.Length.ToString();
    //        bft.createRequest();

    //        foreach (Transaction t in b.transactions)
    //        {

    //            bft.addRecord(t.sessionid, t.outCust.accountnumber, t.outCust.cusname, t.inCust.cus_sho_name,t.Remark,t.paymentRef,g.TRUmoneyToISOmoney(t.amount));
    //        }
    //        //send the request
    //        if (bft.makeRequest())
    //        {
    //            if (bft.ResponseCode == "09")
    //            {
    //                //update status of transactions
    //                foreach (Transaction t in b.transactions)
    //                {
    //                    tsv.updateTrnxStatus(t.sessionid, 3);
    //                }
    //            }
    //            else
    //            {
    //                //update status of transactions
    //                foreach (Transaction t in b.transactions)
    //                {
    //                    tsv.updateTrnxStatus(t.sessionid, 32);
    //                }
    //            }
    //        }
    //    }

    //    //mail cso!
    //}


}
