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
using System.Xml;

public partial class test : System.Web.UI.Page
{
    decimal maxPerTrans = 0; decimal maxPerday = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        //TestTQ.Requery ws = new TestTQ.Requery();
        //string rsp = ws.requery("999999151104113427151104113427", "1", "999999"); string[] bits = instr.Split('+');
        string Name = "Chigozie anyasor Woke";
        string[] bits = Name.Split(' ');
        Name = bits[0] + " " +  bits[1].Substring(0, 1).ToUpper();
        
    }

    public bool getMaxperTransPerday(string bracode, string cusnum, string curcode, string ledcode, string subacctcode)
    {
        bool found = false;
        DataSet ds = new DataSet();
        string sql = "";
        //sql = "select maxpertran,maxperday from tbl_nipconcessionTrnxlimits where statusflag=1 " +
        //  " and bra_code=@bc and cus_num=@cn and cur_code=@cc and led_code=@lc and sub_acct_code =@sc";
        sql = "select bra_code,cus_num,cur_code,led_code,sub_acct_code,maxpertran,maxperday,addedby,statusflag from tbl_nipconcessionTrnxlimits where statusflag=1 " +
            " and bra_code=@bc and cus_num=@cn and cur_code=@cc and led_code=@lc and sub_acct_code =@sc";
        try
        {
            Connect c = new Connect(sql, true);
            c.addparam("@bc", bracode);
            c.addparam("@cn", cusnum);
            c.addparam("@cc", curcode);
            c.addparam("@lc", ledcode);
            c.addparam("@sc", subacctcode);
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                maxPerTrans = decimal.Parse(dr["maxpertran"].ToString());
                maxPerday = decimal.Parse(dr["maxperday"].ToString());
                found = true;
            }
            else
            {
                found = false;
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            found = false;
        }
        return found;
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
 
        bool HasConcessionPerTransPerday = getMaxperTransPerday("223", "522620", "1", "9", "0");
    }
}
