using System;
using System.Data;


public class Transaction
{
    //newly added paragram
    public string NameEnquiryRef;
    public string BeneficiaryBankVerificationNumber;
    public string OriginatorAccountNumber;
    public string OriginatorBankVerificationNumber;
    public string OriginatorKYCLevel;
    public string TransactionLocation;
    public string BeneficiaryKYCLevel;


    public int Refid;

    public Account inCust = new Account();
    public Beneficiary outCust = new Beneficiary();
    public string ReferenceID;
    public string VAT_bra_code = "";
    public string VAT_cus_num = "";
    public string VAT_cur_code = "";
    public string VAT_led_code = "";
    public string VAT_sub_acct_code = "";
    public Int32 Appid;
    public string sessionid;
    public string sessionidNE;
    public string transactionCode;
    public int transactionNature;
    public int channelCode;
    public string BatchNumber = "0";
    public string paymentRef = "0";
    public string mandateRefNum = "0";
    public string billerName="";
    public string billerId="0";

    public string nuban;
    public decimal amount;
    public string amountWords;
    public decimal feecharge;
    public string Remark;
    public decimal vat;
    public string narration;
    public string senderAcctname;



    public string formnum;
    public string docfilename = "nil";
    public string mime;

    public string tellerID;
    public string origin_branch;
    public int status;
    public string inputedby;
    public DateTime inputdate;
    public string approvedby ="";
    public DateTime approveddate;
    public int approvevalue;
    public string csoemail;
    public string bmemail;
    public string hopemail;
    public string Tssacctnum;
    public string Addedby;
    public string ResponseCode;
    public string ResponseDesc;
    public string RecipientName;

    public string bra_code;
    public string cusnum;
    public string curcode;
    public string ledcode;
    public string subacctcode;
    public string nameresponse;
    public string destinationcode;
    public string originatorname;
    public string AccountName;
    public string AccountNumber;
    public string expcode;

    public decimal amt1;
    public decimal amt2;
    public decimal amt3;
}

    