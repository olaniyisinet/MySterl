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
/// Summary description for Errorlog20Secs
/// </summary>
public class Errorlog20Secs
{
	
    public Errorlog20Secs(string ex)
    {
        string root = ConfigurationManager.AppSettings["errorlog"].ToString();
        string err = ex;

        string timenow = DateTime.Now.ToString("yyyyMMddHHmmss_");
        DateTime dt = DateTime.Now;
        string fld = "20secs_" + dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + ".txt";
        string fld1 = "20secs_" + dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_1.txt";

        string path1 = root + fld;
        string path2 = root + fld1;



        try
        {
            if (!File.Exists(path1))
            {
                using (StreamWriter sw = File.CreateText(path1))
                {

                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path1))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        catch
        {
            if (!File.Exists(path2))
            {
                using (StreamWriter sw = File.CreateText(path2))
                {

                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path2))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

    }
}
