using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace BankTellerFundsTransfer_lagos3
{
    class PowerUserService
    {
        public PowerUser getUser(string username)
        {
            //get user's details from database
            Connect c = new Connect("spd_Getuserdetails");
            c.addparam("username", username);
            DataSet ds = c.query("user");
            PowerUser u = new PowerUser();
            if (ds.Tables[0].Rows.Count > 0)
            {
                u = setUser(ds.Tables[0].Rows[0]);
            }
            else
            {
                u.username = "";
            }

            return u;
        }
        public PowerUser setUser(DataRow dr)
        {
            PowerUser u = new PowerUser();
            u.username = dr["username"].ToString();
            u.firstname = dr["firstname"].ToString();
            u.lastname = dr["lastname"].ToString();
            u.fullname = u.lastname + ", " + u.firstname;
            u.starttime = dr["starttime"].ToString();
            u.endtime = dr["endtime"].ToString();
            u.bracode = dr["bra_code"].ToString();
            u.tellerId = dr["tellerid"].ToString();
            //u.utype = dr["usertype"].ToString();
            u.email = dr["email"].ToString();
            return u;
        }
    }
}
