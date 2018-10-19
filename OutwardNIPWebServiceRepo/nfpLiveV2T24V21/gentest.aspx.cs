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

public partial class gentest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SSM sv = new SSM();
        string enk = sv.enkrypt("microsoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoftmicrosoft");
        Response.Write(enk);
        Response.Write("<br />");

        SSM sv1 = new SSM();
        enk = sv1.dekrypt(enk);
        Response.Write(enk);
        Response.Write("<br />");



    }
}
