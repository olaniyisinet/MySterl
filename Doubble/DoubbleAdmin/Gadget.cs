using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DoubbleAdmin
{
    public class Gadget
    {
        public string getRole(string username)
        {
            string resp = "";

            string sql = "select  * from Users_tbl where Username = @userr";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@userr", username);
            DataTable dt = c.Select("rec").Tables[0];

            if(dt.Rows.Count > 0)
            {
                resp = dt.Rows[0]["Role"].ToString();
            }

            return resp;
        }

        public bool checkExisting(string username)
        {
            bool resp = false;

            string sql = "select  * from Users_tbl where Username = @userr";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@userr", username);
            DataTable dt = c.Select("rec").Tables[0];

            if (dt.Rows.Count > 0)
            {
                resp = true;
            }

            return resp;

        }

        public void updateLastPrompt(string username)
        {
            string sql = "update Users_tbl set LastLogin=@lprompt where Username=@username";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@username", username);
            c.AddParam("@lprompt", DateTime.Now);
            c.Select("rec");
        }

        public void Preterminate(string arrangementId, string action)
        {
            try
            {
                string sql = "update ForNonSterlings set Terminate='" + action + "' where ARRANGEMENT_ID=@arrangementId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@arrangementId", arrangementId);
                c.Select("rec");
            }
            catch (Exception e)
            {


            }
            try
            {
                string sql = "update ForSterlings set Terminate='" + action + "' where ARRANGEMENT_ID=@arrangementId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@arrangementId", arrangementId);
                c.Select("rec");
            }
            catch (Exception e)
            {


            }
        }

    }
}