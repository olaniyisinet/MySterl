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


public class ErrorLog
{

    string[] sizeArry = new String[] { "Byes", "KB", "MB", "GB" };
    public string Get_Size_in_KB_MB_GB(ulong sizebytes, int index)
    {

        if (sizebytes < 1000) return sizebytes + sizeArry[index];

        else return Get_Size_in_KB_MB_GB(sizebytes / 1024, ++index);

    }
    public ErrorLog(Exception ex)
    {
        new ErrorLog(ex.ToString());
    }

    public ErrorLog(string ex)
    {
        string root = ConfigurationManager.AppSettings["errorlog"].ToString();
        string err = ex;

        string timenow = DateTime.Now.ToString("yyyyMMddHHmmss_");
        DateTime dt = DateTime.Now;
        string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + ".txt";
        string fld1 = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_1.txt";

        string path1 = root + fld;
        string path2 = root + fld1;

        string fileName = @"E:\AppLogs\nfpLive\errorlog\" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
        FileInfo fi = new FileInfo(fileName);
        //string mgval = Get_Size_in_KB_MB_GB((ulong)fi.Length, 0);
        string mgval = "";
        try
        {
            mgval = Get_Size_in_KB_MB_GB((ulong)fi.Length, 0);
        }
        catch
        {
        }
        if (mgval.ToUpper().Contains("MB"))
        {
            mgval = mgval.Replace("MB", "");
            Int32 mgIntVal = Int32.Parse(mgval);

            if (mgIntVal > 1)
            {
                //rename the file to start afresh if the size is above 1mb
                try
                {
                    File.Move(@"E:\AppLogs\nfpLive\errorlog\" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt", @"E:\AppLogs\nfpLive\errorlog\" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss") + ".txt"); // Try to rename

                }
                catch (IOException ex1)
                {

                }
            }
        }

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

    public ErrorLog(string bracode, string ex)
    {
        string pth = ConfigurationManager.AppSettings["errorlog"].ToString();
        string err = ex;
        DateTime dt = DateTime.Now;
        string fld = bracode + "_" + dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
        pth += fld + dt.ToString("dd") + ".txt";
        if (!File.Exists(pth))
        {
            using (StreamWriter sw = File.CreateText(pth))
            {
                sw.WriteLine(dt.ToString() + " : " + err);
                sw.WriteLine(" ");
                sw.Close();
                sw.Dispose();
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(pth))
            {
                sw.WriteLine(dt.ToString() + " : " + err);
                sw.WriteLine(" ");
                sw.Close();
                sw.Dispose();
            }

        }

    }

    public ErrorLog(string ex, int flag)
    {
        string root = ConfigurationManager.AppSettings["tracer"].ToString();
        string err = ex;

        string timenow = DateTime.Now.ToString("yyyyMMddHHmmss_");
        DateTime dt = DateTime.Now;
        string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + ".txt";
        string fld1 = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_" + dt.ToString("dd") + "_1.txt";

        string path1 = root + fld;
        string path2 = root + fld1;

        string fileName = @"E:\AppLogs\nfpLive\tracer\" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
        FileInfo fi = new FileInfo(fileName);
        //string mgval = Get_Size_in_KB_MB_GB((ulong)fi.Length, 0);
        string mgval = "";
        try
        {
            mgval = Get_Size_in_KB_MB_GB((ulong)fi.Length, 0);
        }
        catch
        {
        }
        if (mgval.ToUpper().Contains("MB"))
        {
            mgval = mgval.Replace("MB", "");
            Int32 mgIntVal = Int32.Parse(mgval);

            if (mgIntVal > 1)
            {
                //rename the file to start afresh if the size is above 1mb
                try
                {
                    File.Move(@"E:\AppLogs\nfpLive\tracer\" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt", @"E:\AppLogs\nfpLive\tracer\" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss") + ".txt"); // Try to rename

                }
                catch (IOException ex1)
                {

                }
            }
        }

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
