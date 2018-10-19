using System;
using System.Configuration;
using System.IO;


public class ErrorLog
{
    public ErrorLog(Exception ex)
    {
        string pth = ConfigurationManager.AppSettings["errorlog"].ToString();
        string err = ex.ToString();
        //string err = ex.Message;
        DateTime dt = DateTime.Now;
        string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
        pth += fld + dt.ToString("dd") + ".txt";
        Logger.Log(err, pth);

    }
    public ErrorLog(string ex)
    {
        string pth = ConfigurationManager.AppSettings["errorlog"].ToString();
        string err = ex;
        DateTime dt = DateTime.Now;
        string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
        pth += fld + dt.ToString("dd") + ".txt";
        Logger.Log(err, pth);
    }
}

public class Tracer
{
    public Tracer(Exception ex)
    {
        string pth = ConfigurationManager.AppSettings["tracer"].ToString();
        string err = ex.ToString();
        //string err = ex.Message;
        DateTime dt = DateTime.Now;
        string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
        pth += fld + dt.ToString("dd") + ".txt";
        Logger.Log(err, pth);

    }
    public Tracer(string ex)
    {
        string pth = ConfigurationManager.AppSettings["tracer"].ToString();
        string err = ex;
        DateTime dt = DateTime.Now;
        string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
        pth += fld + dt.ToString("dd") + ".txt";
        Logger.Log(err, pth);
    }
}


public static class Logger
{
    public static void Log(string err, string pth)
    {
        DateTime dt = DateTime.Now;
        try
        {
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
        catch
        {
            Logger.Log(err, pth + dt.ToString("HHmmssffffff") + ".txt");
        }
    }
}