using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using OtpService.smsSender;
using OtpService.MailSenderService;
using OtpService.ewsTest;
using OtpService.EACBS;
/// <summary>
/// Summary description for SMS
/// </summary>
public class SMS
{
    public SMS()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string sendSMSOTP(string gsm, string msg)
    {
        string TheMsg = "";
        bool status = false;
        string resp = "";

        try
        {

            Gadget gd = new Gadget();
            StringBuilder mess = new StringBuilder();
            TheMsg = "Use " + msg + " as your One Time password. Expires within 2 minutes ";
            mess.Append(msg);

            msgBuilder m = new msgBuilder();
            m.mobile = Utility.mobile234(gsm);
            m.message = mess.ToString();
            string txt = m.buildRequest();

            string t = gd.enkrypt(txt);
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            OtpService.smsSender.smsSenderSoapClient s = new OtpService.smsSender.smsSenderSoapClient();
          //  OtpService.smsSender.smsSender s = new OtpService.smsSender.smsSender();
            WebProxy p = new WebProxy("10.0.0.120", 80);
          //  s.Proxy = p; //commented out ont 10:27pm july 27th
      

            // resp = s.sendMessage(t);
            resp = s.sendMessageLive(t, "Kiosk");

            status = true;

        }

        catch (Exception ex)
        {

            new ErrorLog("Message not sent +\n" + ex.ToString());

        }

        return resp;

    }
    public string sendSMS(string gsm, string msg)
    {

        bool status = false;
        string resp = "";

        try
        {

            Gadget gd = new Gadget();
            StringBuilder mess = new StringBuilder();

            mess.Append(msg);

            msgBuilder m = new msgBuilder();
            m.mobile = Utility.mobile234(gsm);
            m.message = mess.ToString();
            string txt = m.buildRequest();

            string t = gd.enkrypt(txt);
            NetworkCredential nc = new NetworkCredential("spservice", "Kinder$$098", "sterlingbank");
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            OtpService.smsSender.smsSenderSoapClient s = new OtpService.smsSender.smsSenderSoapClient();

            //smsSender.smsSender s = new OtpService.smsSender.smsSender();
            WebProxy p = new WebProxy("10.0.0.120", 80);
            //s.Proxy = p;
            //s.Proxy.Credentials = nc;
            s.ClientCredentials.UserName.UserName = "spservice";
            s.ClientCredentials.UserName.Password = "Kinder$$098";
            // resp = s.sendMessage(t);
            resp = s.sendMessageLive(t, "NEWIBS");

            status = true;

        }

        catch (Exception ex)
        {

            new ErrorLog("Message not sent +\n" + ex.ToString());

        }

        return resp;

    }


    public string sendSMS(string gsm, string msg, int appId)
    {

        bool status = false;
        string resp = "";

        try
        {

            Gadget gd = new Gadget();
            StringBuilder mess = new StringBuilder();

            mess.Append(msg);

            msgBuilder m = new msgBuilder();
            m.mobile = Utility.mobile234(gsm);
            m.message = mess.ToString();
            string txt = m.buildRequest();

            string t = gd.enkrypt(txt);
            NetworkCredential nc = new NetworkCredential("spservice", "Kinder$$098", "sterlingbank");
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            OtpService.smsSender.smsSenderSoapClient s = new OtpService.smsSender.smsSenderSoapClient();

           // smsSender.smsSender s = new smsSender.smsSender();
            WebProxy p = new WebProxy("10.0.0.120", 80);
            //s.Proxy = p;
            //s.Proxy.Credentials = nc;
            p.Credentials = nc;
            // resp = s.sendMessage(t);
            resp = s.sendMessageLive(t, "NEWIBS" + appId);

            status = true;

        }

        catch (Exception ex)
        {

            new ErrorLog("Message not sent +\n" + ex.ToString());

        }

        return resp;

    }

    public string sendSenderSMS(string gsmMob, string amt, string dToken)
    {

        bool status = false;
        string resp = "";

        try
        {

            Gadget gd = new Gadget();
            StringBuilder mess = new StringBuilder();

            mess.Append("Dear Cutomer,");
            mess.Append("\n");
            mess.Append("Your transaction for amount: " + amt);
            mess.Append("\n");
            mess.Append("is been processed.");
            mess.Append("\n");
            mess.Append("Token value: " + dToken);
            mess.Append("\n");
            mess.Append("Customer Care: 01-4484481-5");
            mess.Append("\n");
            mess.Append("Date: " + DateTime.Now.ToString("dd-MMM-yy hh:mm"));


            msgBuilder m = new msgBuilder();
            m.mobile = Utility.mobile234(gsmMob);
            m.message = mess.ToString();
            string txt = m.buildRequest();

            string t = gd.enkrypt(txt);
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            //  s.Endpoint = p;
            OtpService.smsSender.smsSenderSoapClient s = new OtpService.smsSender.smsSenderSoapClient();

           // smsSender.smsSender s = new smsSender.smsSender();
            WebProxy p = new WebProxy("10.0.0.120", 80);
           // s.Proxy = p;
            // resp = s.sendMessage(t);
            resp = s.sendMessageLive(t, "NIPOTC");

            //Mailer mail = new Mailer();

            status = true;




        }

        catch (Exception ex)
        {

            new ErrorLog("Message not sent +\n" + ex.ToString());

        }

        return resp;

    }

    public string sendSenderSMSNoToken(string gsmMob, string amt)
    {

        bool status = false;
        string resp = "";

        try
        {

            Gadget gd = new Gadget();
            StringBuilder mess = new StringBuilder();

            mess.Append("Dear Cutomer,");
            mess.Append("\n");
            mess.Append("Your transaction for amount: " + amt);
            mess.Append("\n");
            mess.Append("is been processed.");
            mess.Append("\n");
            mess.Append("Customer Care: 01-4484481-5");
            mess.Append("\n");
            mess.Append("Date: " + DateTime.Now.ToString("dd-MMM-yy hh:mm"));


            msgBuilder m = new msgBuilder();
            m.mobile = Utility.mobile234(gsmMob);
            m.message = mess.ToString();
            string txt = m.buildRequest();

            string t = gd.enkrypt(txt);
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
           // smsSender.smsSender s = new smsSender.smsSender();
            OtpService.smsSender.smsSenderSoapClient s = new OtpService.smsSender.smsSenderSoapClient();

            WebProxy p = new WebProxy("10.0.0.120", 80);
           // s.Proxy = p;
            // resp = s.sendMessage(t);
            resp = s.sendMessageLive(t, "NIPOTC");

            //Mailer mail = new Mailer();

            status = true;




        }

        catch (Exception ex)
        {

            new ErrorLog("Message not sent +\n" + ex.ToString());

        }

        return resp;

    }
}