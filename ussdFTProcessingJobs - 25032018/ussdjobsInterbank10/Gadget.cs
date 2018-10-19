using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ussdjobsInterbank10
{
    class Gadget
    {
        public string ConvertMobile234(string mobile)
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

        public bool isFoundInBLackList(string mob)
        {
            bool found = false;
            string sql = "select * from Go_BlackList where Misdn =@mob";
            Connect c = new Connect(sql, true);
            c.addparam("@mob", mob);
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

    }
}
