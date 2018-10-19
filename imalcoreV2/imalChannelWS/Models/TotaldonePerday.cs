using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class TotaldonePerday
    {
        public string msgbox = "";
        public decimal Totaldone; public int totlcnt;
        public bool getTotalTransDonePerday(decimal Maxperday, decimal amt, string nuban)
        {
            DataSet ds = new DataSet();
            bool ok = false;
            string sql = "select ISNULL(SUM(amt),0) as totalTOday,ISNULL(count(amt),0) as cnt from tbl_nib_nip_outward " +
                " where nuban =@nu" +
                " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and amtTaken=1";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@nu", nuban);
            ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Totaldone = decimal.Parse(dr["totalTOday"].ToString());
                totlcnt = int.Parse(dr["cnt"].ToString());
                if (Totaldone + amt > Maxperday)
                {
                    ok = true;
                    msgbox = "You have exceeded the daily limit set for you for today.";
                }
                else
                {
                    ok = false;
                }
            }
            else
            {
                ok = false;
                msgbox = "Unable to get concession for today";
            }

            return ok;
        }
    }
}