using System;
using System.Net;
using System.IO;
using System.Xml;

public partial class ussdDefault : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        txtSessionID.Text = DateTime.Now.ToString("yyMMddhhmmssfff");

    }

    protected void sendMessage(string mobile, string session, string mtype, string msg)
    {
        string ntwrk = txtNetwork.SelectedValue.Trim();
        string bdat = "<?xml version=\"1.0\" ?><ussd>" +
            "<msisdn>" + mobile + "</msisdn>" +
            "<sessionid>" + session + "</sessionid>" +
            "<type>" + mtype + "</type>" +
            "<msg>" + msg + "</msg>" +
            "<network>" + ntwrk + "</network>" +
            "</ussd>";

        string url = "http://localhost:8099/v1_0.aspx"; 

        WebRequest req; 
        Stream reqStream;
        try
        {
            req = WebRequest.Create(url);
            req.Method = "Post";
            req.ContentType = "text/xml";
            req.Credentials = CredentialCache.DefaultNetworkCredentials;
            byte[] bdata = System.Text.Encoding.ASCII.GetBytes(bdat);
            req.ContentLength = bdata.Length;
            reqStream = req.GetRequestStream();
            reqStream.Write(bdata, 0, bdata.Length);
            reqStream.Close();
        }
        catch
        {
            txtScreen.Text = "Connection error1";
            return;
        }

        WebResponse resp;
        StreamReader reader;
        string str;
        try
        {
            resp = req.GetResponse();
            reader = new StreamReader(resp.GetResponseStream());
            str = reader.ReadToEnd();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(str);
            string rmsg = xml.GetElementsByTagName("msg").Item(0).InnerText;
            txtScreen.Text = rmsg.Replace("%0A", "\n");

            lblCnt.Text = txtScreen.Text.Length.ToString();
            lblinput.Text = "";
        }
        catch (Exception ex)
        {
            txtScreen.Text = "Response error2";
            Response.Write(ex);
            return;
        }
    }

    protected void sendMessage2(string mobile, string session, string mtype, string msg)
    {
        string bdat = "<?xml version=\"1.0\" ?><ussd>" +
            "<msisdn>" + mobile + "</msisdn>" +
            "<sessionid>" + session + "</sessionid>" +
            "<type>" + mtype + "</type>" +
            "<msg>" + msg + "</msg>" +
            "</ussd>";

        //Response.Write(bdat);

        string url = "http://localhost/mmussd/v1_cardRequest.aspx";
        //string url = "https://epayments.sterlingbankng.com/echanneltest/ussd.aspx";

        HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
        WebResponse myResp = myReq.GetResponse();

        byte[] b = null;
        using (Stream stream = myResp.GetResponseStream())
        using (MemoryStream ms = new MemoryStream())
        {
            int count = 0;
            do
            {
                byte[] buf = new byte[1024];
                count = stream.Read(buf, 0, 1024);
                ms.Write(buf, 0, count);
            } while (stream.CanRead && count > 0);
            b = ms.ToArray();
        }

        string str = System.Text.Encoding.ASCII.GetString(b);
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(str);
        string rmsg = xml.GetElementsByTagName("msg").Item(0).InnerText;
        txtScreen.Text = rmsg.Replace("%0A", "\n");
        lblCnt.Text = txtScreen.Text.Length.ToString();
        lblinput.Text = "";
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        string mtype = "2";
        if (lblinput.Text.StartsWith("*822"))
        {
            mtype = "1";
        }

        sendMessage(txtMobile.Text, txtSessionID.Text, mtype, lblinput.Text);
    }

    protected void btnEnd_Click(object sender, EventArgs e)
    {
        sendMessage(txtMobile.Text, txtSessionID.Text, "6", lblinput.Text);
    }

    protected void btnDial_Click(object sender, EventArgs e)
    {
        lblinput.Text = "*822#";
    }
    protected void btnNewSession_Click(object sender, EventArgs e)
    {
        txtSessionID.Text = DateTime.Now.ToString("yyMMddhhmmssfff");
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        //txtScreen.Text = "";
    }
}