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
using System.Net;
using System.Configuration;

/// <summary>
/// Summary description for TR_FundsTransferAdviceRqtDC
/// </summary>
public class TR_FundsTransferAdviceRqtDC
{
    Gadget g = new Gadget();
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string SessionID;
    public string DestinationInstitutionCode;
    public string ChannelCode;
    public string BeneficiaryAccountName;
    public string BeneficiaryAccountNumber;
    public string BeneficiaryBankVerificationNumber;
    public string NameEnquiryRef;
    public string OriginatorAccountName;
    public string OriginatorAccountNumber;
    public string OriginatorBankVerificationNumber;
    public string OriginatorKYCLevel;
    public string BeneficiaryKYCLevel;



    public string TransactionLocation;
    public string Narration;
    public string PaymentReference;
    public string Amount;
    public string ResponseCode;
    public string toNibss;
    //public string SessionID;
    //public string DestinationBankCode;
    //public string ChannelCode;
    //public string AccountName;
    //public string AccountNumber;
    //public string OriginatorName;
    //public string Narration;
    //public string PaymentReference;
    //public string Amount;
    //public string ResponseCode;
    //public string toNibss;

    public string xml;

    public void createRequest()
    {

        this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&amp;", "&");
        this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&apos;", "'");
        this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&quot;", "\"");

        this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&", "&amp;");
        this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("'", "&apos;");
        this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("\"", "&quot;");


        this.Narration = this.Narration.Replace("&amp;", "&");
        this.Narration = this.Narration.Replace("&apos;", "'");
        this.Narration = this.Narration.Replace("&quot;", "\"");

        this.Narration = this.Narration.Replace("&", "&amp;");
        this.Narration = this.Narration.Replace("'", "&apos;");
        this.Narration = this.Narration.Replace("\"", "&quot;");

        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTAdviceCreditRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<BeneficiaryAccountName>" + BeneficiaryAccountName + "</BeneficiaryAccountName>");
        rqt.Append("<BeneficiaryAccountNumber>" + BeneficiaryAccountNumber + "</BeneficiaryAccountNumber>");
        rqt.Append("<BeneficiaryBankVerificationNumber>" + BeneficiaryBankVerificationNumber + "</BeneficiaryBankVerificationNumber>");
        rqt.Append("<OriginatorAccountName>" + OriginatorAccountName + "</OriginatorAccountName>");
        rqt.Append("<OriginatorAccountNumber>" + OriginatorAccountNumber + "</OriginatorAccountNumber>");
        rqt.Append("<OriginatorBankVerificationNumber>" + OriginatorBankVerificationNumber + "</OriginatorBankVerificationNumber>");
        rqt.Append("<OriginatorKYCLevel>" + OriginatorKYCLevel + "</OriginatorKYCLevel>");
        //rqt.Append("<Narration>" + Narration + "</Narration>");
        //rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rqt.Append("<TransactionLocation>" + TransactionLocation + "</TransactionLocation>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</FTAdviceCreditRequest>");
    }
    //public void createRequest()
    //{

    //    this.AccountName = this.AccountName.Replace("&amp;", "&");
    //    this.AccountName = this.AccountName.Replace("&apos;", "'");
    //    this.AccountName = this.AccountName.Replace("&quot;", "\"");

    //    this.AccountName = this.AccountName.Replace("&", "&amp;");
    //    this.AccountName = this.AccountName.Replace("'", "&apos;");
    //    this.AccountName = this.AccountName.Replace("\"", "&quot;");


    //    this.Narration = this.Narration.Replace("&amp;", "&");
    //    this.Narration = this.Narration.Replace("&apos;", "'");
    //    this.Narration = this.Narration.Replace("&quot;", "\"");

    //    this.Narration = this.Narration.Replace("&", "&amp;");
    //    this.Narration = this.Narration.Replace("'", "&apos;");
    //    this.Narration = this.Narration.Replace("\"", "&quot;");

    //    rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    //    rqt.Append("<FTAdviceCreditRequest>");
    //    rqt.Append("<SessionID>" + SessionID + "</SessionID>");
    //    rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
    //    rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
    //    rqt.Append("<AccountName>" + AccountName + "</AccountName>");
    //    rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
    //    rqt.Append("<OriginatorName>" + OriginatorName + "</OriginatorName>");
    //    rqt.Append("<Narration>" + Narration + "</Narration>");
    //    rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
    //    rqt.Append("<Amount>" + Amount + "</Amount>");
    //    rqt.Append("</FTAdviceCreditRequest>");
    //}

    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<FTAdviceCreditResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<NameEnquiryRef>" + NameEnquiryRef + "</NameEnquiryRef>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<BeneficiaryAccountName>" + BeneficiaryAccountName + "</BeneficiaryAccountName>");
        rsp.Append("<BeneficiaryAccountNumber>" + BeneficiaryAccountNumber + "</BeneficiaryAccountNumber>");
        rsp.Append("<BeneficiaryBankVerificationNumber>" + BeneficiaryBankVerificationNumber + "</BeneficiaryBankVerificationNumber>");
        rsp.Append("<BeneficiaryKYCLevel>" + BeneficiaryKYCLevel + "</BeneficiaryKYCLevel>");
        rsp.Append("<OriginatorAccountName>" + OriginatorAccountName + "</OriginatorAccountName>");
        rsp.Append("<OriginatorAccountNumber>" + OriginatorAccountNumber + "</OriginatorAccountNumber>");
        rsp.Append("<OriginatorBankVerificationNumber>" + OriginatorBankVerificationNumber + "</OriginatorBankVerificationNumber>");
        rsp.Append("<OriginatorKYCLevel>" + OriginatorKYCLevel + "</OriginatorKYCLevel>");
        rsp.Append("<TransactionLocation>" + TransactionLocation + "</TransactionLocation>");
        rsp.Append("<Narration>" + Narration + "</Narration>");
        rsp.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rsp.Append("<Amount>" + Amount + "</Amount>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</FTAdviceCreditResponse>");

        SSMIn ssm = new SSMIn();
        string str = ssm.enkrypt(rsp.ToString());
        //string str = rsp.ToString();
        return str;
    }

    //public string createResponse()
    //{
    //    rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    //    rsp.Append("<FTAdviceCreditResponse>");
    //    rsp.Append("<SessionID>" + SessionID + "</SessionID>");
    //    rsp.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
    //    rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
    //    rsp.Append("<AccountName>" + AccountName + "</AccountName>");
    //    rsp.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
    //    rsp.Append("<OriginatorName>" + OriginatorName + "</OriginatorName>");
    //    rsp.Append("<Narration>" + Narration + "</Narration>");
    //    rsp.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
    //    rsp.Append("<Amount>" + Amount + "</Amount>");
    //    rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
    //    rsp.Append("</FTAdviceCreditResponse>");

    //    SSM ssm = new SSM();
    //    string str = ssm.enkrypt(rsp.ToString());
    //    return str;
    //}
    public bool sendRequest()
    {
        bool ok = false;
        try
        {
            Nibbslive.NIPInterface nr = new Nibbslive.NIPInterface();

            //set a proxy
            bool useproxy = Convert.ToBoolean(ConfigurationManager.AppSettings["proxyUse"]);
            if (useproxy)
            {
                string url = Convert.ToString(ConfigurationManager.AppSettings["proxyURL"]);
                int port = Convert.ToInt16(ConfigurationManager.AppSettings["proxyPort"]);
                WebProxy p = new WebProxy(url, port);
                nr.Proxy = p;
            }

            SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());

            xml = nr.fundtransfersingleitem_dc(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "1x";
            ok = readResponse();
        }
        return ok;
    }
    public bool readResponse()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            xml = xml.Replace("&amp;", "&");
            xml = xml.Replace("&apos;", "'");

            xml = xml.Replace("&", "&amp;");
            xml = xml.Replace("'", "&apos;");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.BeneficiaryAccountName = xmlDoc.GetElementsByTagName("BeneficiaryAccountName").Item(0).InnerText;
            //clean account name
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&amp;", "&");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&apos;", "'");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&quot;", "\"");

            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&", "&amp;");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("'", "&apos;");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("\"", "&quot;");

            this.BeneficiaryAccountNumber = xmlDoc.GetElementsByTagName("BeneficiaryAccountNumber").Item(0).InnerText;
            this.BeneficiaryBankVerificationNumber = xmlDoc.GetElementsByTagName("BeneficiaryBankVerificationNumber").Item(0).InnerText;
            this.OriginatorAccountName = xmlDoc.GetElementsByTagName("OriginatorAccountName").Item(0).InnerText;

            //clean originator
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&amp;", "&");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&apos;", "'");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&quot;", "\"");

            this.OriginatorAccountName = this.OriginatorAccountName.Replace("& ", "&amp;");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("'", "&apos;");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("\"", "&quot;");

            this.OriginatorAccountNumber = xmlDoc.GetElementsByTagName("OriginatorAccountNumber").Item(0).InnerText;
            this.OriginatorBankVerificationNumber = xmlDoc.GetElementsByTagName("OriginatorBankVerificationNumber").Item(0).InnerText;
            this.OriginatorKYCLevel = xmlDoc.GetElementsByTagName("OriginatorKYCLevel").Item(0).InnerText;
            //this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
            //clean narration


            //this.Narration = this.Narration.Replace("&amp;", "&");
            //this.Narration = this.Narration.Replace("&apos;", "'");
            //this.Narration = this.Narration.Replace("&quot;", "\"");

            //this.Narration = this.Narration.Replace("& ", "&amp;");
            //this.Narration = this.Narration.Replace("'", "&apos;");
            //this.Narration = this.Narration.Replace("\"", "&quot;");

            //this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
            this.TransactionLocation = xmlDoc.GetElementsByTagName("TransactionLocation").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "1x";
            return false;
        }
    }
    //public bool readResponse()
    //{
    //    try
    //    {
    //        SSM ssm = new SSM();
    //        xml = ssm.dekrypt(xml);

    //        xml = xml.Replace("&amp;", "&");
    //        xml = xml.Replace("&apos;", "'");

    //        xml = xml.Replace("&", "&amp;");
    //        xml = xml.Replace("'", "&apos;");

    //        XmlDocument xmlDoc = new XmlDocument();
    //        xmlDoc.LoadXml(xml);
    //        this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
    //        this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
    //        this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
    //        this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
    //        //clean account name
    //        this.AccountName = this.AccountName.Replace("&amp;", "&");
    //        this.AccountName = this.AccountName.Replace("&apos;", "'");
    //        this.AccountName = this.AccountName.Replace("&quot;", "\"");

    //        this.AccountName = this.AccountName.Replace("&", "&amp;");
    //        this.AccountName = this.AccountName.Replace("'", "&apos;");
    //        this.AccountName = this.AccountName.Replace("\"", "&quot;");

    //        this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
    //        this.OriginatorName = xmlDoc.GetElementsByTagName("OriginatorName").Item(0).InnerText;

    //        //clean originator
    //        this.OriginatorName = this.OriginatorName.Replace("&amp;", "&");
    //        this.OriginatorName = this.OriginatorName.Replace("&apos;", "'");
    //        this.OriginatorName = this.OriginatorName.Replace("&quot;", "\"");

    //        this.OriginatorName = this.OriginatorName.Replace("& ", "&amp;");
    //        this.OriginatorName = this.OriginatorName.Replace("'", "&apos;");
    //        this.OriginatorName = this.OriginatorName.Replace("\"", "&quot;");

    //        this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
    //        //clean narration


    //        this.Narration = this.Narration.Replace("&amp;", "&");
    //        this.Narration = this.Narration.Replace("&apos;", "'");
    //        this.Narration = this.Narration.Replace("&quot;", "\"");

    //        this.Narration = this.Narration.Replace("& ", "&amp;");
    //        this.Narration = this.Narration.Replace("'", "&apos;");
    //        this.Narration = this.Narration.Replace("\"", "&quot;");

    //        this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
    //        this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
    //        this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        new ErrorLog(ex);
    //        ResponseCode = "1x";
    //        return false;
    //    }
    //}
    public bool readRequest()
    {
        try
        {
            SSMIn ssm = new SSMIn();
            xml = ssm.dekrypt(xml);
            //new ErrorLog("New advice recieved " + xml);
            Mylogger.Info("New advice recieved " + xml);
            xml = xml.Replace("&amp;", "&");
            xml = xml.Replace("&apos;", "'");

            xml = xml.Replace("&", "&amp;");
            xml = xml.Replace("'", "&apos;");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.NameEnquiryRef = xmlDoc.GetElementsByTagName("NameEnquiryRef").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.BeneficiaryAccountName = xmlDoc.GetElementsByTagName("BeneficiaryAccountName").Item(0).InnerText;

            //clean account name
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&amp;", "&");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&apos;", "'");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&quot;", "\"");

            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("&", "&amp;");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("'", "&apos;");
            this.BeneficiaryAccountName = this.BeneficiaryAccountName.Replace("\"", "&quot;");

            this.BeneficiaryAccountNumber = xmlDoc.GetElementsByTagName("BeneficiaryAccountNumber").Item(0).InnerText;
            this.BeneficiaryBankVerificationNumber = xmlDoc.GetElementsByTagName("BeneficiaryBankVerificationNumber").Item(0).InnerText;
            this.BeneficiaryKYCLevel = xmlDoc.GetElementsByTagName("BeneficiaryKYCLevel").Item(0).InnerText;
            this.OriginatorAccountName = xmlDoc.GetElementsByTagName("OriginatorAccountName").Item(0).InnerText;
            this.OriginatorAccountNumber = xmlDoc.GetElementsByTagName("OriginatorAccountNumber").Item(0).InnerText;
            this.OriginatorBankVerificationNumber = xmlDoc.GetElementsByTagName("OriginatorBankVerificationNumber").Item(0).InnerText;
            this.OriginatorKYCLevel = xmlDoc.GetElementsByTagName("OriginatorKYCLevel").Item(0).InnerText;


            //clean Originator Name
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&amp;", "&");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&apos;", "'");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&quot;", "\"");

            this.OriginatorAccountName = this.OriginatorAccountName.Replace("&", "&amp;");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("'", "&apos;");
            this.OriginatorAccountName = this.OriginatorAccountName.Replace("\"", "&quot;");

            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;

            //clean Originator Name
            this.Narration = this.Narration.Replace("&amp;", "&");
            this.Narration = this.Narration.Replace("&apos;", "'");
            this.Narration = this.Narration.Replace("&quot;", "\"");

            this.Narration = this.Narration.Replace("&", "&amp;");
            this.Narration = this.Narration.Replace("'", "&apos;");
            this.Narration = this.Narration.Replace("\"", "&quot;");

            this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
            this.TransactionLocation = xmlDoc.GetElementsByTagName("TransactionLocation").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
    }
    //public bool readRequest()
    //{
    //    try
    //    {
    //        SSM ssm = new SSM();
    //        xml = ssm.dekrypt(xml);
    //        xml = xml.Replace("&amp;", "&");
    //        xml = xml.Replace("&apos;", "'");

    //        xml = xml.Replace("&", "&amp;");
    //        xml = xml.Replace("'", "&apos;");

    //        XmlDocument xmlDoc = new XmlDocument();
    //        xmlDoc.LoadXml(xml);
    //        this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
    //        this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
    //        this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
    //        this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;

    //        //clean account name
    //        this.AccountName = this.AccountName.Replace("&amp;", "&");
    //        this.AccountName = this.AccountName.Replace("&apos;", "'");
    //        this.AccountName = this.AccountName.Replace("&quot;", "\"");

    //        this.AccountName = this.AccountName.Replace("&", "&amp;");
    //        this.AccountName = this.AccountName.Replace("'", "&apos;");
    //        this.AccountName = this.AccountName.Replace("\"", "&quot;");

    //        this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
    //        this.OriginatorName = xmlDoc.GetElementsByTagName("OriginatorName").Item(0).InnerText;


    //        //clean Originator Name
    //        this.OriginatorName = this.OriginatorName.Replace("&amp;", "&");
    //        this.OriginatorName = this.OriginatorName.Replace("&apos;", "'");
    //        this.OriginatorName = this.OriginatorName.Replace("&quot;", "\"");

    //        this.OriginatorName = this.OriginatorName.Replace("&", "&amp;");
    //        this.OriginatorName = this.OriginatorName.Replace("'", "&apos;");
    //        this.OriginatorName = this.OriginatorName.Replace("\"", "&quot;");

    //        this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;

    //        //clean Originator Name
    //        this.Narration = this.Narration.Replace("&amp;", "&");
    //        this.Narration = this.Narration.Replace("&apos;", "'");
    //        this.Narration = this.Narration.Replace("&quot;", "\"");

    //        this.Narration = this.Narration.Replace("&", "&amp;");
    //        this.Narration = this.Narration.Replace("'", "&apos;");
    //        this.Narration = this.Narration.Replace("\"", "&quot;");

    //        this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
    //        this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        new ErrorLog(ex);
    //        return false;
    //    }
    //}
}
