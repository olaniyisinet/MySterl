 
using System.Xml;

namespace MMWS
{
    public static class XMLTool
    {
        public static string GetNodeData(string xmltext, string nodename)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmltext);
            return xml.GetElementsByTagName(nodename).Item(0).InnerText; 
        }

        public static string GetNodeAttribute(string xmltext, string nodename, string attributeName)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmltext);
            return xml.GetElementsByTagName(nodename).Item(0).Attributes[attributeName].Value;  
        }
    }
}
