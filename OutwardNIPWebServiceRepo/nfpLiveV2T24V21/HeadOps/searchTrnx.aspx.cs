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
using System.IO;

public partial class HeadOps_searchTrnx : System.Web.UI.Page
{
    public Gadget gm = new Gadget();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            calendar1.iDay = DateTime.Now.Day.ToString();
            calendar1.iMonth = DateTime.Now.Month.ToString();
            calendar1.iYear = DateTime.Now.Year.ToString();

            calendar2.iDay = DateTime.Now.Day.ToString();
            calendar2.iMonth = DateTime.Now.Month.ToString();
            calendar2.iYear = DateTime.Now.Year.ToString();

            PowerUserService psv = new PowerUserService();
            PowerUser p = psv.getUser(User.Identity.Name);
            Session["bracode"] = p.bracode;
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        PowerUserService pus = new PowerUserService();
        PowerUser u = pus.getUser(User.Identity.Name);

        //select the transaction by search
        string sql = "select transactioncode, inputedby, approvedby,  max(inputdate) as inputdate, " +
            "max(approveddate) as approveddate, max(approvevalue) as approvevalue, count(transactioncode) as cn, " +
            "(max(bra_code) + '-' + max(cus_num) + '-' + max(cur_code) + '-' + max(led_code) + '-' + max(sub_acct_code)) as accnum, " +
            "max(accname) as accname, min(statusflag) as statusflag, max(Transnature) as transnature, max(docfilename) as docfilename " +
            "from tbl_nibbstrans where transnature in (0, 1) and  Orig_bra_code = @bracode " +
            "and inputdate > @sdate and inputdate < @edate and inputdate >= getdate() ";

        string cat = ddlCategory.SelectedValue;
        string val = txtVal.Text.Trim();

        int v = 0;
        try
        {
            v = Convert.ToInt32(val);
        }
        catch (Exception ex){}

        switch (cat)
        {
            case "1": sql += "and transactioncode = '" + val + "' "; break;
            case "2": sql += "and (bra_code + '-' + cus_num + '-' + cur_code + '-' + led_code + '-' + sub_acct_code) = '" + val + "' "; break;
            case "3": sql += "and inputedby like '%" + val + "%' "; break;
            case "4": sql += "and approvedby like '%" + val + "%' "; break;
            case "5": sql += "and Transnature = " + v + " "; break;
            case "6": sql += "and approvevalue = " + v + " "; break;
        }
        sql += "group by transactioncode, inputedby, approvedby ";
        try
        {
            Connect c = new Connect(sql, true);
            c.addparam("bracode", u.bracode);
            c.addparam("sdate", calendar1.getDate());
            c.addparam("edate", calendar2.getDate().AddDays(1));
            DataSet ds = c.query("recs");
            gvTransactions.DataSource = ds;
            gvTransactions.DataBind();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    protected string fileDownload(object doc)
    {
        string file = doc.ToString();
        string ft = file.ToLower().Substring(file.Length - 3, 3);

        string btn = "<img src='../images/" + ft + ".png' border='0' />";
        string b = Convert.ToString(Session["bracode"]);
        if (b == "")
        {
            FormsAuthentication.RedirectToLoginPage();
        }
        string path = ConfigurationManager.AppSettings["uploadpath"].ToString();
        path = Path.Combine("..\\uploads\\", b + "\\" + file);

        return "<a target='_blank' href='" + path + "'>" + btn + "</a>";
    }

    protected string getAction(object status, object tcode)
    {
        string code = tcode.ToString();
        string act = status.ToString();
        string fxn = "";

        switch (act)
        {
            case "0": //Pending for Name Request
                //do name request for transaction
                fxn = "<input type='button' value='Send' onclick='doNameRequest(\"" + code + "\");' />";
                break;
            case "1": //Awaiting NIBSS Name Response
                //requery transaction
                fxn = "<input type='button' value='ReSend' onclick='reqNameEnquiry(\"" + code + "\");' />";
                break;
            case "2": //Ready for HOP Authorization
                //mail HOP
                fxn = "<input type='button' value='Authorize' onclick='trnxAuthorize(\"" + code + "\");' />";
                break;
            case "3": //Awaiting NIBSS Transfer Response
                //requery transaction trnxTransStatus
                //fxn = "<input type='button' value='ReQuery' onclick='trnxRequery(\"" + code + "\");' />";
                fxn = "";
                break;
            case "31": //Awaiting NIBSS Transfer Response
                //requery transaction
                //fxn = "<input type='button' value='Transfer' onclick='trnxRetransfer(\"" + code + "\");' />";
                fxn = "";
                break;
            case "32": //Transaction Rejected by NIbss
                //requery transaction
                fxn = "<input type='button' value='Reverse' onclick='trnxReverse(\"" + code + "\");' />";
                break;
            case "4": //Successful Transaction
                fxn = "";
                break;
        }


        return fxn;
    }
    protected string getStatusQuery(object status, object tcode)
    {
        string code = tcode.ToString();
        string act = status.ToString();
        string fxn = "";

        switch (act)
        {
            case "0": //Pending for Name Request
                //Query transaction status
                fxn = "";
                break;
            case "1": //Awaiting NIBSS Name Response
                //requery transaction
                //fxn = "requeryNameRequest";
                fxn = "";
                break;
            case "2": //Ready for HOP Authorization
                //mail HOP
                fxn = "";
                break;
            case "3": //Awaiting NIBSS Transfer Response
                //requery transaction
                fxn = "<input type='button' value='Query Status' onclick='trnxTransStatus(\"" + code + "\");' />";
                break;
            case "4": //Successful Transaction
                fxn = "<input type='button' value='Query Status' onclick='trnxTransStatus(\"" + code + "\");' />";
                break;
            case "31":
                //requery transaction
                fxn = "<input type='button' value='Query Status' onclick='trnxTransStatus(\"" + code + "\");' />";
                break;
        }


        return fxn;
    }
    protected string getDecline(object status, object tcode)
    {
        string code = tcode.ToString();
        string act = status.ToString();
        string fxn = "";

        switch (act)
        {
            case "0": //Pending for Name Request
                //do name request for transaction
                fxn = "<input type='button' value='Decline' onclick='trnxReject(\"" + code + "\");' />";
                break;
            case "99": //Decline
                fxn = "Declined Request";
                break;
        }
        return fxn;
    }
    protected string sayDate(object dtm)
    {
        DateTime dt = Convert.ToDateTime(dtm);
        DateTime def = new DateTime(1900, 1, 1);
        if (dt == def)
        {
            return "";
        }
        return dt.ToString(); ;
    }
}
