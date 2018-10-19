using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_BulkFundTransferDC
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string DestinationBankCode;
    public string ChannelCode;
    public string BatchNumber;
    public string NumberOfRecords;
    public string ResponseCode;
    public Record[] Record;
    public string xml;

    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTBulkCreditRequest>");
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
        rsp.Append("<FTBulkCreditResponse>");
        rsp.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rsp.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</FTBulkCreditResponse>");

        SSM ssm = new SSM();
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }

    public void addRecord(string RecID, string AccountNumber, string AccountName, string OriginatorName, string Narration, string PaymentReference, string Amount)
    {
        rqt.Append("<Record>");
        rqt.Append("<RecID>" + RecID + "</RecID>");
        rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rqt.Append("<AccountName>" + AccountName + "</AccountName>");
        rqt.Append("<OriginatorName>" + OriginatorName + "</OriginatorName>");
        rqt.Append("<Narration>" + Narration + "</Narration>");
        rqt.Append("<PaymentReference>" + PaymentReference + "</PaymentReference>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</Record>");
    }

    public bool makeRequest()
    {
        rqt.Append("</FTBulkCreditRequest>");
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
            //xml = nr.fundtransferbulkitem_dc(str);
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
            //XmlNodeList xmlhead = xmlDoc.GetElementsByTagName("Header");
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
                r.AccountName = xmlrec[i].ChildNodes[2].InnerText;
                r.OriginatorName = xmlrec[i].ChildNodes[3].InnerText;
                r.Narration = xmlrec[i].ChildNodes[4].InnerText;
                r.PaymentReference = xmlrec[i].ChildNodes[5].InnerText;
                r.Amount = xmlrec[i].ChildNodes[6].InnerText;
                r.ResponseCode = xmlrec[i].ChildNodes[7].InnerText;
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
