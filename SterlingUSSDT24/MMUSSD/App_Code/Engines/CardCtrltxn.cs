using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for CardCtrltxn
/// </summary>
public class CardCtrltxn : BaseEngine
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    Gadget g = new Gadget();
    public string RequestID;
    public string xml;
    public string Pan;
    public string RequestType;
    public string AccountNumber;
    public string ResponseCode;
    public string AccountType;
    public string Amount;
    public string ResponseText;


    public string CreateDailyLimitReq()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<CardRequest>");
        rqt.Append("<RequestID>" + RequestID + "</RequestID>");
        rqt.Append("<RequestType>" + RequestType + "</RequestType>");
        rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rqt.Append("<AccountType>" + AccountType + "</AccountType>");
        rqt.Append("<Pan>" + Pan + "</Pan>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</CardRequest>");
        //SSM ssm = new SSM();
        //string str = ssm.enkrypt(rqt.ToString());

        //return str;
        return rqt.ToString();
    }

    public bool readResponse(string respxml)
    {
        try
        {
            //SSM ssm = new SSM();
            //xml = ssm.dekrypt(xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(respxml);
            this.RequestID = xmlDoc.GetElementsByTagName("RequestId").Item(0).InnerText;
            this.RequestType = xmlDoc.GetElementsByTagName("RequestType").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            this.ResponseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;            
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }
    public bool SendRequest(string xml)
    {
        bool resp = false;
        CardCtrlWS.CardServClient cardws = new CardCtrlWS.CardServClient();
        string answer = cardws.CardOps(xml);
        try
        {           
            //SSM ssm = new SSM();
            //string str = ssm.enkrypt(rqt.ToString());           
            resp = readResponse(answer);
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = readResponse(answer);
        }
        return resp;
    }
    public string CreateReq()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<CardRequest>");
        rqt.Append("<RequestID>" + RequestID + "</RequestID>");
        rqt.Append("<RequestType>" + RequestType + "</RequestType>");
        rqt.Append("<AccountNumber>" + AccountNumber + "</AccountNumber>");
        rqt.Append("<AccountType>" + AccountType + "</AccountType>");
        rqt.Append("<Pan>" + Pan + "</Pan>");
        rqt.Append("</CardRequest>");
        //SSM ssm = new SSM();
        //string str = ssm.enkrypt(rqt.ToString());
        //return str;
        return rqt.ToString();
    }


}