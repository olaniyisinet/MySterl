using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace CustomerAccount
{
    public static class CustomerAccount_Util
    {
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        public static void formatMobile_to_234(ref string mobile)
        {
            if (mobile.StartsWith("0"))
            {
                mobile = mobile.Remove(0, 1);
                mobile = string.Format("234{0}", mobile);
            }
            if (!mobile.StartsWith("0") && !mobile.StartsWith("234"))
            {
                mobile = string.Format("234{0}", mobile);
            }
        }

        public static void formatMobile_to_080(ref string mobile)
        {
            if (mobile.StartsWith("234"))
            {
                mobile = mobile.Remove(0, 3);
                mobile = string.Format("0{0}", mobile);
            }
            if (!mobile.StartsWith("0") && !mobile.StartsWith("234"))
            {
                mobile = string.Format("0{0}", mobile);
            }
        }

        public static int CountRows(DataTable dataTable)
        {
            int k = 0;
            try
            {
                k = dataTable.Rows.Count;
            }
            catch
            {
                k = -1;
            }
            return k;
        }

        public static int CountRows(DataSet dataset, int tableid = 0)
        {
            int k = 0;
            try
            {
                k = dataset.Tables[tableid].Rows.Count;
            }
            catch
            {
                k = -1;
            }
            return k;
        }
    }

    public class StagingArea
    {
        public string SN { get; set; }
        public string BVN { get; set; }
        public string AccountNo { get; set; }
        public string AccountType { get; set; }
        public string PhoneNo { get; set; }
        public string DateofBirth { get; set; }
        public string DateUploaded { get; set; }
        public string BatchID { get; set; }
        public string RequestFlag { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }

    public class ApplicationLog_
    {
        public ApplicationLog_(string req, string dest)
        {
            string pth = ConfigurationManager.AppSettings[dest];
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            if (!File.Exists(pth))
            {
                using (StreamWriter sw = File.CreateText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + req);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + req);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public ApplicationLog_(string req, string dest, string refID)
        {
            string pth = ConfigurationManager.AppSettings[dest];
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            string folder = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            folder = ConfigurationManager.AppSettings[dest] + "\\" + folder + dt.ToString("dd");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string filename = folder + "\\" + refID + ".txt";
            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(req);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(req);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public ApplicationLog_(Exception req, string dest)
        {
            string pth = ConfigurationManager.AppSettings[dest];
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            if (!File.Exists(pth))
            {
                using (StreamWriter sw = File.CreateText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + req);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + req);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
    }

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
            return js.Deserialize(objectData, objectType);
        }
    }
}