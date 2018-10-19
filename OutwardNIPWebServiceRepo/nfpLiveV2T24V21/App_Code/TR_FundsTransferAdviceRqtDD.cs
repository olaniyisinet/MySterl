using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Text;

/// <summary>
/// Summary description for TR_FundsTransferAdviceRqtDD
/// </summary>
public class TR_FundsTransferAdviceRqtDD
{
    Gadget g = new Gadget();
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string SessionID;
    public string NameEnquiryRef;
    public string DestinationInstitutionCode;
    public string ChannelCode;

    public string DebitAccountName;
    public string DebitAccountNumber;
    public string DebitBankVerificationNumber;
    public string DebitKYCLevel;
    public string BeneficiaryAccountName;
    public string BeneficiaryAccountNumber;
    public string BeneficiaryBankVerificationNumber;
    public string BeneficiaryKYCLevel;
    public string TransactionLocation;
    public string Narration;
    public string PaymentReference;
    public string MandateReferenceNumber;
    public string TransactionFee;
    public string Amount;
    public string ResponseCode;
    public string xml;

    public void createRequest()
    {

        this.DebitAccountName = this.DebitAccountName.Replace("&amp;", "&");
        this.DebitAccountName = this.DebitAccountName.Replace("&apos;", "'");
        this.DebitAccountName = this.DebitAccountName.Replace("&quot;", "\"");

        this.DebitAccountName = this.DebitAccountName.Replace("&", "&amp;");
        this.DebitAccountName = this.DebitAccountName.Replace("'", "&apos;");
        this.DebitAccountName = this.DebitAccountName.Replace("\"", "&quot;");


        this.Narration = this.Narration.Replace("&amp;", "&");
        this.Narration = this.Narration.Replace("&apos;", "'");
        this.Narration = this.Narration.Replace("&quot;", "\"");

        this.Narration = this.Narration.Replace("&", "&amp;");
        this.Narration = this.Narration.Replace("'", "&apos;");
        this.Narration = this.Narration.Replace("\"", "&quot;");

        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTAdviceDebitRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<DestinationBankCode>" + DestinationInstitutionCode + "</DestinationBankCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<AccountName>" + DebitAccountName + "</AccountName>");
        rqt.Append("<AccountNumber>" + DebitAccountNumber + "</AccountNumber>");
        //rqt.Append("<BillerName>" + BillerName + "</BillerName>");
        //rqt.Append("<BillerID>" + BillerID + "</BillerID>");
        rqt.Append("<Narration>" + Narration + "</Narration>");
        rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rqt.Append("<MandateReferenceNumber>" + MandateReferenceNumber + "</MandateReferenceNumber>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</FTAdviceDebitRequest>");
    }

    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<FTAdviceDebitResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<NameEnquiryRef>" + NameEnquiryRef + "</NameEnquiryRef>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<DebitAccountName>" + DebitAccountName + "</DebitAccountName>");
        rsp.Append("<DebitAccountNumber>" + DebitAccountNumber + "</DebitAccountNumber>");
        rsp.Append("<DebitBankVerificationNumber>" + DebitBankVerificationNumber + "</DebitBankVerificationNumber>");
        rsp.Append("<DebitKYCLevel>" + DebitKYCLevel + "</DebitKYCLevel>");
        rsp.Append("<BeneficiaryAccountName>" + BeneficiaryAccountName + "</BeneficiaryAccountName>");
        rsp.Append("<BeneficiaryAccountNumber>" + BeneficiaryAccountNumber + "</BeneficiaryAccountNumber>");
        rsp.Append("<BeneficiaryBankVerificationNumber>" + BeneficiaryBankVerificationNumber + "</BeneficiaryBankVerificationNumber>");
        rsp.Append("<BeneficiaryKYCLevel>" + BeneficiaryKYCLevel + "</BeneficiaryKYCLevel>");
        rsp.Append("<TransactionLocation>" + TransactionLocation + "</TransactionLocation>");
        rsp.Append("<Narration>" + Narration + "</Narration>");
        rsp.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rsp.Append("<MandateReferenceNumber>" + MandateReferenceNumber + "</MandateReferenceNumber>");
        rsp.Append("<TransactionFee>" + TransactionFee + "</TransactionFee>");
        rsp.Append("<Amount>" + Amount + "</Amount>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</FTAdviceDebitResponse>");

        SSMIn ssm = new SSMIn();
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }

    public bool readRequest()
    {
        try
        {
            SSMIn ssm = new SSMIn();
            xml = ssm.dekrypt(xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.NameEnquiryRef = xmlDoc.GetElementsByTagName("NameEnquiryRef").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.DebitAccountName = xmlDoc.GetElementsByTagName("DebitAccountName").Item(0).InnerText;
            this.DebitAccountNumber = xmlDoc.GetElementsByTagName("DebitAccountNumber").Item(0).InnerText;
            this.DebitBankVerificationNumber = xmlDoc.GetElementsByTagName("DebitBankVerificationNumber").Item(0).InnerText;
            this.DebitKYCLevel = xmlDoc.GetElementsByTagName("DebitKYCLevel").Item(0).InnerText;

            this.BeneficiaryAccountName = xmlDoc.GetElementsByTagName("BeneficiaryAccountName").Item(0).InnerText;
            this.BeneficiaryAccountNumber = xmlDoc.GetElementsByTagName("BeneficiaryAccountNumber").Item(0).InnerText;
            this.BeneficiaryBankVerificationNumber = xmlDoc.GetElementsByTagName("BeneficiaryBankVerificationNumber").Item(0).InnerText;
            this.BeneficiaryKYCLevel = xmlDoc.GetElementsByTagName("BeneficiaryKYCLevel").Item(0).InnerText;

            this.TransactionLocation = xmlDoc.GetElementsByTagName("TransactionLocation").Item(0).InnerText;
            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
            this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;

            this.MandateReferenceNumber = xmlDoc.GetElementsByTagName("MandateReferenceNumber").Item(0).InnerText;
            this.TransactionFee = xmlDoc.GetElementsByTagName("TransactionFee").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }
}