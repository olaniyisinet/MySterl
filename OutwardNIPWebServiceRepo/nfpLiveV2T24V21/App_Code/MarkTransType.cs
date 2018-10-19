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
/// Summary description for MarkTransType
/// </summary>
public class MarkTransType
{
    public void markTransType(Int32 refid)
    {
        string sql = "update tbl_nibssmobile set outwardTrnsType = 1, appsTransType=1 where refid=@rid";
        Connect c = new Connect(sql, true);
        c.addparam("@rid", refid);
        c.query();
    }
}
