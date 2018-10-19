using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneDirectory;
using Sterling.BaseLIB.Utility;
using System.Text.RegularExpressions;
using System.Data;

namespace PlentyMoney
{

    #region Common Region
    public class Common
    {
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

        public static string RegexDecimal()
        {
            return @"^[0-9]\d*(\.\d+)?$";
        }

        public static string RegexDate(string val)
        {
            string[] treat =  val.Trim().Split('-');

            if (treat.Length == 3)
            {

            }
            else
            { 
            }
            return "";
        }

        public static string RegexDate()
        {
            return @"^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d$";
        }
    }

    #endregion

    public class PlentyInfo
    {
        public string CardType { get; set; }
        public string RequestType { get; set; }
        public string NUBAN { get; set; }
        public string Mobile { get; set; }
        public string CollectionType { get; set; }  //1: my branch, 2: others
        public string CustumerName { get; set; }
        public string CollectionBranchCode { get; set; }
        public string CollectionBranchStateCode { get; set; }
        public string Status { get; set; }
        public string RequestDate { get; set; }
        public string TreatedDate { get; set; }
        public string TreatedBy { get; set; }
        public string ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }

        public string Telephone { get; set; }
        public string GSM { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string Addrss { get; set; }
        public string City { get; set; }

        public string Region { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string DeliveryBranchCard { get; set; }
        public string DeliveryBranchPin { get; set; }

    }

}
