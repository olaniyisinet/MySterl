using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SendOtpMail
/// </summary>
public class SendOtpMail
{
       private readonly MailSenderService.ServiceSoapClient _mailSenderService = new MailSenderService.ServiceSoapClient();
   public string SendOtpViaMail(string destinationEmail,string sourceEmail,string otp,string subject)
    {
        ewsTest.ServiceSoapClient ews = new ewsTest.ServiceSoapClient();
        var resp2 = ews.SendMail(destinationEmail, sourceEmail, "<p>Your One Time Password is <strong>" + otp + "</strong></p>", subject);
        //var resp = _mailSenderService.SendMail(destinationEmail, sourceEmail, "<p>Your One Time Password is <strong>"+otp+"</strong></p>", subject);
        return resp2;
    }
    public string SendOtpViaMailNew(string destinationEmail, string sourceEmail,string otp, string subject)
    {
        ewsTest.ServiceSoapClient ews = new ewsTest.ServiceSoapClient();
        var resp2 = ews.SendMail(destinationEmail, sourceEmail, otp, subject);
        //var resp = _mailSenderService.SendMail(destinationEmail, sourceEmail, "<p>Your One Time Password is <strong>"+otp+"</strong></p>", subject);
        return resp2;
    }
}