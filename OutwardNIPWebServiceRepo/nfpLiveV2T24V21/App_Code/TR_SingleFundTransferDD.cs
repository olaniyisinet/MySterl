using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_SingleFundTransferDD
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();

    public string SessionID;
    public string DestinationBankCode;
    public string ChannelCode;
    public string AccountName;
    public string AccountNumber;
    public string BillerName;
    public string BillerID;
    public string Narration;
    public string PaymentReference;
    public string MandateReferenceNumber;
    public string Amount;
    public string ResponseCode;
    public string xml;
    string resp;

    public string NameEnquiryRef;
    public string DestinationInstitutionCode;
    public string DebitAccountName;
    public string DebitAccountNumber;
    public string DebitBankVerificationNumber;
    public string DebitKYCLevel;
    public string BeneficiaryAccountName;
    public string BeneficiaryAccountNumber;
    public string BeneficiaryBankVerificationNumber;
    public string BeneficiaryKYCLevel;
    public string TransactionLocation;

    public string TransactionFee;

    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTSingleDebitRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<AccountName>" + AccountName + "</AccountName>");
        rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rqt.Append("<BillerName>" + BillerName + "</BillerName>");
        rqt.Append("<BillerID>" + BillerID + "</BillerID>");
        rqt.Append("<Narration>" + Narration + "</Narration>");
        rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rqt.Append("<MandateReferenceNumber>" + MandateReferenceNumber + "</MandateReferenceNumber>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</FTSingleDebitRequest>");
    }

    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<FTSingleDebitResponse>");
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
        rsp.Append("</FTSingleDebitResponse>");

        SSMIn ssm = new SSMIn();
        //new ErrorLog("DD Response " + rsp.ToString());
        Mylogger.Info("DD Response " + rsp.ToString());
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }


    public bool sendRequest()
    {
        bool ok = false;
        try
        {
            Nibbslive.NIPInterface nr = new Nibbslive.NIPInterface();
            //Nibbslive.NFPBankSimulationNoCryptoService nr = new Nibbslive.NFPBankSimulationNoCryptoService();

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
            //xml = nr.fundtransfersingleitem_dd(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            ResponseCode = "XX";
            AccountName = "Error";
        }
        return ok;
    }


    public bool readResponse()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
            this.BillerName = xmlDoc.GetElementsByTagName("BillerName").Item(0).InnerText;
            this.BillerID = xmlDoc.GetElementsByTagName("BillerID").Item(0).InnerText;
            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
            this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
            this.MandateReferenceNumber = xmlDoc.GetElementsByTagName("MandateReferenceNumber").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
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
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
    }
}
