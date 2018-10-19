using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for TotaldonePerday
/// </summary>
public class TotaldonePerday
{
    public string msgbox = "";
    public decimal Totaldone; public int totlcnt;
    public bool getTotalTransDonePerday(string bra_code, string cus_num, string cur_code, string led_code, string sub_act_code, decimal Maxperday, decimal amt, string nuban)
    {
        DataSet ds = new DataSet();
        bool ok = false;
        string sql = "select ISNULL(SUM(amt),0) as totalTOday,ISNULL(count(amt),0) as cnt from tbl_nibssmobile " +
            " where nuban =@nu" +
            " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and vTellerMsg=1";
        Connect c = new Connect(sql, true);
        c.addparam("@nu", nuban);
        //c.addparam("@bc", bra_code);
        //c.addparam("@cn", cus_num);
        //c.addparam("@cc", cur_code);
        //c.addparam("@lc", led_code);
        //c.addparam("@sc", sub_act_code);
        ds = c.query("rec");
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
    public bool getTotalTransDonePerday1(string bra_code, string cus_num, string cur_code, string led_code, string sub_act_code, decimal maxpertrans, decimal maxperday, decimal amt)
    {
        DataSet ds = new DataSet();
        bool ok = false;
        string sql = "select ISNULL(SUM(amt),0) as totalTOday from tbl_nibssmobile " +
            " where bra_code = @bc and cus_num=@cn and cur_code=@cc and led_code =@lc and sub_acct_code=@sc " +
            " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and vTellerMsg=1";
        Connect c = new Connect(sql, true);
        c.addparam("@bc", bra_code);
        c.addparam("@cn", cus_num);
        c.addparam("@cc", cur_code);
        c.addparam("@lc", led_code);
        c.addparam("@sc", sub_act_code);
        ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            Totaldone = decimal.Parse(dr["totalTOday"].ToString());
            if (Totaldone + amt > maxpertrans)
            {
                ok = false;
                msgbox = "You have exceeded the daily limit set for you for today.";
            }
            else if (Totaldone + amt > maxperday)
            {
                ok = false;
                msgbox = "You have exceeded the daily limit set for you for today.";
            }
            else
            {
                ok = true;
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
