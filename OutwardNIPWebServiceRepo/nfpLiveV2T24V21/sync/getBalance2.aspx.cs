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

public partial class sync_getBalance2 : System.Web.UI.Page
{
    string bc;
    string cn; 
    string cc;
    string lc; 
    string sc;
    sbp.banks b = new sbp.banks();
    string formatval;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["roles"] != null)
        {
            try
            {
                string acctnum = Convert.ToString(Request.Params["acc"]);

                if (acctnum.Length == 10)
                {
                    NubanServices nu = new NubanServices();
                    acctnum = nu.NubanNumber(acctnum);
                }

                DataSet dst = b.getBalanceDetails(acctnum);
                if (dst.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dst.Tables[0].Rows[0];
                    bc = dr["bra_code"].ToString();
                    cn = dr["cus_num"].ToString();
                    cc = dr["cur_code"].ToString();
                    lc = dr["led_code"].ToString();
                    sc = dr["sub_acct_code"].ToString();
                }

                sbp.banks bank = new sbp.banks();
                DataSet ds = bank.getCusBalance(bc, cn, cc, lc, sc); //live banks
                //DataSet ds = bank.getCusBalanceTest(bc, cn, cc, lc, sc);//test banks

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
