using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_SingleNameEnquiry
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();

    public string SessionID;

    //public string DestinationBankCode;
    public string DestinationInstitutionCode;
    public string BankVerificationNumber;
    public string KYCLevel;

    public string ChannelCode;
    public string AccountNumber;
    public string AccountName;
    public string ResponseCode;

    public string xml;
	
    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<NESingleRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rqt.Append("</NESingleRequest>");
    }

    public string createResponse()
    {
        this.AccountName = this.AccountName.Replace("&amp;", "&");
        this.AccountName = this.AccountName.Replace("&apos;", "'");
        this.AccountName = this.AccountName.Replace("&quot;", "\"");

        this.AccountName = this.AccountName.Replace("&", "&amp;");
        this.AccountName = this.AccountName.Replace("'", "&apos;");
        this.AccountName = this.AccountName.Replace("\"", "&quot;");

        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<NESingleResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rsp.Append("<AccountName>" + AccountName + "</AccountName>");
        rsp.Append("<BankVerificationNumber>" + BankVerificationNumber + "</BankVerificationNumber>");
        rsp.Append("<KYCLevel>" + KYCLevel + "</KYCLevel>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</NESingleResponse>");


        SSMIn ssm = new SSMIn();
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }

    public bool sendRequesteBILLSne()
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
            SSM1 ssm = new SSM1();
            string str = ssm.enkrypt(rqt.ToString());
            xml = nr.nameenquirysingleitem(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog("Error occured doing Name enquiry for ebills.  The response returned is " + xml + ex);
            ResponseCode = "96";
        }
        return ok;
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
            string InstCode = ConfigurationSettings.AppSettings["AppZoneIntCode"].ToString();
            if (AccountNumber.StartsWith("99"))
            {
                try
                {
                    //call bank one webservice
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<AccountNameVerificationRequest>");
                    sb.Append("<InstitutionCode>" + InstCode + "</InstitutionCode>");
                    sb.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
                    sb.Append("</AccountNameVerificationRequest>");
                    //init() pn = init();
                    string rsp1 = init().BankOneGetAccountName(sb.ToString());
                    //Deserialize
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(rsp1);
                    ok = readResponse(rsp1);
                }
                catch (Exception ex)
                {
                    AccountName = "";
                    ResponseCode = "07";
                }
            }
            else
            {
                SSM ssm = new SSM();
                string str = ssm.enkrypt(rqt.ToString());
                xml = nr.nameenquirysingleitem(str);
                ok = readResponse();
            }
        }
        catch (Exception ex)
        {
            Mylogger1.Error(ex);
            ResponseCode = "96";
        }
        return ok;
    }

    public bool readRequest()
    {
        try
        {
            SSMIn ssm = new SSMIn();
            xml = ssm.dekrypt(xml);
            //new ErrorLog("Name Enuiry " + xml);
            Mylogger.Info("Name Enuiry " + xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
    }

    public bool readResponse()
    {
        try
        {
            new ErrorLog("Read Inward NE " + xml);
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.DestinationInstitutionCode = xmlDoc.GetElementsByTagName("DestinationInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.AccountNumber = xmlDoc.GetElementsByTagName("AccountNumber").Item(0).InnerText;
            this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            //clean account name
            this.AccountName = this.AccountName.Replace("&", "&amp;");
            this.AccountName = this.AccountName.Replace("'", "&apos;");
            this.AccountName = this.AccountName.Replace("\"", "&quot;");

            this.BankVerificationNumber = xmlDoc.GetElementsByTagName("BankVerificationNumber").Item(0).InnerText;
            this.KYCLevel = xmlDoc.GetElementsByTagName("KYCLevel").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
        
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "96";
            return false;
        }
    }
    public BankOneService.SterlingFundsTransferService init()
    {
        BankOneService.SterlingFundsTransferService c = new BankOneService.SterlingFundsTransferService();
        string urlLive = ConfigurationSettings.AppSettings["BankOneService.SterlingFundsTransferService"].ToString();
        c.Url = urlLive;
        int ServPort = Convert.ToInt32(ConfigurationSettings.AppSettings["ServerPort"].ToString());
        string serProxy = ConfigurationSettings.AppSettings["ServerProxy"].ToString();
        IWebProxy proxy = new WebProxy(serProxy, ServPort);
        
        c.Proxy = proxy;
        c.RequestEncoding = Encoding.GetEncoding("utf-8");
        NetworkCredential netCredential = new NetworkCredential();
        Uri uri = new Uri(c.Url);
        //proxy.IsBypassed(uri);
        ICredentials credentials = netCredential.GetCredential(uri, "Basic");
        c.Credentials = credentials;
        c.PreAuthenticate = true;
        return c;
    }
    public bool readResponse(string xml)
    {
        try
        {
            new ErrorLog("Read Inward NE " + xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            //this.SessionID = SessionID;
            this.DestinationInstitutionCode = "999058";
            this.ChannelCode = "1";
            this.AccountNumber = xmlDoc.GetElementsByTagName("CustomerNumber").Item(0).InnerText;
            this.AccountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            //clean account name
            this.AccountName = this.AccountName.Replace("&", "&amp;");
            this.AccountName = this.AccountName.Replace("'", "&apos;");
            this.AccountName = this.AccountName.Replace("\"", "&quot;");
            this.BankVerificationNumber = xmlDoc.GetElementsByTagName("BVN").Item(0).InnerText;
            this.KYCLevel = xmlDoc.GetElementsByTagName("KYCLevel").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "96";
            return false;
        }
    }
}
