using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDProcessor
{
    class Utility
    {
        public bool CardAuthenticate(string cardacct, Int32 last4digits)
        {
            bool found = false;
            string sql = "select * from tbl_USSD_card_auth where cardacct =@cat and last4carddigits =@l4d ";
            Connect c = new Connect(sql, true);
            c.addparam("@cat", cardacct);
            c.addparam("@l4d", last4digits);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                found = true;
            }
            else
            {
                found = false;
            }
            return found;
        }
        public static string ConvertMobile080(string mobile)
        {
            char[] trima = { '+', ' ' };
            mobile = mobile.Trim(trima);
            if (mobile.Length == 11 && mobile.StartsWith("0"))
            {
                return mobile;
            }
            if (mobile.Length >= 10)
            {
                mobile = "0" + mobile.Substring(mobile.Length - 10, 10);
                return mobile;
            }
            return mobile;
        }
        public static string ConvertMobile234(string mobile)
        {
            char[] trima = { '+', ' ' };
            mobile = mobile.Trim(trima);
            if (mobile.Length == 13 && mobile.StartsWith("234"))
            {
                return mobile;
            }
            if (mobile.Length >= 10)
            {
                mobile = "234" + mobile.Substring(mobile.Length - 10, 10);
                return mobile;
            }
            return mobile;
        }
        public int insertIntoInfobip(string msg, string msdn)
        {
            int val = 0; string phone = ""; string sender = "STERLING";
            phone = ConvertMobile234(msdn);
            string sql = "insert into proxytable (phone,sender,text) values(@phone,@sender,@msg)";
            Connect c = new Connect(sql, true, true, true);
            c.addparam("@phone", phone);
            c.addparam("@sender", sender);
            c.addparam("@msg", msg);
            try
            {
                val = c.query();
            }
            catch
            {
                val = 0;
            }
            return val;
        }
        public bool GetAcctByMobile(string mobnum, string acct)
        {
            bool found = false;
            try
            {
                EACBS1.banksSoapClient ws1 = new EACBS1.banksSoapClient();
                string Nuban = "";
                string Msisdn = mobnum.Replace("234", "0");
                DataSet ds = ws1.getCustomerAccountsByMobileNo2(Msisdn);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        Nuban = dr["NUBAN"].ToString();
                        if(Nuban == acct)
                        {
                            found = true;
                            return found;
                        }
                    }
                }
            }
            catch
            {
                found = false;
            }
            return found;
        }
    }
}
