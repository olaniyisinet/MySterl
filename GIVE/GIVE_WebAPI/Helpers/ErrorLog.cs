using System;
using System.Configuration;
using System.IO;


namespace GIVE_WebAPI.Helpers
{
	public class ErrorLog
	{

	public static string LOG_FILE_NAME = ConfigurationManager.AppSettings["logFile"].ToString(); 

    public static string MSG_LOG_FILE_NAME = ConfigurationManager.AppSettings["msgFile"].ToString();

		public ErrorLog(string message, string stactrace)
		{
			string path = @"Error.txt";  // file path
			using (StreamWriter sw = new StreamWriter(path, true))
			{ // If file exists, text will be appended ; otherwise a new file will be created
				sw.Write(string.Format("Message: {0}<br />{1}StackTrace :{2}{1}Date :{3}{1}-----------------------------------------------------------------------------{1}",
					message, Environment.NewLine, stactrace, DateTime.Now.ToString()));
			}
		}

		public static void LogException(Exception ex)
		{
			if (ex != null)
			{
				string targetSite = ""; // ex.TargetSite.Name
				string message = ""; // ex.Message
				string stackTrace = ""; // ex.StackTrace
				string source = ""; // ex.Source
				string trace = ""; // ex.StackTrace

				try
				{
					message = ex.Message;
					targetSite = ex.TargetSite.Name;
					stackTrace = ex.StackTrace;
					source = ex.Source;
					trace = ex.StackTrace;
				}
				catch (Exception exc)
				{
				}

				Exception innerException = ex.InnerException;

				File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LOG_FILE_NAME + GetLogFileNameForToday()), "[Date]>> " + DateTime.Now +"\r\n" + "[Method Name]>> " + targetSite +"\r\n" + "[Message]>> " + message +"\r\n" + "[Source]>> " + source +"\r\n" + "[Stack Trace]>> " + trace +"\r\n");

				if (innerException != null)
					LogException(innerException);
				else
					File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LOG_FILE_NAME + GetLogFileNameForToday()),"\r\n" +"\r\n");
			}
			else
				File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LOG_FILE_NAME + GetLogFileNameForToday()), "[Date]>> " + DateTime.Today +"\r\n" + "[Method Name]>> " + "Exception is NOTHING" +"\r\n" + "[Message]>> " + "Exception is NOTHING" +"\r\n" + "[Source]>> " + "Exception is NOTHING" +"\r\n" + "[Stack Trace]>> " + "Exception is NOTHING" +"\r\n");
		}


		private static string GetLogFileNameForToday()
		{
			string filename = string.Empty;
			string ext = ".txt";
			string year = DateTime.Now.Year.ToString();
			string month = DateTime.Now.Month.ToString();
			string day = DateTime.Now.Day.ToString();

			filename = year + "_" + month + "_" + day + ext;
			return filename;
		}


	}
}