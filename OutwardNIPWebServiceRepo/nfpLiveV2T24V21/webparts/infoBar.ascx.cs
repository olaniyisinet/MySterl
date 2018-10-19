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

public partial class webparts_infoBar : System.Web.UI.UserControl
{
    private bool _css;
    private string _msg;

    public bool css
    {
        get { return _css; }
        set { _css = value; }
    }
    public string msg
    {
        get { return _msg; }
        set
        {
            _msg = value;
            paint();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        paint();
    }

    public void paint()
    {
        if (msg == "")
        {
            infoBox.Visible = false;
        }
        else
        {
            infoBox.Visible = true;
        }

        if (css)
        {
            infoBox.CssClass = "infoTrue";
        }
        else
        {
            infoBox.CssClass = "infoFalse";
        }
        lblMsg.Text = msg;
    }
}
