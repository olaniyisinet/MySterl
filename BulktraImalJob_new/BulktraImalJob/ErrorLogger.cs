using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BulktraImalJob
{
    class ErrorLogger
    {
        public ErrorLogger(Exception ex)
        {
            new ErrorLogger(ex.ToString());
        }
        public ErrorLogger(string ex)
        {
            string pth = ConfigurationManager.AppSettings["errorlog"];
            string pth2 = ConfigurationManager.AppSettings["errorlog"];
            if (!Directory.Exists(pth))
            {
                Directory.CreateDirectory(pth);
            }

            string err = ex;
            DateTime dt = DateTime.Now;
            string fld = dt.ToString("yyyy") + "_" + dt.ToString("MM") + "_";
            pth += fld + dt.ToString("dd") + ".txt";
            pth2 += fld + DateTime.Now.Ticks.ToString() + ".txt";
            try
            {

                using (StreamWriter sw = File.AppendText(pth))
                {
                    sw.WriteLine(dt.ToString() + " : " + err);
                    sw.WriteLine(" ");
                    sw.Close();
                    //sw.Dispose();
                }

            }
            catch (Exception)
            {
                using (StreamWriter sw = File.AppendText(pth2))
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
   
