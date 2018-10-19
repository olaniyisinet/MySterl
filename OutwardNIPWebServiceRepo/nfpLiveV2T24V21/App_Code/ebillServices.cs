using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for ebillServices
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class ebillServices : System.Web.Services.WebService {

    [WebMethod]
    public string NameEnquiry(string SessionID, string DestinationBankCode, string ChannelCode, string AccountNumber)
    {
        Gadget g = new Gadget();
        AccountService acs = new AccountService();
        Transaction t = new Transaction();
        TransactionService tsv = new TransactionService();
        //Do Name Enquiry to NIBSS
        TR_SingleNameEnquiry sne = new TR_SingleNameEnquiry();
        sne.SessionID = SessionID;// 
        sne.DestinationInstitutionCode = DestinationBankCode;
        sne.ChannelCode = ChannelCode;
        sne.AccountNumber = AccountNumber;
        sne.createRequest();
        //send name enquiry request to NIBSS
        sne.sendRequesteBILLSne();
        return sne.ResponseCode;
    }

    [WebMethod]
    public string SendtoNIBSS(string sessionid, string bracodeval, string cusnumval, string curcodeval,
        string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode,
        string ChannelCode, string AccountName, string AccountNumber, string paymentRef, string cusshowname, string ebillsNarration,
        string BeneficiaryBankVerificationNumber, string BeneficiaryKYCLevel, string OriginatorAccountNumber, string OriginatorBankVerificationNumber, 
        string OriginatorKYCLevel, string TransactionLocation, string NameEnquiryRef)
    {
        //check if the name enquiry session exists already
        


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
        //t.feecharge = decimal.Parse(fee);
        t.AccountName = AccountName;
        t.AccountNumber = AccountNumber;
        t.channelCode = Convert.ToInt16(ChannelCode);//Channel code for Internet Banking
        t.sessionid = sessionidNE;
        string acct = bracodeval + cusnumval + curcodeval + ledcodeval + subacctval;

        if (t.ledcode == "6602" || t.ledcode == "1701" || t.ledcode == "6020" || t.ledcode == "1000" || t.ledcode == "6017" || t.ledcode == "6018" || t.ledcode == "6019")
        {
            new ErrorLog("57:Transaction not permitted " + acct + " Refid " + logval);
            msg = "57:Transaction not permitted";
            return msg;
        }
        

        t.transactionCode = g.newTrnxRef(bracodeval);

        t.origin_branch = t.bra_code;
        t.inCust.cus_sho_name = orignatorName;
        t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + t.transactionCode;

        t.inCust.bra_code = t.bra_code;
        t.inCust.cus_num = t.cusnum;
        t.inCust.cur_code = t.curcode;
        t.inCust.led_code = t.ledcode;
        t.inCust.sub_acct_code = t.subacctcode;

        string NewSessionid = sessionid;// g.newSessionId(DestinationBankCode);
        t.sessionid = NewSessionid; //sessionid for Funds Transfer


        

        TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();
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
   

        sft.Narration = ebillsNarration;   //"Transfer from " + cusshowname + " to " + t.AccountName;

        if (paymentRef.Length > 100)
        {
            paymentRef = paymentRef.Substring(0, 100);
        }

        sft.PaymentReference = paymentRef;
        t.paymentRef = paymentRef;
        sft.Amount = amt;
        sft.createRequest();

        TransactionService trs = new TransactionService();
        //send Funds Transfer Request to NIBSS
        sft.sendRequestEBILLS();

        string rspstmt = "";
        switch (sft.ResponseCode)
        {
            case "00":
                //success
                rspstmt = sft.ResponseCode;// "3";  //acs.ResponseMsg;
                break;
            case "03": //txt = "Invalid sender"; break;
            case "05": //txt = "Do not honor"; break;
            case "06": //txt = "Dormant account"; break;
            case "07": //txt = "Invalid account"; break;
            case "08": //txt = "Account name mismatch"; break;
            case "09": //txt = "Request processing in progress"; break;
            case "12": //txt = "Invalid transaction"; break;
            case "13": //txt = "Invalid amount"; break;
            case "14": //txt = "Invalid Batch Number"; break;
            case "15": //txt = "Invalid Session or Record ID"; break;
            case "16": //txt = "Unknown Bank Code"; break;
            case "17": //txt = "Invalid Channel"; break;
            case "18": //txt = "Wrong Method Call"; break;
            case "21": //txt = "No action taken"; break;
            case "25": //txt = "Unable to locate record"; break;
            case "26": //txt = "Duplicate record"; break;
            case "30": //txt = "Wrong destination account format"; break;
            case "34": //txt = "Suspected fraud"; break;
            case "35": //txt = "Contact sending bank"; break;
            case "51": //txt = "No sufficient funds"; break;
            case "57": //txt = "Transaction not permitted to sender"; break;
            case "58": //txt = "Transaction not permitted on channel"; break;
            case "61": //txt = "Transfer Limit Exceeded"; break;
            case "63": //txt = "Security violation"; break;
            case "65": //txt = "Exceeds withdrawal frequency"; break;
            case "68": //txt = "Response received too late"; break;
            case "91": //txt = "Beneficiary Bank not available"; break;
            case "92": //txt = "Routing Error"; break;
            case "94": //txt = "Duplicate Transaction"; break;
            case "96": //txt = "Corresponding Bank is currently offline."; break;
            case "97": //txt = "Timeout waiting for response from destination."; break;
                rspstmt = sft.ResponseCode;
                string rsp = NIPReversal(logval);
                new ErrorLog("IBS Reversal was done==>" + rsp);
                break;
            case "1x":
            default:
                rspstmt = sft.ResponseCode;// "5";
                break;

        }
        //update the table with the response and sessioid for DC
        t.ResponseCode = sft.ResponseCode;
        t.Refid = Int32.Parse(logval);
        t.sessionid = sft.SessionID;
        int cn = trs.UpdateMobileTrans(t);

        return rspstmt + ":" + logval.ToString();
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

    [WebMethod]
    public string doSSMEnc(string txt)
    {
        //SSM ssm = new SSM();
        SSM1 ssm = new SSM1();
        string str = ssm.enkrypt(txt);
        return str;
    }

    [WebMethod]
    public string doSSMDec(string txt)
    {
        //SSM ssm = new SSM();
        SSM1 ssm = new SSM1();
        string str = ssm.dekrypt(txt);
        return str;
    }
}

