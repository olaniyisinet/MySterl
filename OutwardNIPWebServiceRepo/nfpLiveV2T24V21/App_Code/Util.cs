using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
/// <summary>
/// Summary description for Util
/// </summary>
public static class Util
{
    public static string Serialize<T>(T value)
    {
        string XML;
        XmlSerializer ser = new XmlSerializer(typeof(T));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = new UnicodeEncoding(false, false);
        settings.Indent = false;

        using (StringWriter textWriter = new Utf8StringWriter())
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                ser.Serialize(xmlWriter, value, ns);
            }
            XML = textWriter.ToString();
        }
        return XML;
    }
    public static AccountNameVerificationResponse Deserialize(string req)
    {
        AccountNameVerificationResponse res = new AccountNameVerificationResponse();
        using (StringReader stringReader = new StringReader(req))
        {
            XmlSerializer ser = new XmlSerializer(typeof(AccountNameVerificationResponse));
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            res = (AccountNameVerificationResponse)ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
        }
        return res;

    }
    
    public static DepositResponse DeserializeDeposit(string req)
    {
        DepositResponse res = new DepositResponse();
        using (StringReader stringReader = new StringReader(req))
        {
            XmlSerializer ser = new XmlSerializer(typeof(DepositResponse));
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            res = (DepositResponse)ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
        }
        return res;

    }
}