using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace imalcore
{
    public static class Mylogger
    {

        public static ILog Log;

        static Mylogger()
        {
            Log = LogManager.GetLogger("LogFileAppender1");
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
}