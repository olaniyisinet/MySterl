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
/// Summary description for NubanServices
/// </summary>
public class NubanServices
{
    public string Frm_bra_code = "";
    public string Frm_cus_num = "";
    public string Frm_cur_code = "";
    public string Frm_led_code = "";
    public string Frm_sub_acct_code = "";

    public string NubanNumber(string val)
    {
        string nuban = "";
        sbp.banks b = new sbp.banks();
        DataSet ds = b.getNubanAccount(val);
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["AccountNumber"].ToString();
            Frm_bra_code = dr["BRA_CODE"].ToString();
            Frm_cus_num = dr["CUS_NUM"].ToString();
            Frm_cur_code = dr["CUR_CODE"].ToString();
            Frm_led_code = dr["LED_CODE"].ToString();
            Frm_sub_acct_code = dr["SUB_ACCT_CODE"].ToString();
        }
        return nuban;
    }
}
