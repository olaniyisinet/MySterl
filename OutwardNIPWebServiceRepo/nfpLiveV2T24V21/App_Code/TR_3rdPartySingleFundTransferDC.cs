using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Net;
using System.Text;

/// <summary>
/// Summary description for TR_3rdPartySingleFundTransferDC
/// </summary>
public class TR_3rdPartySingleFundTransferDC
{
    Gadget g = new Gadget();
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string SessionID;
    public string DestinationBankCode;
    public string ChannelCode;
    public string AccountName;
    public string AccountNumber;
    public string OriginatorName;
    public string Narration;
    public string PaymentReference;
    public string Amount;
    public string ResponseCode;
    public string toNibss;

    public string DestinationInstitutionCode;
    public string BeneficiaryAccountName;
    public string NameEnquiryRef;
    public string BeneficiaryAccountNumber;
    public string BeneficiaryBankVerificationNumber;
    public string BeneficiaryKYCLevel;
    public string OriginatorAccountName;
    public string OriginatorAccountNumber;
    public string OriginatorBankVerificationNumber;
    public string OriginatorKYCLevel;
    public string TransactionLocation;
    public string xml;

    public void createRequest()
    {
        //DestinationInstitutionCode = DestinationBankCode;

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
        rqt.Append("<FTSingleCreditRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<NameEnquiryRef>" + NameEnquiryRef + "</NameEnquiryRef>");
        rqt.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<BeneficiaryAccountName>" + BeneficiaryAccountName + "</BeneficiaryAccountName>");
        rqt.Append("<BeneficiaryAccountNumber>" + BeneficiaryAccountNumber + "</BeneficiaryAccountNumber>");
        rqt.Append("<BeneficiaryBankVerificationNumber>" + BeneficiaryBankVerificationNumber + "</BeneficiaryBankVerificationNumber>");
        rqt.Append("<BeneficiaryKYCLevel>" + BeneficiaryKYCLevel + "</BeneficiaryKYCLevel>");
        rqt.Append("<OriginatorAccountName>" + OriginatorAccountName + "</OriginatorAccountName>");
        rqt.Append("<OriginatorAccountNumber>" + OriginatorAccountNumber + "</OriginatorAccountNumber>");
        rqt.Append("<OriginatorBankVerificationNumber>" + OriginatorBankVerificationNumber + "</OriginatorBankVerificationNumber>");
        rqt.Append("<OriginatorKYCLevel>" + OriginatorKYCLevel + "</OriginatorKYCLevel>");
        rqt.Append("<TransactionLocation>" + TransactionLocation + "</TransactionLocation>");
        rqt.Append("<Narration>" + Narration + "</Narration>");
        rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</FTSingleCreditRequest>");
    }

    public string createResponse()
    {
        this.AccountName = this.AccountName.Replace("&amp;", "&");
        this.AccountName = this.AccountName.Replace("&apos;", "'");
        this.AccountName = this.AccountName.Replace("&quot;", "\"");

        this.AccountName = this.AccountName.Replace("&", "&amp;");
        this.AccountName = this.AccountName.Replace("'", "&apos;");
        this.AccountName = this.AccountName.Replace("\"", "&quot;");


        this.Narration = this.Narration.Replace("&amp;", "&");
        this.Narration = this.Narration.Replace("&apos;", "'");
        this.Narration = this.Narration.Replace("&quot;", "\"");

        this.Narration = this.Narration.Replace("&", "&amp;");
        this.Narration = this.Narration.Replace("'", "&apos;");
        this.Narration = this.Narration.Replace("\"", "&quot;");


        this.OriginatorName = this.OriginatorName.Replace("&amp;", "&");
        this.OriginatorName = this.OriginatorName.Replace("&apos;", "'");
        this.OriginatorName = this.OriginatorName.Replace("&quot;", "\"");

        this.OriginatorName = this.OriginatorName.Replace("&", "&amp;");
        this.OriginatorName = this.OriginatorName.Replace("'", "&apos;");
        this.OriginatorName = this.OriginatorName.Replace("\"", "&quot;");


        this.PaymentReference = this.PaymentReference.Replace("&amp;", "&");
        this.PaymentReference = this.PaymentReference.Replace("&apos;", "'");
        this.PaymentReference = this.PaymentReference.Replace("&quot;", "\"");

        this.PaymentReference = this.PaymentReference.Replace("&", "&amp;");
        this.PaymentReference = this.PaymentReference.Replace("'", "&apos;");
        this.PaymentReference = this.PaymentReference.Replace("\"", "&quot;");

        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<FTSingleCreditResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<AccountName>" + AccountName + "</AccountName>");
        rsp.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rsp.Append("<OriginatorName>" + OriginatorName + "</OriginatorName>");
        rsp.Append("<Narration>" + Narration + "</Narration>");
        rsp.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rsp.Append("<Amount>" + Amount + "</Amount>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</FTSingleCreditResponse>");

        SSM ssm = new SSM();
        new ErrorLog("FT response to be sent " + rsp.ToString());
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }

    public bool sendRequest()
    {
        bool ok = false;
        try
        {
            Nibbslive.NIPInterface nr = new Nibbslive.NIPInterface();
            nr.Timeout = 60000;
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
            new ErrorLog(rqt.ToString());

            //SSM2 ssm = new SSM2();
            SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());

            xml = nr.fundtransfersingleitem_dc(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog("Error Occured calling NIBSS ws " + ex);
            ResponseCode = "1x";
            ok = readResponse();
        }
        return ok;
    }


    public bool sendRequest2()
    {
        bool ok = false;
        try
        {
            Nibbslive.NIPInterface nr = new Nibbslive.NIPInterface();
            nr.Timeout = 60000;
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
            //new ErrorLog(rqt.ToString());
            Mylogger1.Info(rqt.ToString());

            SSM2 ssm = new SSM2();
            //SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());

            xml = nr.fundtransfersingleitem_dc(str);
            ok = readResponse2();
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error Occured calling NIBSS ws " + ex);
            Mylogger1.Error("Error Occured calling NIBSS ws ", ex);
            ResponseCode = "1x";
            ok = readResponse();
        }
        return ok;
    }
    //public bool sendRequest()
    //{
    //    bool ok = false;
    //    try
    //    {
    //        NipThirdparty.NIPOFIInterfaceService nr = new NipThirdparty.NIPOFIInterfaceService();
    //        //Nibbslive.NFPBankSimulationNoCryptoService nr = new Nibbslive.NFPBankSimulationNoCryptoService();

    //        //set a proxy
    //        bool useproxy = Convert.ToBoolean(ConfigurationManager.AppSettings["proxyUse"]);
    //        if (useproxy)
    //        {
    //            string url = Convert.ToString(ConfigurationManager.AppSettings["proxyURL"]);
    //            int port = Convert.ToInt16(ConfigurationManager.AppSettings["proxyPort"]);
    //            WebProxy p = new WebProxy(url, port);
    //            nr.Proxy = p;
    //        }

    //        SSM ssm = new SSM();
    //        string str = ssm.enkrypt(rqt.ToString());

    //        xml = nr.fundtransfersingleitem_dc(str);
    //        ok = readResponse();
    //    }
    //    catch (Exception ex)
    //    {
    //        new ErrorLog(ex);
    //        ResponseCode = "1x";
    //        ok = readResponse();
    //    }
    //    return ok;
    //}

    public bool readResponse()
    {
        try
        {
            //SSM2 ssm = new SSM2();
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            xml = xml.Replace("&amp;", "&");
            xml = xml.Replace("&apos;", "'");

            xml = xml.Replace("&", "&amp;");
            xml = xml.Replace("'", "&apos;");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            //clean account name
            this.AccountName = this.AccountName.Replace("&amp;", "&");
            this.AccountName = this.AccountName.Replace("&apos;", "'");
            this.AccountName = this.AccountName.Replace("&quot;", "\"");

            this.AccountName = this.AccountName.Replace("&", "&amp;");
            this.AccountName = this.AccountName.Replace("'", "&apos;");
            this.AccountName = this.AccountName.Replace("\"", "&quot;");

            this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
            this.OriginatorName = xmlDoc.GetElementsByTagName("OriginatorName").Item(0).InnerText;

            //clean originator
            this.OriginatorName = this.OriginatorName.Replace("&amp;", "&");
            this.OriginatorName = this.OriginatorName.Replace("&apos;", "'");
            this.OriginatorName = this.OriginatorName.Replace("&quot;", "\"");

            this.OriginatorName = this.OriginatorName.Replace("& ", "&amp;");
            this.OriginatorName = this.OriginatorName.Replace("'", "&apos;");
            this.OriginatorName = this.OriginatorName.Replace("\"", "&quot;");

            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
            //clean narration


            this.Narration = this.Narration.Replace("&amp;", "&");
            this.Narration = this.Narration.Replace("&apos;", "'");
            this.Narration = this.Narration.Replace("&quot;", "\"");

            this.Narration = this.Narration.Replace("& ", "&amp;");
            this.Narration = this.Narration.Replace("'", "&apos;");
            this.Narration = this.Narration.Replace("\"", "&quot;");

            this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
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
    public bool readResponse2()
    {
        try
        {
            SSM2 ssm = new SSM2();
            //SSM ssm = new SSM();
            Gadget g = new Gadget();

            new ErrorLog("Resp Before Decrypt::" + xml);
            xml = ssm.dekrypt(xml);

            xml = xml.Replace("&amp;", "&");
            xml = xml.Replace("&apos;", "'");

            xml = xml.Replace("&", "&amp;");
            xml = xml.Replace("'", "&apos;");

            new ErrorLog("Resp After Decrypt::" + xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.NameEnquiryRef = xmlDoc.GetElementsByTagName("NameEnquiryRef").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;

            this.BeneficiaryAccountName = g.CleanXmlText(xmlDoc.GetElementsByTagName("BeneficiaryAccountName").Item(0).InnerText);
            BeneficiaryAccountNumber = xmlDoc.GetElementsByTagName("BeneficiaryAccountNumber").Item(0).InnerText;
            BeneficiaryKYCLevel = xmlDoc.GetElementsByTagName("BeneficiaryKYCLevel").Item(0).InnerText;
            BeneficiaryBankVerificationNumber = xmlDoc.GetElementsByTagName("BeneficiaryBankVerificationNumber").Item(0).InnerText;
            //clean account name

            this.OriginatorAccountName = g.CleanXmlText(xmlDoc.GetElementsByTagName("OriginatorAccountName").Item(0).InnerText);
            OriginatorAccountNumber = xmlDoc.GetElementsByTagName("OriginatorAccountNumber").Item(0).InnerText;
            OriginatorBankVerificationNumber = xmlDoc.GetElementsByTagName("OriginatorBankVerificationNumber").Item(0).InnerText;
            OriginatorKYCLevel = xmlDoc.GetElementsByTagName("OriginatorKYCLevel").Item(0).InnerText;

            this.Narration = g.CleanXmlText(xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText);
            //clean narration


            this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            this.TransactionLocation = xmlDoc.GetElementsByTagName("TransactionLocation").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "1x";
            return false;
        }
    }

    public bool readRequest()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);
            //xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><FTSingleCreditRequest><SessionID>039232110524153307110524153308</SessionID><DestinationBankCode>434</DestinationBankCode><ChannelCode>1</ChannelCode><AccountName>Anyasor Chigozie</AccountName><AccountNumber>223124951190</AccountNumber><OriginatorName>Hassan Abdul Abdul1</OriginatorName><Narration>Transfer from 039 to 232</Narration><PaymentReference>8888888888</PaymentReference><Amount>222.00</Amount></FTSingleCreditRequest>";
            //xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><FTSingleCreditRequest><SessionID>214232910519165816110519165816</SessionID><DestinationBankCode>232</DestinationBankCode><ChannelCode>1</ChannelCode><AccountName>Okesola Adedayo Opeoluwa</AccountName><AccountNumber>223210447190</AccountNumber><OriginatorName>Hassan Abdul Abdul1</OriginatorName><Narration>Transfer from 039 to 232</Narration><PaymentReference>8888888888</PaymentReference><Amount>100.00</Amount></FTSingleCreditRequest>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
            this.OriginatorName = xmlDoc.GetElementsByTagName("OriginatorName").Item(0).InnerText;
            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
            this.PaymentReference = xmlDoc.GetElementsByTagName("PaymentReference").Item(0).InnerText;
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