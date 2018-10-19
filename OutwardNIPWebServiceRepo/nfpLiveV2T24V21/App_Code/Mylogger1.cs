using System;
using System.Collections.Generic;
using System.Web;
using log4net;

/// <summary>
/// Summary description for Mylogger1
/// </summary>
public class Mylogger1
{
    public static ILog Log;

    static Mylogger1()
    {
        Log = LogManager.GetLogger("LogFileAppender");
    }
    public static void Error(object msg)
    {
        Log.Error(msg);
    }

    public static void Error(object msg, Exception ex)
    {
        Log.Error(msg, ex);
    }

    public static void Info(object msg)
    {
        Log.Info(msg);
    }
}