using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_BulkFundTransferDD
{
    StringBuilder rqt = new StringBuilder();
    public string DestinationBankCode;
    public string ChannelCode;
    public string BatchNumber;
    public string NumberOfRecords;
    public string ResponseCode;
    public string xml;

    public void create()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTBulkDebitRequest>");
        rqt.Append("<Header>");
        rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rqt.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rqt.Append("</Header>");
    }

    public void addRecord(string RecID, string AccountNumber, string AccountName, string BillerName, string BillerID, string Narration, string PaymentReference, string MandateReferenceNumber, string Amount)
    {
        rqt.Append("<Record>");
        rqt.Append("<RecID>" + RecID + "</RecID>");
        rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rqt.Append("<AccountName>" + AccountName + "</AccountName>");
        rqt.Append("<BillerName>" + BillerName + "</BillerName>");
        rqt.Append("<BillerID>" + BillerID + "</BillerID>");
        rqt.Append("<Narration>" + Narration + "</Narration>");
        rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rqt.Append("<MandateReferenceNumber>" + MandateReferenceNumber + "</MandateReferenceNumber>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</Record>");
    }


    public bool makeRequest()
    {
        rqt.Append("</FTBulkDebitRequest>");
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
            //xml = nr.fundtransferbulkitem_dd(rqt.ToString());
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ResponseCode = "XX";
        }
        return ok;
    }

    protected bool readResponse()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList xmlhead = xmlDoc.GetElementsByTagName("Header");
            this.DestinationBankCode = xmlhead[0].ChildNodes[0].InnerText;
            this.ChannelCode = xmlhead[0].ChildNodes[1].InnerText;
            this.BatchNumber = xmlhead[0].ChildNodes[2].InnerText;
            this.NumberOfRecords = xmlhead[0].ChildNodes[3].InnerText;
            this.ResponseCode = xmlhead[0].ChildNodes[4].InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }
}
