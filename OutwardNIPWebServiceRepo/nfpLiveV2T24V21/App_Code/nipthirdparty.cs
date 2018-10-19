using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for nipthirdparty
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class nipthirdparty : System.Web.Services.WebService {

    [WebMethod]
    public string ThirdPartyInterbank(string sessionid, string bracodeval, string cusnumval, string curcodeval,
        string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode,
        string ChannelCode, string AccountName, string AccountNumber, string paymentRef,
        string BeneficiaryBankVerificationNumber, string BeneficiaryKYCLevel, string OriginatorAccountNumber,
        string OriginatorBankVerificationNumber, string OriginatorKYCLevel)
    {
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

        string NewSessionid = g.newSessionGlobalSterlingMoney("223", t.channelCode);
        t.sessionid = NewSessionid; //sessionid for Funds Transfer

        TR_3rdPartySingleFundTransferDC sft = new TR_3rdPartySingleFundTransferDC();
        //sft.SessionID = sessionid;

        //sft.DestinationBankCode = DestinationBankCode;
        //sft.ChannelCode = ChannelCode;
        //sft.AccountName = AccountName;
        //sft.AccountNumber = AccountNumber;
        //sft.OriginatorName = orignatorName;

        //sft.OriginatorName = sft.OriginatorName.Replace("&amp;", "&");
        //sft.OriginatorName = sft.OriginatorName.Replace("&apos;", "'");
        //sft.OriginatorName = sft.OriginatorName.Replace("&quot;", "\"");

        //sft.OriginatorName = sft.OriginatorName.Replace("&", "&amp;");
        //sft.OriginatorName = sft.OriginatorName.Replace("'", "&apos;");
        //sft.OriginatorName = sft.OriginatorName.Replace("\"", "&quot;");

        //sft.Narration = "Transfer from " + orignatorName + " to " + t.AccountName;
        //sft.PaymentReference = paymentRef;
        //t.paymentRef = paymentRef;
        //sft.Amount = amt;
        sft.SessionID = NewSessionid;
        sft.DestinationInstitutionCode = DestinationBankCode;
        sft.ChannelCode = ChannelCode;
        sft.BeneficiaryAccountName = t.AccountName;
        sft.BeneficiaryAccountNumber = AccountNumber;
        sft.OriginatorAccountName = orignatorName;
        sft.BeneficiaryBankVerificationNumber = BeneficiaryBankVerificationNumber;
        sft.BeneficiaryKYCLevel = BeneficiaryKYCLevel;
        sft.OriginatorAccountNumber = OriginatorAccountNumber;
        sft.OriginatorBankVerificationNumber = OriginatorBankVerificationNumber;
        sft.OriginatorKYCLevel = OriginatorKYCLevel;
        sft.Narration = "Transfer from " + orignatorName + " to " + t.AccountName;

        sft.NameEnquiryRef = sessionidNE;
        sft.PaymentReference = paymentRef;
        t.paymentRef = paymentRef;
        sft.Amount = amt;
        sft.createRequest();
        TransactionService trs = new TransactionService();
        //send Funds Transfer Request to NIBSS
        sft.sendRequest2();

        //update the table with the response and sessioid for DC
        t.ResponseCode = sft.ResponseCode;
        t.Refid = Int32.Parse(logval);
        t.sessionid = sft.SessionID;
        //int cn = trs.UpdateMobileTrans(t);

        return sft.ResponseCode + ":" + NewSessionid;
    }
    
}
