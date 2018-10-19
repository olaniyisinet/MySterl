using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

/// <summary>
/// Summary description for GetCusName
/// </summary>
public class GetCusName
{
    public string getCusName(string acct, string cusname)
    {
        string TheName = ""; string nuban = "";
        StreamReader sr = new StreamReader("E:/wwwroot/nfpLiveV2T24/CusName/" + "cusname.txt");
        String readline = string.Empty;
        string[] splitline;
        char[] splitter = new char[] { '|' };

        while ((readline = sr.ReadLine()) != null)
        {
            splitline = readline.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            nuban = Convert.ToString(splitline[0]);
            if (nuban == acct)
            {
                TheName = Convert.ToString(splitline[1]);
                return TheName;
            }
            else
            {
                TheName = cusname;
            }
        }
        return TheName;
    }
}
