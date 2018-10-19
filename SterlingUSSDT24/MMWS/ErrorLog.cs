using System;
using System.Configuration;
using System.IO;

namespace MMWS
{
    public class ErrorLog
    {
        public ErrorLog(Exception ex)
        {
            new ErrorLog(ex.ToString());

        }
        public ErrorLog(string ex)
        {
            string pth = ConfigurationManager.AppSettings["errorlog"];
            string err = ex;
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
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
    }

}
