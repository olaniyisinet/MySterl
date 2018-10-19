using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
public partial class Sync_getBalance : System.Web.UI.Page
{
    string formatval;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["roles"] != null)
        {
            try
            {
                string tmp = Convert.ToString(Request.Params["acc"]);
                string[] s = tmp.Split('/');
                string bc = Convert.ToString(s[0]);
                string cn = Convert.ToString(s[1]);
                string cc = Convert.ToString(s[2]);
                string lc = Convert.ToString(s[3]);
                string sc = Convert.ToString(s[4]);

                sbp.banks bank = new sbp.banks();
                //DataSet ds = bank.getCusBalance(bc, cn, cc, lc, sc); live banks
                DataSet ds = bank.getCusBalanceTest(bc, cn, cc, lc, sc);//test banks

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    decimal bal = Convert.ToDecimal(dr["cle_bal"]);
                    string thecusname = dr["cus_sho_name"].ToString();
                    formatval = bal.ToString("#,###.00");
                    Response.Write(thecusname.ToString() + "__" + formatval);
                }
                else
                {
                    Response.Write("No record!__0");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Invalid account!__0");
                ErrorLog err = new ErrorLog(ex);
            }


        }
        else
        {
            Response.Redirect("../login.aspx");
        }

    }
}
