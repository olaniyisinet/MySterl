using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Xml;

/// <summary>
/// Summary description for TR_MandateAdvice
/// </summary>
public class TR_MandateAdvice
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();

    public string SessionID;
    public string DestinationInstitutionCode;
    public string ChannelCode;
    public string MandateReferenceNumber;
    public string Amount;
    public string ResponseCode;
    public string DebitAccountName;
    public string DebitAccountNumber;
    public string DebitBankVerificationNumber;

    public string DebitKYCLevel;
    public string BeneficiaryAccountName;
    public string BeneficiaryAccountNumber;
    public string BeneficiaryBankVerificationNumber;
    public string BeneficiaryKYCLevel;
    public string xml;

    public bool readRequest()
    {
        try
        {
            SSMIn ssm = new SSMIn();
            xml = ssm.dekrypt(xml);
            //new ErrorLog("Mandate Request Received " + xml);
            Mylogger.Info("Mandate Request Received " + xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.MandateReferenceNumber = xmlDoc.GetElementsByTagName("MandateReferenceNumber").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            this.DebitAccountName = xmlDoc.GetElementsByTagName("DebitAccountName").Item(0).InnerText;
            this.DebitAccountNumber = xmlDoc.GetElementsByTagName("DebitAccountNumber").Item(0).InnerText;
            this.DebitBankVerificationNumber = xmlDoc.GetElementsByTagName("DebitBankVerificationNumber").Item(0).InnerText;
            this.DebitKYCLevel = xmlDoc.GetElementsByTagName("DebitKYCLevel").Item(0).InnerText;
            this.BeneficiaryAccountName = xmlDoc.GetElementsByTagName("BeneficiaryAccountName").Item(0).InnerText;
            this.BeneficiaryAccountNumber = xmlDoc.GetElementsByTagName("BeneficiaryAccountNumber").Item(0).InnerText;
            this.BeneficiaryBankVerificationNumber = xmlDoc.GetElementsByTagName("BeneficiaryBankVerificationNumber").Item(0).InnerText;
            this.BeneficiaryKYCLevel = xmlDoc.GetElementsByTagName("BeneficiaryKYCLevel").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
    }

    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<MandateAdviceResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<MandateReferenceNumber>" + MandateReferenceNumber + "</MandateReferenceNumber>");
        rsp.Append("<Amount>" + Amount + "</Amount>");
        rsp.Append("<DebitAccountName>" + DebitAccountName + "</DebitAccountName>");
        rsp.Append("<DebitAccountNumber>" + DebitAccountNumber + "</DebitAccountNumber>");
        rsp.Append("<DebitBankVerificationNumber>" + DebitBankVerificationNumber + "</DebitBankVerificationNumber>");
        rsp.Append("<DebitKYCLevel>" + DebitKYCLevel + "</DebitKYCLevel>");
        rsp.Append("<BeneficiaryAccountName>" + BeneficiaryAccountName + "</BeneficiaryAccountName>");
        rsp.Append("<BeneficiaryAccountNumber>" + BeneficiaryAccountNumber + "</BeneficiaryAccountNumber>");
        rsp.Append("<BeneficiaryBankVerificationNumber>" + BeneficiaryBankVerificationNumber + "</BeneficiaryBankVerificationNumber>");
        rsp.Append("<BeneficiaryKYCLevel>" + BeneficiaryKYCLevel + "</BeneficiaryKYCLevel>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</MandateAdviceResponse>");

        SSMIn ssm = new SSMIn();
        new ErrorLog("DD Response " + rsp.ToString());
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }
}
