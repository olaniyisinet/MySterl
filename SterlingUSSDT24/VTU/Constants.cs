using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Configuration;
using System.IO;

namespace VTU.UTILITIES
{
    #region Constant Region
    public class Constants
    {
        public  const int Status_Yellow = 0;
        public  const int Status_Green= 1;
        public  const int Status_Red = 2;
        public const int Status_Blue = 89;//on transit
        public const int Status_Timeout = 90;//on time out

        public const int RequestType_Self = 1;
        public const int RequestType_Other = 2;

        public const int ChannelID = 9;
        public const int AppID = 26;

        public const string  mfionChannelID = "05";//test : 11, live: 12

        public const int vtu_STL = 1;
        public const int vtu_MM = 2;

        public const int TELCO_ETISALAT = 1;
        public const int TELCO_GLO = 2;
        public const int TELCO_AIRTEL = 3;
        public const int TELCO_MTN = 4;


    }
    #endregion

    #region Common Region
    public class Common
    {
        public static void formatMobile(ref string mobile)
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

        public static string GetSubString(string Tag,int Index,int lenght)
        {
            if (String.IsNullOrEmpty(Tag))
            {
                return string.Empty;
            }
            else
            {
                return Tag.Substring(Index, lenght);
            }
        }

        public static string GetRef(string Tag)
        {
            if(String.IsNullOrEmpty(Tag))
            {
               return string.Format("{0}", DateTime.Now.Ticks);
            }
            else
            {
              return string.Format("{0}{1}", DateTime.Now.Ticks,Tag);
            }
        }
      
        public static string GetDefaultPage()
        {
            return "~/Account/Login.aspx";
        }

        public static int GetIntegerValue(Object obj)
        {
            int retVal = 0;
            try
            {
                retVal = (!(obj is DBNull) ? Convert.ToInt32(obj.ToString()) : 0);
            }
            catch
            {
            }
            return retVal;


        }

        public static float GetSingleValue(Object obj)
        {
            Single retVal = 0;
            try
            {
                retVal = (!(obj is DBNull) ? Convert.ToSingle(obj.ToString()) : 0);
            }
            catch
            {
            }
            return retVal;
        }

        public static string GetStringValue(object obj)
        {
            string s = "";
            try
            {
                s = (!(obj is DBNull) ? obj.ToString() : "");
            }
            catch
            {
            }
            return s;
        }

        public static string GetStringValue_ZeroString(object obj)
        {
            string s = "0";
            try
            {
                s = (!(obj is DBNull) ? obj.ToString() : "0");
            }
            catch
            {
            }
            return s;
        }

        public static string GetStringValue_NullString(object obj)
        {
            string s = null;
            try
            {
                s = (!(obj is DBNull) ? obj.ToString() : "");
            }
            catch
            {
            }
            return s;
        }

        public static decimal GetDecimalValue(Object obj)
        {
            decimal retVal = 0M;
            try
            {
                retVal = (!(obj is DBNull) ? Convert.ToDecimal(obj.ToString()) : 0M);
            }
            catch
            {
            }
            return retVal;
        }

        public static decimal tryGetDecimalValue(Object obj)
        {
            decimal retVal = 0M;
            try
            {
                retVal = (!(obj is DBNull) ? Convert.ToDecimal(obj.ToString()) : 0M);
            }
            catch
            {
                retVal = -99M;
            }
            return retVal;
        }

        public static bool GetBoolValue(Object obj)
        {
            bool retVal = false;
            try
            {
                retVal = (!(obj is DBNull) ? (bool)obj : false);
            }
            catch { }
            return retVal;
        }

        public static DateTime? GetDateNullValue(Object obj)
        {
            DateTime? retVal = null;
            try
            {
                if (!(obj is DBNull))
                {
                    retVal = Convert.ToDateTime(obj);
                }
                else
                {
                    retVal = null;
                }
            }
            catch { }
            return retVal;
        }

        public static DateTime GetDateValue(Object obj)
        {
            DateTime retVal = DateTime.Now;
            try
            {
                if (!(obj is DBNull))
                {
                    retVal = (DateTime)obj;
                }
            }
            catch { }
            return retVal;
        }

        public static string GetDateStringValue(Object obj)
        {
            string retVal = DateTime.Now.ToShortDateString();
            try
            {
                retVal = (!(obj is DBNull) ? ((DateTime)obj).ToShortDateString() : "");
            }
            catch { }
            return retVal;
        }

        public static string FormatDate_Long(DateTime dt)
        {
            if (dt != null)
            {
                return String.Format("{0:dd MMMM yyyy}", dt);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate_Long1(DateTime dt)
        {
            if (dt != null)
            {
                return String.Format("{0:MMMM dd, yyyy}", dt);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate(DateTime? dt)
        {
            if (dt != null)
            {
                return String.Format("{0:dd-MMM-yyyy}", dt);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate_Short(DateTime? dt)
        {
            if (dt != null)
            {
                return String.Format("{0:dd/MM/yyyy}", dt);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate(DateTime dt)
        {
            if (dt != null)
            {
                return String.Format("{0:dd-MMM-yyyy}", dt);
            }
            else
            {
                return string.Empty;
            }


        }

        public static string FormatDate_YYYY(DateTime? dt)
        {
            if (dt != null)
            {
                return String.Format("{0:yyyy/MM/dd}", dt);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate_Get_YYYY(DateTime? dt)
        {
            if (dt != null)
            {
                return String.Format("{0:yyyy}", dt);
            }
            else
            {
                return string.Empty;
            }


        }

        public static string FormatDate_YYYY_1(string dt)
        {
            if (dt != null)
            {
                try
                {
                    return String.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(dt));
                }
                catch { return string.Empty; }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate(string dt)
        {
            if (dt != null)
            {
                try
                {
                    return String.Format("{0:dd-MMM-yyyy}", Convert.ToDateTime(dt));
                }
                catch { return string.Empty; }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string FormatDate(object dt)
        {
            if (dt != null)
            {
                try
                {
                    return String.Format("{0:dd-MMM-yyyy}", Convert.ToDateTime(dt));
                }
                catch { return string.Empty; }
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<string> GetYear(int from)
        {
            List<string> listYear = new List<string>();
            if (from == -1) //get current year + 1 and current year - 1
            {
                DateTime dtime = DateTime.Now;
                string year = dtime.Year.ToString();
                int mainYear = Convert.ToInt32(year);
                int startYear = mainYear - 1;
                int endYear = mainYear;// +1;
                int i = endYear;
                while (i >= startYear)
                {
                    listYear.Add(String.Format("{0}", i.ToString()));
                    i--;
                }
            }
            else
            {
                DateTime dtime = DateTime.Now;
                string year = dtime.Year.ToString();
                int mainYear = Convert.ToInt32(year);
                //int i = mainYear - (mainYear - from);
                int i = Convert.ToInt32(from);
                while (i <= mainYear)
                {
                    listYear.Add(String.Format("{0}", mainYear.ToString()));
                    mainYear--;
                }
            }
            return listYear;
        }

        public static void SendSMS(string sender, string reciever, string msg)
        {
            //sms.send();
        }

    }

    public class SterlingRegex
    {
        public static bool IsMatch(string strVal, string pattern)
        {
            var match = Regex.Match(strVal, pattern, RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static string RegexInt()
        {
            return @"^\d+$";
        }

        public static string RegexEmail()
        {
            return @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        }

        public static string RegexPhone(int min, int max)
        {
            string str = string.Format(@"^\d{{0},{1}}$", min, max);
            //return "^\d{9,14}$";
            return str;
        }

        public static string RegexPhone()
        {

            return @"^\d{9,14}$";
        }

        public static string RegexNUBAN()
        {
            return @"^\d{10,10}$";
        }

        public static string RegexDecimal()
        {
            return @"^[0-9]\d*(\.\d+)?$";
        }

        public static string RegexFloat()
        {
            return "";
        }
    }

    #endregion

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

    public class ApplicationLog
    {
        public ApplicationLog(string req, string dest)
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

        public ApplicationLog(string req, string dest, string refID)
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

        public ApplicationLog(Exception req, string dest)
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
}
