using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;


public class TR_Balanceenquiry
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();

    public string SessionID;
    public string DestinationInstitutionCode;
    public string ChannelCode;
    public string AuthorizationCode;
    public string TargetAccountName;
    public string TargetBankVerificationNumber;
    public string ResponseCode;
    public string AvailableBalance;
    public string TargetAccountNumber;
    //public string SessionID;
    //public string DestinationBankCode;
    //public string ChannelCode;
    //public string SpecialCode;
    //public string AccountNumber;
    //public string AccountName;
    //public string ResponseCode;
    //public string AvailableBalance;

    public string xml;

    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<BalanceEnquiryRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<AuthorizationCode>" + AuthorizationCode + "</AuthorizationCode>");
        rqt.Append("<TargetAccountName>" + TargetAccountName + "</TargetAccountName>");
        rqt.Append("<TargetAccountNumber>" + TargetAccountNumber + "</TargetAccountNumber>");
        rqt.Append("</BalanceEnquiryRequest>");

        //rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        //rqt.Append("<BalanceEnquiryRequest>");
        //rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        //rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        //rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        //rqt.Append("<SpecialCode>" + SpecialCode + "</SpecialCode>");
        //rqt.Append("<AccountName>" + AccountName + "</AccountName>");
        //rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        //rqt.Append("</BalanceEnquiryRequest>");
    }
    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<BalanceEnquiryResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<AuthorizationCode>" + AuthorizationCode + "</AuthorizationCode>");
        rsp.Append("<TargetAccountName>" + TargetAccountName + "</TargetAccountName>");
        rsp.Append("<TargetBankVerificationNumber>" + TargetBankVerificationNumber + "</TargetBankVerificationNumber>");
        rsp.Append("<TargetAccountNumber>" + TargetAccountNumber + "</TargetAccountNumber>");
        rsp.Append("<AvailableBalance>" + AvailableBalance + "</AvailableBalance>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</BalanceEnquiryResponse>");

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
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AuthorizationCode = xmlDoc.GetElementsByTagName("AuthorizationCode").Item(0).InnerText;
            this.TargetAccountName = xmlDoc.GetElementsByTagName("TargetAccountName").Item(0).InnerText;
            this.TargetBankVerificationNumber = xmlDoc.GetElementsByTagName("TargetBankVerificationNumber").Item(0).InnerText;
            this.TargetAccountNumber = xmlDoc.GetElementsByTagName("TargetAccountNumber").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
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
            SSMIn ssm = new SSMIn();
            string str = ssm.enkrypt(rqt.ToString());
            //xml = nr.balanceenquiry(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "XX";
        }
        return ok;
    }
    public bool readResponse()
    {
        try
        {
            SSMIn ssm = new SSMIn();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AuthorizationCode = xmlDoc.GetElementsByTagName("AuthorizationCode").Item(0).InnerText;
            this.TargetAccountName = xmlDoc.GetElementsByTagName("TargetAccountName").Item(0).InnerText;
            this.TargetAccountNumber = xmlDoc.GetElementsByTagName("TargetAccountNumber").Item(0).InnerText;
            this.AvailableBalance = xmlDoc.GetElementsByTagName("AvailableBalance").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            //this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            //this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
            //this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            //this.SpecialCode = xmlDoc.GetElementsByTagName("SpecialCode").Item(0).InnerText;
            //this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            //this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
            //this.AvailableBalance = xmlDoc.GetElementsByTagName("AvailableBalance").Item(0).InnerText;
            //this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;

            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }
}
