using System;
using System.Collections.Generic;
using System.Web;
using log4net;

/// <summary>
/// Summary description for Mylogger
/// </summary>
//public static class Mylogger
//{
//    public static ILog Log { get; set; }

//    static Mylogger()
//    {
//        Log = LogManager.GetLogger(typeof(Mylogger));
//    }
//    public static void Error(object msg)
//    {
//        Log.Error(msg);
//    }

//    public static void Error(object msg, Exception ex)
//    {
//        Log.Error(msg, ex);
//    }

//    public static void Info(object msg)
//    {
//        Log.Info(msg);
//    }


//}
public static class Mylogger
{

    public static ILog Log;

    static Mylogger()
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