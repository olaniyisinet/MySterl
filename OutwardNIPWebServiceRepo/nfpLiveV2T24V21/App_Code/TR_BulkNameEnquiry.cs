using System;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.Net;
using System.Configuration;

public class TR_BulkNameEnquiry
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string DestinationBankCode;
    public string ChannelCode;
    public string BatchNumber;
    public string NumberOfRecords;
    public string ResponseCode;
    public string xml;
    public Record[] Record;    

    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<NEBulkRequest>");
        rqt.Append("<Header>");
        rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rqt.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rqt.Append("</Header>");
    }

    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<NEBulkResponse>");
        rsp.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rsp.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</NEBulkResponse>");

        SSM ssm = new SSM();
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }

    public void addRecord(string recId, string accountnumber)
    {
        rqt.Append("<Record>");
        rqt.Append("<RecID>" + recId + "</RecID>");
        rqt.Append("<AccountNumber>" + accountnumber + "</AccountNumber>");
        rqt.Append("</Record>");
    }

    public bool sendRequest()
    {
        rqt.Append("</NEBulkRequest>");
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
            //xml = nr.nameenquirybulkitem(str);
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
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
            this.ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            this.BatchNumber = xmlDoc.GetElementsByTagName("BatchNumber").Item(0).InnerText;
            this.NumberOfRecords = xmlDoc.GetElementsByTagName("NumberOfRecords").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText; 
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }

    public bool readRequest()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList xmlhead = xmlDoc.GetElementsByTagName("Header");
            this.BatchNumber = xmlhead[0].ChildNodes[2].InnerText;
            this.DestinationBankCode = this.BatchNumber.Substring(0, 3);
            this.ChannelCode = xmlhead[0].ChildNodes[1].InnerText;
            this.NumberOfRecords = xmlhead[0].ChildNodes[3].InnerText;

            //int cnt = 0;
            XmlNodeList xmlrec = xmlDoc.GetElementsByTagName("Record");
            Record = new Record[xmlrec.Count];
            for (int i = 0; i < xmlrec.Count; i++)
            {
                Record r = new Record();
                r.RecID = xmlrec[i].ChildNodes[0].InnerText;
                r.AccountNumber = xmlrec[i].ChildNodes[1].InnerText;
                Record[i] = r;
            }
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }


}
