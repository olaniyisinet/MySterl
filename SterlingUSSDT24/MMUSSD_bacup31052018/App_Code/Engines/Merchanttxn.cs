using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for Merchanttxn
/// </summary>
public class Merchanttxn : BaseEngine
{
    StringBuilder rqt = new StringBuilder();
    StringBuilder rsp = new StringBuilder();
    Gadget g = new Gadget();
    public string xml;
    public string SessionID;
    public string RequestorID;
    public string MerchantCode;
    public string ResponseCode;
    public string MerchantName;
    public string Amount;
    public string PayerPhoneNumber;
    public string FinancialInstitutions;
    public string PayerBVN;
    public string debitacct;
    public string MandateCode;
    public string PayerName;
    public string Telco;
    public string MerchantPhoneNumber;
    public string ReferenceCode;
    public string FinancialInstitutionCode;
    public string pin;
    public string Fee;

    public void CreateMerchantNameRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<MerchantNameRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<RequestorID>" + RequestorID + "</RequestorID>");
        rqt.Append("<MerchantCode>" + MerchantCode + "</MerchantCode>");
        rqt.Append("</MerchantNameRequest>");
    }
    public bool sendRequest()
    {
        bool ok = false;
        try
        {
            mpayenroll.MerchantEnrollmentEndPointClient ws = new mpayenroll.MerchantEnrollmentEndPointClient();
            SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());

            ws.MerchantNameEnquiry(ref str);
            xml = str;
            ok = readResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
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
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.MerchantName = xmlDoc.GetElementsByTagName("MerchantName").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }
    public void CreatePrePaymentRequest()
    {
        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<Pre-PaymentRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<RequestorID>" + RequestorID + "</RequestorID>");
        rqt.Append("<PayerPhoneNumber>" + PayerPhoneNumber + "</PayerPhoneNumber>");
        rqt.Append("<MerchantCode>" + MerchantCode + "</MerchantCode>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("</Pre-PaymentRequest>");
    }

    public bool sendPrePaymentEnquiRequest()
    {
        bool ok = false;
        try
        {
            mpay.PaymentWebServiceClient ws = new mpay.PaymentWebServiceClient();
            SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());

            ws.PrePayment(ref str);
            xml = str;
            ok = readPrePaymentResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ok = readPrePaymentResponse();
        }
        return ok;
    }
    public bool readPrePaymentResponse()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.RequestorID = xmlDoc.GetElementsByTagName("RequestorID").Item(0).InnerText;
            try
            {
                this.MerchantName = xmlDoc.GetElementsByTagName("MerchantName").Item(0).InnerText;
            }
            catch { }
            this.MerchantCode = xmlDoc.GetElementsByTagName("MerchantCode").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            return true;
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            return false;
        }
    }

    public void CreatePaymentRequest()
    {
        string Fincode = System.Web.Configuration.WebConfigurationManager.AppSettings["Fincode"].ToString();
        string name = "Sterling Bank"; string dob = "";

        Encrypto ec = new Encrypto();
        pin = ec.Decrypt(pin);

        rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        rqt.Append("<PaymentRequest>");
        rqt.Append("<SessionID>" + SessionID + "</SessionID>");
        rqt.Append("<RequestorID>" + RequestorID + "</RequestorID>");
        rqt.Append("<PayerPhoneNumber>" + PayerPhoneNumber + "</PayerPhoneNumber>");
        rqt.Append("<PayerBVN>" + PayerBVN + "</PayerBVN>");
        rqt.Append("<Telco>" + Telco + "</Telco>");
        rqt.Append("<Amount>" + Amount + "</Amount>");
        rqt.Append("<MerchantCode>" + MerchantCode + "</MerchantCode>");
        rqt.Append("<MandateCode>" + MandateCode + "</MandateCode>");
        rqt.Append("<FinancialInstitutionCode Name= " + "\"" + name + "\"" +  " accountNumber = " + "\"" +  debitacct + "\"" + " FISpecificInformation= " + "\"" + Fincode + "\"" + " >" + Fincode + "</FinancialInstitutionCode>");
        rqt.Append("<Params>");
        rqt.Append("<Param name=\"DOB\">" + dob + "</Param>");
        rqt.Append("<Param name=\"USSD Authentication PIN\">" + pin + "</Param>");
        rqt.Append("</Params>");
        rqt.Append("</PaymentRequest>");
    }
    public bool sendPaymentRequest()
    {
        bool ok = false;
        try
        {
            mpay.PaymentWebServiceClient ws = new mpay.PaymentWebServiceClient();
            SSM ssm = new SSM();
            string str = ssm.enkrypt(rqt.ToString());

            ws.Payment(ref str);
            xml = str;
            ok = readPaymentResponse();
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            ok = readPaymentResponse();
        }
        return ok;
    }
    public bool readPaymentResponse()
    {
        try
        {
            SSM ssm = new SSM();
            xml = ssm.dekrypt(xml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            this.SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            this.RequestorID = xmlDoc.GetElementsByTagName("RequestorID").Item(0).InnerText;
            this.PayerPhoneNumber = xmlDoc.GetElementsByTagName("PayerPhoneNumber").Item(0).InnerText;
            this.Telco = xmlDoc.GetElementsByTagName("Telco").Item(0).InnerText;
            this.MerchantCode = xmlDoc.GetElementsByTagName("MerchantCode").Item(0).InnerText;
            this.MerchantPhoneNumber = xmlDoc.GetElementsByTagName("MerchantPhoneNumber").Item(0).InnerText;
            this.ReferenceCode = xmlDoc.GetElementsByTagName("ReferenceCode").Item(0).InnerText;
            this.Amount = xmlDoc.GetElementsByTagName("Amount").Item(0).InnerText;
            this.ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            this.Fee = xmlDoc.GetElementsByTagName("Fee").Item(0).InnerText;
            this.FinancialInstitutionCode = xmlDoc.GetElementsByTagName("FinancialInstitutionCode").Item(0).InnerText;
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