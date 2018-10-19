using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_SingleStatusQuery
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string SourceInstitutionCode;
    public string SessionID;
    //public string DestinationInstitutionCode;
    public string ChannelCode;
    public string ResponseCode;
    public string xml;

    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<TSQuerySingleRequest>");
        rqt.Append("<SourceInstitutionCode>" + SourceInstitutionCode + "</SourceInstitutionCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("</TSQuerySingleRequest>");
    }
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
            xml = nr.txnstatusquerysingleitem(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "1x";
        }
        return ok;
    }
    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<TSQuerySingleResponse>");
        rsp.Append("<SourceInstitutionCode>" + SourceInstitutionCode + "</SourceInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</TSQuerySingleResponse>");

        SSMIn ssm = new SSMIn();
        string str = ssm.enkrypt(rsp.ToString());
        new ErrorLog(rsp.ToString());
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
            this.SourceInstitutionCode = xmlDoc.GetElementsByTagName("SourceInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }
    public bool readResponse()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SourceInstitutionCode = xmlDoc.GetElementsByTagName("SourceInstitutionCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;

            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }

}
