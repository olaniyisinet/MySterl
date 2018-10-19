using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;

public class Mailer
{
    public string mailFrom = ConfigurationManager.AppSettings["mailsender"].ToString();
    public string mailHost = ConfigurationManager.AppSettings["mailserver"].ToString();
    public string mailBody = "";
    public string mailSubject = "";
    public MailMessage message = new MailMessage();

    public Mailer()
    {
        MailAddress address = new MailAddress(mailFrom);
        message.From = address;
    }
    public Mailer(string email)
    {
        MailAddress address = new MailAddress(email);
        message.From = address;
    }

    public void addTo(string email)
    {
        //test if only one email
        string s = email;
        s = s.Replace(" ", "");
        s = s.Replace(",", " ");
        s = s.Replace(";", " ");
        string[] words = s.Split(' ');
        Gadget g = new Gadget();
        foreach (string word in words)
        {
            if (g.checkEmail(word))
            {
                MailAddress address = new MailAddress(word);
                message.To.Add(address);
            }
        }
    }

    public void addCC(string email)
    {
        string s = email;
        s = s.Replace(" ", "");
        s = s.Replace(",", " ");
        s = s.Replace(";", " ");
        string[] words = s.Split(' ');
        Gadget g = new Gadget();
        foreach (string word in words)
        {
            if (g.checkEmail(word))
            {
                MailAddress address = new MailAddress(word);
                message.To.Add(address);
            }
        }
    }

    public void addBCC(string email)
    {
        string s = email;
        s = s.Replace(" ", "");
        s = s.Replace(",", " ");
        s = s.Replace(";", " ");
        string[] words = s.Split(' ');
        Gadget g = new Gadget();
        foreach (string word in words)
        {
            if (g.checkEmail(word))
            {
                MailAddress address = new MailAddress(word);
                message.To.Add(address);
            }
        }
    }

    public void sendTheMail(string admin)
    {
        SmtpClient smtpClient = new SmtpClient();
        NetworkCredential nc = new NetworkCredential(@"sterlingbank\ebusiness", "ebusiness");
        try
        {
            message.Subject = mailSubject;
            message.Body += mailBody;
            message.IsBodyHtml = true;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = nc;
            smtpClient.Host = mailHost;
            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        finally
        {
        }

    }
    public void sendTheMail()
    {
        SmtpClient smtpClient = new SmtpClient();
        NetworkCredential nc = new NetworkCredential(@"sterlingbank\ebusiness", "ebusiness");
        try
        {
            message.Subject = mailSubject;
            message.Body = "<table width='800' border='0' cellspacing='0' cellpadding='2' style='font-family:Verdana;'>";
            message.Body += "<tr><td>&nbsp;</td>";
            message.Body += "<td><div align='right'><img src='http://www.sterlingbankng.com/images/str_logo.png'></div></td>";
            message.Body += "</tr><tr>";
            message.Body += "<td colspan='2'>&nbsp;</td>";
            message.Body += "</tr><tr>";
            message.Body += "<td colspan='2'>";
            message.Body += mailBody;
            message.Body += "</td></tr><tr></table>";
            message.IsBodyHtml = true;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = nc;
            smtpClient.Host = mailHost;
            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        finally
        {
        }

    }
}
