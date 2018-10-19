using Doubble.MAIL.EWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doubble.MAIL
{
    public class DoubbleMAILService
    {
        private readonly ServiceSoapClient mailsender = new ServiceSoapClient();
        public void sendExDoubble(string fName, string dayte, string mail)
        {            
            string subject = "DOUBBLE";
            string html = System.Web.HttpContext.Current.Server.MapPath("~/templates/DoubbleMail.html");
            html = System.IO.File.ReadAllText(html);
            string body = "";
            body = html.Replace("[fName]", fName).Replace("[date1]", dayte);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);
            }
            catch (Exception)
            {
            }
        }

        public void sendLater(string fName, string dayte, string amount, string mail)
        {
            string subject = "DOUBBLE";
            string html = System.Web.HttpContext.Current.Server.MapPath("~/templates/Later.html");
            html = System.IO.File.ReadAllText(html);
            string body = "";
            body = html.Replace("[fName]", fName).Replace("[dayte]", dayte).Replace("[amount]", amount);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);
            }
            catch (Exception)
            {
            }
        }

        public void sendExDoubbleLump(string fName, string mail)
        {
            string subject = "DOUBBLE";
            string html = System.Web.HttpContext.Current.Server.MapPath("~/templates/DoubbleLumpMail.html");
            html = System.IO.File.ReadAllText(html);
            string body = "";
            body = html.Replace("[fName]", fName);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);                
            }
            catch (Exception e)
            {                
            }
        }

        public void sendDoubbleOpeningMail(string fName, string dayte, string mail, string password)
        {
            string subject = "DOUBBLE";
            string html = System.Web.HttpContext.Current.Server.MapPath("~/templates/newDoubbleMail.html");
            html = System.IO.File.ReadAllText(html);
            string body = "";
            body = html.Replace("[fName]", fName).Replace("[date1]", dayte).Replace("[password]", password);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);
            }
            catch (Exception e)
            {
            }
        }
        public void LsendDoubbleOpeningMail(string fName, string mail, string password)
        {
            string subject = "DOUBBLE";
            string html = System.Web.HttpContext.Current.Server.MapPath("~/templates/newDoubbleLumpMail.html");
            html = System.IO.File.ReadAllText(html);
            string body = "";
            body = html.Replace("[fName]", fName).Replace("[password]", password);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);
            }
            catch (Exception e)
            {
            }
        }

        public void sendAccountOpeningMail(string fName, string acct, string valCode, string mail)
        {
            string subject = "WELCOME";
            //string html = System.IO.File.ReadAllText(@"E:\Tolu\ToBeDeployed\Doubble\Doubble\templates\AccountOpening.html");
            string html = System.Web.HttpContext.Current.Server.MapPath("~/templates/AccountOpening.html");
            html = System.IO.File.ReadAllText(html);
            string body = "";
            body = html.Replace("[fName]", fName).Replace("[account]", acct).Replace("[valCode]", valCode);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);
            }
            catch (Exception e)
            {

            }
        }
    }
}
