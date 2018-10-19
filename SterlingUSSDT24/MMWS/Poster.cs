using System;
using System.Configuration;
using System.Net;

namespace MMWS
{
    public class Poster
    {
        private Request q;
        public string  PostData { get; set; }

        public Poster(Request qr)
        {
            q = qr;
            PostData = "";
            AddParam("channelID", q.ChannelID);
            AddParam("service", q.Service);
            AddParam("sourceMDN", q.Mobile);
            if (q.Name != "Activation")
            {
                AddParam("sourcePIN", q.PIN);
            }
            AddParam("txnName", q.Name);
            AddParam("institutionID", q.InstID);
            if (q.Name == "Login")
            {
                AddParam("authenticationString", q.AuthKey);
            }
            else
            {
                AddParam("authenticationKey", q.AuthKey);
            }
        }

        public string Send()
        {
            string responseString = SendMessage();
            return XMLTool.GetNodeData(responseString, "message");
        }


        public string SendWithAuth()
        {
            if(q.AuthKey.StartsWith("ERROR:"))
            {
                return q.AuthKey;
            }
            string responseString = SendMessage();
            return XMLTool.GetNodeData(responseString, "message");

        }
        public string Login()
        {
            string responseString = SendMessage();
            new ErrorLog("Response Text: " + responseString);
            string messageCode = XMLTool.GetNodeAttribute(responseString, "message", "code");
            string message = XMLTool.GetNodeData(responseString, "message");
            if(messageCode != "630") return message;
            string rmsg = XMLTool.GetNodeData(responseString, "key");
            return rmsg;
        }

        public void AddParam(string key, object val)
        {
            if(PostData != "")
            {
                PostData += "&";
            }
            else
            {
                PostData += "?";
            }
            PostData += key + "=" + val;
        }

         

        private string SendMessage()
        {
            WebRequest.DefaultWebProxy = new WebProxy("10.0.0.120", 80)
            {
                Credentials = new NetworkCredential("ibsservice", "Sterling123")
            };

            string uri = ConfigurationManager.AppSettings["mfinoAPI"];
            new ErrorLog("Request: " + uri + PostData);
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            string txt = uri + PostData;
            string responseString;
            try
            {
                using (WebClient client = new WebClient())
                {
                    responseString = client.DownloadString(txt);
                }
            }
            catch(Exception ex)
            {
                new ErrorLog("Response Text: " + ex.Message);
                responseString = "We sorry.. an error occurred, pls contact admin";
            }
            return responseString;
        }

/*
        private void SendMessageOld()
        {
            //string uri = "https://10.0.0.245:8443/webapi/sdynamic";
            //new ErrorLog("Request: " + uri + PostData);
            //ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            //HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(uri);
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //byte[] data = encoding.GetBytes(PostData);
            //httpWReq.Method = "POST";
            //httpWReq.ContentType = "application/x-www-form-urlencoded";
            //httpWReq.ContentLength = data.Length;
            //using (Stream stream = httpWReq.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}
            //HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
            //string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //return responseString;
        }
*/

        public string SendWithAuthRaw()
        {
            if (q.AuthKey.StartsWith("ERROR:"))
            {
                return q.AuthKey;
            }
            string responseString = SendMessage(); 
            return responseString;
        }
    }
}
