using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for BankOneClass
/// </summary>
public class BankOneClass
{
    StringBuilder rqt = new StringBuilder();
    public string accountNumber;
    public string accountName;
    public string status;
    public string phoneNumber;
    public string last6Digits;
    public string kycLevel;
    public string accountTransferLimit;
    public string statusMessage;
    public string BVN;
    public decimal availableBal;
    public decimal ledgerBal;
    string key = ConfigurationManager.AppSettings["BankOneKey"];


    public string createRequestForNameEnq()
    {
        rqt.Append("<AccountNameVerification>");
        rqt.Append("<Authentication>");
        rqt.Append("<Key>" + key + "</Key>");
        rqt.Append("</Authentication>");
        rqt.Append("<AccountNameVerificationRequest>");
        rqt.Append("<AccountNumber>" + accountNumber + "</AccountNumber>");
        rqt.Append("</AccountNameVerificationRequest>");
        rqt.Append("</AccountNameVerification>");
        return rqt.ToString();
    }

    public bool readResponseForNameEnq(string xml)
    {
        bool resp = false;
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.status = xmlDoc.GetElementsByTagName("Status").Item(0).InnerText;
            this.phoneNumber = xmlDoc.GetElementsByTagName("PhoneNumber").Item(0).InnerText;
            this.BVN = xmlDoc.GetElementsByTagName("BVN").Item(0).InnerText;
            this.accountName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
            resp = true;
            return resp;

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return resp;
        }

    }

    public string createReqForCardValidation()
    {
        rqt.Append("<CardValidation>");
        rqt.Append("<Authentication>");
        rqt.Append("<Key>" + key + "</Key>");
        rqt.Append("</Authentication>");
        rqt.Append("<CardValidationRequest>");
        rqt.Append("<Digits>" + last6Digits + "</Digits>");
        rqt.Append("<AccountNumber>" + accountNumber + "</AccountNumber>");
        rqt.Append("</CardValidationRequest>");
        rqt.Append("</CardValidation>");
        return rqt.ToString();
    }

    public bool readRespForCardValidation(string xml)
    {
        bool resp = false;
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.status = xmlDoc.GetElementsByTagName("Status").Item(0).InnerText;
            this.statusMessage = xmlDoc.GetElementsByTagName("StatusMessage").Item(0).InnerText;
            resp = true;
            return resp;

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return resp;
        }
    }

    public string createReqForBalanceEnq()
    {
        //<BalanceEnquiry><Authentication><Key>22948DD9-A03D-42C1-88D8-3A297D0F4F51</Key></Authentication><BalanceEnquiryRequest><AccountNumber>1100005284</AccountNumber></BalanceEnquiryRequest></BalanceEnquiry>
        rqt.Append("<BalanceEnquiry>");
        rqt.Append("<Authentication>");
        rqt.Append("<Key>" + key + "</Key>");
        rqt.Append("</Authentication>");
        rqt.Append("<BalanceEnquiryRequest>");
        rqt.Append("<AccountNumber>" + accountNumber + "</AccountNumber>");
        rqt.Append("</BalanceEnquiryRequest>");
        rqt.Append("</BalanceEnquiry>");
        return rqt.ToString();
    }

    public bool readRespForBalEnq(string input)
    {
        bool resp = false;
        //          <BalanceEnquiryResponse>< ResponseCode > 00 </ ResponseCode >< StatusMessage />< LedgerBalance > 87,201.80 </ LedgerBalance >< AvailableBalance > 87,201.80 </ AvailableBalance ></ BalanceEnquiryResponse >

        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);
            this.availableBal = Convert.ToDecimal(xmlDoc.GetElementsByTagName("AvailableBalance").Item(0).InnerText);
            this.ledgerBal = Convert.ToDecimal(xmlDoc.GetElementsByTagName("LedgerBalance").Item(0).InnerText);
            resp = true;
            return resp;

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return resp;
        }
    }

}