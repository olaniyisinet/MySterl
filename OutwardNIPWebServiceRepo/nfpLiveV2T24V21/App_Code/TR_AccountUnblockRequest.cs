using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Net;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for TR_AccountUnblockRequest
/// </summary>
public class TR_AccountUnblockRequest
{
    Gadget g = new Gadget();
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();

    public string SessionID;
    public string DestinationInstitutionCode;
    public string ChannelCode;
    public string ReferenceCode;
    public string TargetAccountName;
    public string TargetBankVerificationNumber;
    public string TargetAccountNumber;
    public string ReasonCode;
    public string ResponseCode;
    public string Narration;
    public string Amount;
    public string xml;

    public void createRequest()
    {
        this.TargetAccountName = this.TargetAccountName.Replace("&amp;", "&");
        this.TargetAccountName = this.TargetAccountName.Replace("&apos;", "'");
        this.TargetAccountName = this.TargetAccountName.Replace("&quot;", "\"");

        this.TargetAccountName = this.TargetAccountName.Replace("&", "&amp;");
        this.TargetAccountName = this.TargetAccountName.Replace("'", "&apos;");
        this.TargetAccountName = this.TargetAccountName.Replace("\"", "&quot;");

        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<AmountBlockRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<ReferenceCode>" + ReferenceCode + "</ReferenceCode>");
        rqt.Append("<TargetAccountName>" + TargetAccountName + "</TargetAccountName>");
        rqt.Append("<TargetBankVerificationNumber>" + TargetBankVerificationNumber + "</TargetBankVerificationNumber>");
        rqt.Append("<TargetAccountNumber>" + TargetAccountNumber + "</TargetAccountNumber>");
        rqt.Append("<ReasonCode>" + ReasonCode + "</ReasonCode>");
        rqt.Append("<Narration>" + Narration + "</Narration>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</AmountBlockRequest>");
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
            this.ReferenceCode = xmlDoc.GetElementsByTagName("ReferenceCode").Item(0).InnerText;
            this.TargetAccountName = xmlDoc.GetElementsByTagName("TargetAccountName").Item(0).InnerText;
            //clean account name
            this.TargetAccountName = this.TargetAccountName.Replace("&amp;", "&");
            this.TargetAccountName = this.TargetAccountName.Replace("&apos;", "'");
            this.TargetAccountName = this.TargetAccountName.Replace("&quot;", "\"");

            this.TargetAccountName = this.TargetAccountName.Replace("&", "&amp;");
            this.TargetAccountName = this.TargetAccountName.Replace("'", "&apos;");
            this.TargetAccountName = this.TargetAccountName.Replace("\"", "&quot;");

            this.TargetBankVerificationNumber = xmlDoc.GetElementsByTagName("TargetBankVerificationNumber").Item(0).InnerText;
            this.TargetAccountNumber = xmlDoc.GetElementsByTagName("TargetAccountNumber").Item(0).InnerText;
            this.ReasonCode = xmlDoc.GetElementsByTagName("ReasonCode").Item(0).InnerText;

            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
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
    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<AmountBlockResponse>");
        rsp.Append("<SessionID>" + SessionID + "</SessionID>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<ReferenceCode>" + ReferenceCode + "</ReferenceCode>");
        rsp.Append("<TargetAccountName>" + TargetAccountName + "</TargetAccountName>");
        rsp.Append("<TargetBankVerificationNumber>" + TargetBankVerificationNumber + "</TargetBankVerificationNumber>");
        rsp.Append("<TargetAccountNumber>" + TargetAccountNumber + "</TargetAccountNumber>");
        rsp.Append("<ReasonCode>" + ReasonCode + "</ReasonCode>");
        rsp.Append("<Narration>" + Narration + "</Narration>");
        rsp.Append("<Amount>" + Amount + "</Amount>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</AmountBlockResponse>");

        SSMIn ssm = new SSMIn();
        string str = ssm.enkrypt(rsp.ToString());
        //string str = rsp.ToString();
        return str;
    }

    public bool readRequest()
    {
        try
        {
            SSMIn ssm = new SSMIn();
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
            this.ReferenceCode = xmlDoc.GetElementsByTagName("ReferenceCode").Item(0).InnerText;
            this.TargetAccountName = xmlDoc.GetElementsByTagName("TargetAccountName").Item(0).InnerText;
            //clean account name
            this.TargetAccountName = this.TargetAccountName.Replace("&amp;", "&");
            this.TargetAccountName = this.TargetAccountName.Replace("&apos;", "'");
            this.TargetAccountName = this.TargetAccountName.Replace("&quot;", "\"");

            this.TargetAccountName = this.TargetAccountName.Replace("&", "&amp;");
            this.TargetAccountName = this.TargetAccountName.Replace("'", "&apos;");
            this.TargetAccountName = this.TargetAccountName.Replace("\"", "&quot;");

            this.TargetBankVerificationNumber = xmlDoc.GetElementsByTagName("TargetBankVerificationNumber").Item(0).InnerText;
            this.TargetAccountNumber = xmlDoc.GetElementsByTagName("TargetAccountNumber").Item(0).InnerText;

            this.ReasonCode = xmlDoc.GetElementsByTagName("ReasonCode").Item(0).InnerText;
            this.Narration = xmlDoc.GetElementsByTagName("Narration").Item(0).InnerText;
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