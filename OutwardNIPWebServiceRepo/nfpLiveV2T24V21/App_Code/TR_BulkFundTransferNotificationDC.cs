using System;
using System.Text;
using System.Xml;
using System.Net;
using System.Configuration;

public class TR_BulkFundTransferNotificationDC
{
    StringBuilder rqt = new StringBuilder();
    public string DestinationBankCode;
    public string ChannelCode;
    public string BatchNumber;
    public string NumberOfRecords;
    public string ResponseCode;
    public Record[] Record;
    public string xml;

    //create a bulk notification request to nibss
    public void createRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<FTBulkCreditNotificationRequest>");
        rqt.Append("<Header>");
        rqt.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rqt.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rqt.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rqt.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rqt.Append("</Header>");
    }

    //add record to bulk notification
    public void addRecord(Record r)
    {
        rqt.Append("<Record>");
        rqt.Append("<RecID>" + r.RecID + "</RecID>");
        rqt.Append("<AccountNumber>" + r.AccountNumber + "</AccountNumber>");
        rqt.Append("<AccountName>" + r.AccountName + "</AccountName>");
        rqt.Append("<OriginatorName>" + r.OriginatorName + "</OriginatorName>");
        rqt.Append("<Narration>" + r.Narration + "</Narration>");
        rqt.Append("<PaymentReference>" + r.PaymentReference + "</PaymentReference>");
        rqt.Append("<Amount>" + r.Amount + "</Amount>");
        rqt.Append("<ResponseCode>" + r.ResponseCode + "</ResponseCode>");
        rqt.Append("</Record>");
    }

    //send bulk notification request to nibbs
    public bool sendRequest()
    {
        rqt.Append("</FTBulkCreditNotificationRequest>");
        bool ok = false;
        try
        {
            Nibbslive.NIPInterface nr = new Nibbslive.NIPInterface();
            //Nibbslive.NFPBankSimulationNoCryptoService nr = new Nibbslive.NFPBankSimulationNoCryptoService();
            SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());
            //xml = nr.fundtransfernotificationbulkitem_dc(str);
            ok = readResponse();
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            ResponseCode = "XX";
        }
        return ok;
    }


    //read a bulk ft request on us from nibss
    public bool readRequest()
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
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            return false;
        }
    }

    //create a bulk ft reponse to nibss request
    public string createResponse()
    {
        StringBuilder rsp = new StringBuilder();
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<FTBulkCreditNotificationResponse>");
        rsp.Append("<DestinationBankCode>" + DestinationBankCode + "</DestinationBankCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rsp.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</FTBulkCreditNotificationResponse>");

        SSM ssm = new SSM();
        string str = ssm.enkrypt(rsp.ToString());
        return str;
    }

    //read bulk notification response
    protected bool readResponse()
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
}
