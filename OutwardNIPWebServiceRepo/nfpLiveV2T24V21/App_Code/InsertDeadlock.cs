using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for InsertDeadlock
/// </summary>
public class InsertDeadlock
{
    Gadget g = new Gadget();
    public void insertRec(string sessionid, string paymentRef, string mandateRefNum, string sendername, decimal amt, decimal feecharge, string bra_code,
      string cus_num, string cur_code, string led_code, string sub_acct_code, string accname, string Remark, string Responsecode, string ResponseMsg,
        string NameEnquiryRef, string BeneficiaryBankVerificationNumber, string OriginatorAccountNumber, string OriginatorBankVerificationNumber,
       int OriginatorKYCLevel, string TransactionLocation)
    {
        string sql = "insert into tbl_wstrans_deadlck (sessionid,bra_code,cus_num,cur_code,led_code,sub_acct_code,amt,paymentRef,mandateRefNum, " +
            " remark,sendername,Responsecode,accname,feecharge,ResponseMsg,NameEnquiryRef, " +
            " BeneficiaryBankVerificationNumber,OriginatorAccountNumber,OriginatorBankVerificationNumber,OriginatorKYCLevel,TransactionLocation) " +

            " values (@sessionid,@bra_code,@cus_num,@cur_code,@led_code,@sub_acct_code,@amt,@paymentRef,@mandateRefNum, " +
            " @remark,@sendername,@Responsecode,@accname,@feecharge,@ResponseMsg,@NameEnquiryRef, " +
            " @BeneficiaryBankVerificationNumber,@OriginatorAccountNumber,@OriginatorBankVerificationNumber,@OriginatorKYCLevel,@TransactionLocation) ";

        Connect c11 = new Connect(sql, true);
        c11.addparam("@sessionid", sessionid);
        c11.addparam("@bra_code", bra_code);
        c11.addparam("@cus_num", cus_num);
        c11.addparam("@cur_code", cur_code);
        c11.addparam("@led_code", led_code);
        c11.addparam("@sub_acct_code", sub_acct_code);
        c11.addparam("@amt", amt);
        c11.addparam("@paymentRef", paymentRef);
        c11.addparam("@mandateRefNum", mandateRefNum);
        c11.addparam("@remark", Remark);
        c11.addparam("@sendername", sendername);
        c11.addparam("@Responsecode", Responsecode);
        c11.addparam("@accname", accname);
        c11.addparam("@feecharge", 0);
        c11.addparam("@ResponseMsg", g.responseCodes(Responsecode));

        c11.addparam("@NameEnquiryRef", NameEnquiryRef);
        c11.addparam("@BeneficiaryBankVerificationNumber", BeneficiaryBankVerificationNumber);
        c11.addparam("@OriginatorAccountNumber", OriginatorAccountNumber);
        c11.addparam("@OriginatorBankVerificationNumber", OriginatorBankVerificationNumber);


        c11.addparam("@OriginatorKYCLevel", OriginatorKYCLevel);
        c11.addparam("@TransactionLocation", TransactionLocation);
        c11.query();
    }
}
