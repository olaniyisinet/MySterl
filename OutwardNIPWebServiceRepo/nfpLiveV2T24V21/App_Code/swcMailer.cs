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

public class swcMailer
{
    public string mailFrom = "Neft@Sterlingbankng.com";    
    public string mailHost = "10.0.0.88";
    public string mailBody = "";
    public string mailSubject = "";
    public MailMessage message = new MailMessage();

	public swcMailer()
	{
        MailAddress address = new MailAddress(mailFrom);
        message.From = address;
    }
    public swcMailer(string email)
    {
        MailAddress address = new MailAddress(email);
        message.From = address;
    }

    public void addTo(string email)
    {
        MailAddress address = new MailAddress(email);
        message.To.Add(address);
    }

    public void addCC(string email)
    {
        MailAddress address = new MailAddress(email);
        message.CC.Add(address);
    }

    public void addBCC(string email)
    {
        MailAddress address = new MailAddress(email);
        message.Bcc.Add(address);
    }
    public void sendTheMail()
    {
        SmtpClient smtpClient = new SmtpClient();
        try
        {
            message.Subject = mailSubject;
            message.Body = mailBody;
            message.IsBodyHtml = true;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            smtpClient.UseDefaultCredentials = false;
            //smtpClient.Credentials = nc;
            smtpClient.Host = mailHost;
            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
    }
}
