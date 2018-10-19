using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_BulkFundTransferNotificationDD
{
    StringBuilder rqt = new StringBuilder();
    public string DestinationBankCode;
    public string ChannelCode;
    public string BatchNumber;
    public string NumberOfRecords;
    public string ResponseCode;
    public Record[] Record;

    public string xml;

 
    protected bool readRequest()
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList xmlhead = xmlDoc.GetElementsByTagName("Header");
            this.DestinationBankCode = xmlhead[0].ChildNodes[0].InnerText;
            this.ChannelCode = xmlhead[0].ChildNodes[1].InnerText;
            this.BatchNumber = xmlhead[0].ChildNodes[2].InnerText;
            this.NumberOfRecords = xmlhead[0].ChildNodes[3].InnerText;

            int cnt = 0;
            XmlNodeList xmlrec = xmlDoc.GetElementsByTagName("Record");
            Record = new Record[cnt];
            for (int i = 0; i < xmlrec.Count; i++)
            {
                Record[i].RecID = xmlrec[i].ChildNodes[0].InnerText;
                Record[i].AccountNumber = xmlrec[i].ChildNodes[1].InnerText;
                Record[i].AccountName = xmlrec[i].ChildNodes[2].InnerText;
                Record[i].BillerName = xmlrec[i].ChildNodes[3].InnerText;
                Record[i].BillerID = xmlrec[i].ChildNodes[4].InnerText;
                Record[i].Narration = xmlrec[i].ChildNodes[5].InnerText;
                Record[i].PaymentReference = xmlrec[i].ChildNodes[6].InnerText;
                Record[i].MandateReferenceNumber = xmlrec[i].ChildNodes[7].InnerText;
                Record[i].Amount = xmlrec[i].ChildNodes[8].InnerText;
                Record[i].ResponseCode = xmlrec[i].ChildNodes[9].InnerText;
            }
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }

    public bool sendResponse()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTBulkDebitNotificationResponse>");
        rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rqt.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rqt.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rqt.Append("</FTBulkDebitNotificationResponse>");

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
            //xml = nr.(rqt.ToString());
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "XX";
        }
        return ok;
    }
}
