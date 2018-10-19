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

public partial class sync_getRecords : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string trxcode = Request.Params["trx"].ToString();
        if (trxcode != "")
        {
            TransactionService tsv = new TransactionService();
            DataSet ds = tsv.getTrnxSubRecords(trxcode);
            //gvRecords.DataSource = ds;
            //gvRecords.DataBind();
            Gadget g = new Gadget();

            string txt = "";

            txt += "<table border='1' cellpadding='2' celspacing='0' width='100%'><tr>" +
                "<th>Payment Ref</th>" +
                "<th>Beneficiary's Bank</th>" +
                "<th>Account No</th>" +
                "<th>Beneficiary (Schedule)</th>" +
                "<th>Beneficiary (Name Request)</th>" +
                "<th>Status</th>" +
                "<th>Fee (=N=)</th>" +
                "<th>Amount (=N=)</th>" +
                "</tr>";
            decimal amt = 0;
            decimal totala = 0;
            decimal fee = 0;
            decimal totalf = 0;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];

                amt = Convert.ToDecimal(dr["amt"]);
                fee = Convert.ToDecimal(dr["feecharge"]);
                totala += amt;
                totalf += fee;           

                txt += "<tr>" +
                    "<td style='text-align:center;'>" + dr["paymentRef"].ToString() + "</td>" +
                    "<td>" + dr["benebank"].ToString() + "</td>" +
                    "<td>" + dr["beneaccount"].ToString() + "</td>" +
                    "<td>" + dr["benename"].ToString() + "</td>" +
                    "<td>" + dr["nameresponse"].ToString() + "</td>" +
                    "<td>" + g.getStatus(dr["statusflag"].ToString()) + "</td>" +
                    "<td style='text-align:right;'>" + g.printMoney(fee) + "</td>" +
                    "<td style='text-align:right;'>" + g.printMoney(amt) + "</td>" +
                    "</tr>";
            }


            txt += "<tr>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td style='text-align:right;'><strong>TOTAL:</strong></td>" +
                "<td style='text-align:right;'><strong>" + g.printMoney(totalf) + "</strong></td>" +
                "<td style='text-align:right;'><strong>" + g.printMoney(totala) + "</strong></td>" +
                "</tr>";


            txt += "<tr>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td>&nbsp;</td>" +
                "<td style='text-align:right;'><strong>GRAND TOTAL:</strong></td>" +
                "<td style='text-align:right;'>&nbsp;</td>" +
                "<td style='text-align:right;'><strong>" + g.printMoney(totala + totalf) + "</strong></td>" +
                "</tr></table>";

            Response.Write(txt);

        }
    }

}
