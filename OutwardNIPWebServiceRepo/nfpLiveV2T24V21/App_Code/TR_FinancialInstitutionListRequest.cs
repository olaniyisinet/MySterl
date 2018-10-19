using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Text;

/// <summary>
/// Summary description for TR_FinancialInstitutionListRequest
/// </summary>
public class TR_FinancialInstitutionListRequest
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    public string BatchNumber;
    public string DestinationInstitutionCode;
    public string ChannelCode;
    public string NumberOfRecords;
    public string ResponseCode;
    public string TransactionLocation;
    public string xml;
    public Record[] Record;

    public string createResponse()
    {
        rsp.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rsp.Append("<FinancialInstitutionListResponse>");
        rsp.Append("<BatchNumber>" + BatchNumber + "</BatchNumber>");
        rsp.Append("<DestinationInstitutionCode>" + DestinationInstitutionCode + "</DestinationInstitutionCode>");
        rsp.Append("<ChannelCode>" + ChannelCode + "</ChannelCode>");
        rsp.Append("<NumberOfRecords>" + NumberOfRecords + "</NumberOfRecords>");
        rsp.Append("<ResponseCode>" + ResponseCode + "</ResponseCode>");
        rsp.Append("</FinancialInstitutionListResponse>");

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
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList xmlhead = xmlDoc.GetElementsByTagName("Header");
            this.BatchNumber = xmlhead[0].ChildNodes[0].InnerText;
            this.DestinationInstitutionCode = this.BatchNumber.Substring(0, 6);
            this.ChannelCode = xmlhead[0].ChildNodes[2].InnerText;
            this.NumberOfRecords = xmlhead[0].ChildNodes[1].InnerText;
            this.TransactionLocation = xmlhead[0].ChildNodes[3].InnerText;

            //int cnt = 0;
            XmlNodeList xmlrec = xmlDoc.GetElementsByTagName("Record");
            Record = new Record[xmlrec.Count];
            for (int i = 0; i < xmlrec.Count; i++)
            {
                Record r = new Record();
                r.InstitutionCode = xmlrec[i].ChildNodes[0].InnerText;
                r.InstitutionName = xmlrec[i].ChildNodes[1].InnerText;
                r.Category = xmlrec[i].ChildNodes[2].InnerText;
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