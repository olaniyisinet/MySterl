using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for sbp_outward_trans
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class sbp_outward_trans : System.Web.Services.WebService {

    public sbp_outward_trans () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }
    [WebMethod]
    public string NameEnquiry(string SessionID, string DestinationBankCode, string ChannelCode, string AccountNumber)
    {
        GetActualBankCode abc = new GetActualBankCode();
        DestinationBankCode = abc.getNewBankCode(DestinationBankCode);
        string msg = "";
        string responsecodeVal = "";
        string AcctNameval = "";
        TR_SingleNameEnquiry sne = new TR_SingleNameEnquiry();
        sne.SessionID = SessionID;
        sne.DestinationInstitutionCode = DestinationBankCode;
        sne.ChannelCode = ChannelCode;
        sne.AccountNumber = AccountNumber;
        sne.createRequest();
        //sne.SessionID = SessionID;
        //sne.DestinationBankCode = DestinationBankCode;
        //sne.ChannelCode = ChannelCode;
        //sne.AccountNumber = AccountNumber;
        //sne.createRequest();

        if (!sne.sendRequest()) //unsuccessful request
        {
            responsecodeVal = sne.ResponseCode;
            AcctNameval = "";
            msg = responsecodeVal + ":" + AcctNameval;
            new ErrorLog(msg);
        }
        else
        {
            responsecodeVal = sne.ResponseCode;
            AcctNameval = sne.AccountName;
            msg = responsecodeVal + ":" + AcctNameval;
            new ErrorLog(msg);
        }
        return msg;
    }

    [WebMethod]
    public string FT_SendtoNIBSS(string sessionid, string bracodeval, string cusnumval, string curcodeval,
        string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode,
        string ChannelCode, string AccountName, string AccountNumber, string paymentRef)
    {


        GetActualBankCode abc = new GetActualBankCode();
        DestinationBankCode = abc.getNewBankCode(DestinationBankCode);
 
        //check if the name enquiry session exists already
        Connect cnSNE = new Connect("spd_checkExistSessionNE");
        cnSNE.addparam("@sessionne", sessionid);
        cnSNE.query();
        if (cnSNE.returnValue > 0)
        {
            return "26"; //
        }


        string msg;
        string ledger = "";
        string logval = "0";
        Gadget g = new Gadget();
        AccountService acs = new AccountService();
        Transaction t = new Transaction();
        TransactionService tsv = new TransactionService();
        string sessionidNE = "";
        sessionidNE = sessionid;//get session id from the already done NE
        t.bra_code = bracodeval;
        t.cusnum = cusnumval;
        t.curcode = curcodeval;
        t.ledcode = ledcodeval;
        t.subacctcode = subacctval;
        t.paymentRef = paymentRef;
        t.amount = decimal.Parse(amt);
        t.destinationcode = DestinationBankCode;
        t.transactionCode = g.newTrnxRef(bracodeval);
        t.originatorname = orignatorName;
        t.feecharge = 0;
        t.AccountName = AccountName;
        t.AccountNumber = AccountNumber;
        t.channelCode = Convert.ToInt16(ChannelCode);
        t.sessionid = sessionidNE;
        string acct = bracodeval + cusnumval + curcodeval + ledcodeval + subacctval;

        //Log transaction Request First
        logval = tsv.InsertNameBankTellerTrans(t);
        if (t.curcode != "566")
        {
            //new ErrorLog("57:Transaction not permitted " + acct + " Refid " + logval);
            Mylogger1.Info("57:Transaction not permitted " + acct + " Refid " + logval);
            msg = "57";
            return msg;
        }
        t.Refid = Int32.Parse(logval);
        //update the Response code
        
        tsv.UpdateBankTellerTransactions(t);


        t.tellerID = "9948";
        //compute the amount to pay
        //t.feecharge = acs.calculateFee(t.amount);

        t.transactionCode = g.newTrnxRef(bracodeval);

        t.origin_branch = t.bra_code;
        t.inCust.cus_sho_name = orignatorName;
        t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + t.transactionCode;

        t.inCust.bra_code = t.bra_code;
        t.inCust.cus_num = t.cusnum;
        t.inCust.cur_code = t.curcode;
        t.inCust.led_code = t.ledcode;
        t.inCust.sub_acct_code = t.subacctcode;

        //string NewSessionid = g.newSessionId(DestinationBankCode);
        string NewSessionid = g.newSessionGlobal(t.bra_code, t.channelCode); //g.newSessionId(DestinationBankCode, t.bra_code, t.channelCode);
        t.sessionid = NewSessionid; //sessionid for Funds Transfer
        t.feecharge = decimal.Parse(fee);

       

        //if successful

        TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();
        sft.SessionID = NewSessionid;
        sft.DestinationInstitutionCode = DestinationBankCode;
        sft.ChannelCode = ChannelCode;
        sft.BeneficiaryAccountName = t.AccountName;
        sft.BeneficiaryAccountNumber = AccountNumber;
        sft.OriginatorAccountName = orignatorName;

        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&amp;", "&");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&apos;", "'");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&quot;", "\"");

        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&", "&amp;");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("'", "&apos;");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("\"", "&quot;");

        //sft.SessionID = sessionid;
        //sft.DestinationBankCode = DestinationBankCode;
        //sft.ChannelCode = ChannelCode;
        //sft.AccountName = t.AccountName;
        //sft.AccountNumber = AccountNumber;
        //sft.OriginatorName = orignatorName;

        //sft.OriginatorName = sft.OriginatorName.Replace("&amp;", "&");
        //sft.OriginatorName = sft.OriginatorName.Replace("&apos;", "'");
        //sft.OriginatorName = sft.OriginatorName.Replace("&quot;", "\"");

        //sft.OriginatorName = sft.OriginatorName.Replace("&", "&amp;");
        //sft.OriginatorName = sft.OriginatorName.Replace("'", "&apos;");
        //sft.OriginatorName = sft.OriginatorName.Replace("\"", "&quot;");

        sft.Narration = "Transfer from " + orignatorName + " to " + t.AccountName;
        sft.PaymentReference = paymentRef;
        t.paymentRef = paymentRef;
        sft.Amount = amt;
        sft.createRequest();

        TransactionService trs = new TransactionService();
        //send Funds Transfer Request to NIBSS
        sft.sendRequest();

        //new ErrorLog("Response for IMAL outward transaction " + sft.ResponseCode + " for sessionid " + sessionid);
        Mylogger1.Info("Response for IMAL outward transaction " + sft.ResponseCode + " for sessionid " + sessionid);
                //}
        //update the table with the response and sessioid for DC
        t.ResponseCode = sft.ResponseCode;
        t.Refid = Int32.Parse(logval);
        t.sessionid = sft.SessionID;
        int cn = trs.UpdateMobileTrans(t);

        return sft.ResponseCode;// +":" + logval.ToString();
    }

    [WebMethod]
    public string FT_SendtoNIBSSIMAL(string sessionid, string bracodeval, string cusnumval, string curcodeval,
    string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode,
    string ChannelCode, string AccountName, string AccountNumber, string paymentRef,
    string sessionidFT, string BeneBVN, string OrigBVN)
    {
        string logval = "0";
        Gadget g = new Gadget();
        AccountService acs = new AccountService();
        Transaction t = new Transaction();
        TransactionService tsv = new TransactionService();
        string sessionidNE = "";
        sessionidNE = sessionid;//get session id from the already done NE
        t.bra_code = bracodeval;
        t.cusnum = cusnumval;
        t.curcode = curcodeval;
        t.ledcode = ledcodeval;
        t.subacctcode = subacctval;
        t.paymentRef = paymentRef;
        t.amount = decimal.Parse(amt);
        t.destinationcode = DestinationBankCode;
        t.transactionCode = g.newTrnxRef(bracodeval);
        t.originatorname = orignatorName;
        //t.feecharge = 0;
        t.feecharge = decimal.Parse(fee);
        t.AccountName = AccountName;
        t.AccountNumber = AccountNumber;
        t.channelCode = Convert.ToInt16(ChannelCode);
        t.sessionid = sessionidNE;
        //t.nuban = OriginatorAccountNumber;
        //Log transaction Request First
        logval = tsv.InsertNameBankTellerTrans(t);
        //logval = tsv.InsertOutgoingNibss(t);


        t.Refid = Int32.Parse(logval);
        //update the Response code

        tsv.UpdateBankTellerTransactions(t);

        TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();
        sft.SessionID = sessionidFT;
        sft.NameEnquiryRef = sessionid;
        sft.DestinationInstitutionCode = DestinationBankCode;
        sft.ChannelCode = ChannelCode;
        sft.BeneficiaryAccountName = t.AccountName;
        sft.BeneficiaryAccountNumber = AccountNumber;
        sft.OriginatorAccountName = orignatorName;

        sft.BeneficiaryBankVerificationNumber = BeneBVN;
        sft.BeneficiaryKYCLevel = "1";
        sft.OriginatorBankVerificationNumber = OrigBVN;
        sft.OriginatorKYCLevel = "1";
        sft.TransactionLocation = "1";

        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&amp;", "&");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&apos;", "'");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&quot;", "\"");

        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("&", "&amp;");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("'", "&apos;");
        sft.OriginatorAccountName = sft.OriginatorAccountName.Replace("\"", "&quot;");


        sft.Narration = "Transfer from " + orignatorName + " to " + t.AccountName;
        sft.PaymentReference = paymentRef;
        t.paymentRef = paymentRef;
        sft.Amount = amt;
        sft.createRequest();

        TransactionService trs = new TransactionService();
        //send Funds Transfer Request to NIBSS
        sft.sendRequest();


        //update the table with the response and sessioid for DC
        t.ResponseCode = sft.ResponseCode;
        //t.Refid = Int32.Parse(logval);
        t.sessionid = sft.SessionID;
        int cn = trs.UpdateMobileTrans(t);
        return sft.ResponseCode;// +":" + logval.ToString();
    }

    public string NIPReversal(string logval)
    {
        //insert into the mobile reversal table
        Connect cn = new Connect("spd_mobileTrnxReverse");
        cn.addparam("@refid", logval);
        cn.addparam("@doneby", "AUTOSYSTEM");
        cn.addparam("@action", "reverse");
        cn.query();
        return "";
    }
    
}

