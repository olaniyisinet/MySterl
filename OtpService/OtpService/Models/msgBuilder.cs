using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;

/// <summary>
/// Summary description for msgBuilder
/// </summary>
public class msgBuilder
{
    public string mobile;
    public string message;
    public string response;
    private StringBuilder xml = new StringBuilder();
    public msgBuilder()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string buildRequest()
    {
        xml = new StringBuilder();
        xml.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>");
        xml.Append("<txtmessage>");
        xml.Append("<txtmobile>" + mobile + "</txtmobile>");
        xml.Append("<txtcontent>" + cleanString(message) + "</txtcontent>");
        xml.Append("</txtmessage>");
        return xml.ToString();
    }

    protected void readXML(string xml)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        mobile = xmlDoc.GetElementsByTagName("txtmobile").Item(0).InnerText;
        message = xmlDoc.GetElementsByTagName("txtcontent").Item(0).InnerText;
        message = dirtyString(message);
    }

    protected string cleanString(string str)
    {
        str = str.Replace("&", "&amp;");
        str = str.Replace("<", "&lt;");
        str = str.Replace(">", "&gt;");
        str = str.Replace("\"", "&quot;");
        str = str.Replace("'", "&apos;");
        return str;
    }

    protected string dirtyString(string str)
    {
        str = str.Replace("&amp;", "&");
        str = str.Replace("&lt;", "<");
        str = str.Replace("&gt;", ">");
        str = str.Replace("&quot;", "\"");
        str = str.Replace("&apos;", "'");
        return str;
    }
}