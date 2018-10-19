using System;
using System.Web.Script.Serialization;

namespace com.sbpws.utility
{
    public class JSONize
    {
        public static string SerializeToString(object objectInstance)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(objectInstance);
        }

        public static object DeserializeFromString(string objectData, Type objectType)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize(objectData,objectType);
        }
    }
}