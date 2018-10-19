using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbpws.utility
{
    public class XMLize
    {
        public static string SerializeToString1(object objectInstance)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(); 
            ns.Add("", "");
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance,ns);
            }
            return sb.ToString();
        }


        public static string SerializeToString(object objectInstance, bool omitdeclare = false)
        {
            string txt = "";
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(objectInstance.GetType());
            var settings = new XmlWriterSettings(); 
            settings.OmitXmlDeclaration = omitdeclare;
            settings.Encoding = new UTF8Encoding(false);
            settings.ConformanceLevel = ConformanceLevel.Document;
            var  memoryStream = new MemoryStream();             
            using (var writer = XmlWriter.Create(memoryStream, settings))
            {
                serializer.Serialize(writer, objectInstance, emptyNamepsaces);
                txt = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            return txt;
        }

        public static object DeserializeFromString(string objectData, Type objectType)
        {
            var serializer = new XmlSerializer(objectType);
            object result;
            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }
            return result;
        }

 
 
    }
}

 