using DoubbleJob;
using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Doubble
{
    public class Gadget
    {

        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void sendExDoubble(string fName, string dayte, string mail)
        {
            string subject = "DOUBBLE";
            DoubbleJob.EWS.ServiceSoapClient mailsender = new DoubbleJob.EWS.ServiceSoapClient();
            string html = Directory.GetCurrentDirectory() + "\\DoubbleMail.html";
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

        public void sendExDoubbleLump(string fName, string mail)
        {
            DoubbleJob.EWS.ServiceSoapClient mailsender = new DoubbleJob.EWS.ServiceSoapClient();
            string subject = "DOUBBLE";
            string html = Directory.GetCurrentDirectory() + "\\DoubbleLumpMail.html";
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
            DoubbleJob.EWS.ServiceSoapClient mailsender = new DoubbleJob.EWS.ServiceSoapClient();
            string subject = "DOUBBLE";
            string html = Directory.GetCurrentDirectory() + "\\newDoubbleMail.html";
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
            DoubbleJob.EWS.ServiceSoapClient mailsender = new DoubbleJob.EWS.ServiceSoapClient();
            string html = Directory.GetCurrentDirectory() + "\\newDoubbleLumpMail.html";
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
        public void sendReminderMail(string fName, string amt, string dayte, string mail)
        {
            DoubbleJob.EWS.ServiceSoapClient mailsender = new DoubbleJob.EWS.ServiceSoapClient();
            string subject = "DOUBBLE REMINDER";
            //string html = System.IO.File.ReadAllText(@"C:\DevApps\logs\Doubble\DoubbleJob\templates\Reminder.html");
            string path = Directory.GetCurrentDirectory() + "\\Reminder.html";
            string html = System.IO.File.ReadAllText(path);
            string body = "";
            body = html.Replace("[fName]", fName).Replace("[date1]", dayte).Replace("[amount]",amt);
            try
            {
                mailsender.SendMail(mail, "e-business@sterlingbankng.com", body, subject);
            }
            catch (Exception e)
            {

            }
        }

        public string thAppend(string day)
        {
            if (day.EndsWith("1"))
            {
                return day + "st";
            }
            else if (day.EndsWith("2"))
            {
                return day + "nd";
            }
            else if (day.EndsWith("3"))
            {
                return day + "rd";
            }
            else
            {
                return day + "th";
            }
        }




        public bool sendmail(string toAddress, string cc, string subject, string body, string attachment2, string nam)
        {
            // toAddress = "Rabiu.Adetayo@Sterlingbankng.com"

            //DoubbleJob.EWS.ServiceSoapClient mailsender = new DoubbleJob.EWS.ServiceSoapClient();
            //byte[] bytes = System.IO.File.ReadAllBytes(attachment2);
            System.Net.Mail.SmtpClient mailHost = new System.Net.Mail.SmtpClient();
            //mailHost.Host = "10.0.20.77";
            mailHost.Host = "172.18.2.11";
            mailHost.Port = 25;
            System.Net.Mail.Attachment ma = new System.Net.Mail.Attachment(attachment2);

            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            System.Net.NetworkCredential nc = new System.Net.NetworkCredential("spservice", "Kinder$$098");
            mailHost.Credentials = nc;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(toAddress);
            mailMessage.From = new System.Net.Mail.MailAddress("ebusiness@sterlingbankng.com");
            // mailMessage.CC.Add(cc)
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.Attachments.Add(ma);


            try
            {
                mailHost.Send(mailMessage);
                //mailsender.SendMailWithAttachment(toAddress, "ebusiness@sterlingbankng.com", body, subject, null, bytes, nam);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public string BinaryToString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentNullException("binary");

            if ((binary.Length % 8) != 0)
                throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string section = binary.Substring(i, 8);
                int ascii = 0;
                try
                {
                    ascii = Convert.ToInt32(section, 2);
                }
                catch
                {
                    throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
                }
                builder.Append((char)ascii);
            }
            return builder.ToString();
        }
        public String Encrypt(String val)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                string sharedkeyval = ""; string sharedvectorval = "";
                sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "1100010100010100100000001101010101011100000001010000111000000010";
                sharedvectorval = BinaryToString(sharedvectorval);
                byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
                cs.Write(toEncrypt, 0, toEncrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                return "";
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public String Decrypt(String val)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                string sharedkeyval = ""; string sharedvectorval = "";

                sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "1100010100010100100000001101010101011100000001010000111000000010";
                sharedvectorval = BinaryToString(sharedvectorval);

                byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toDecrypt = Convert.FromBase64String(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);


                cs.Write(toDecrypt, 0, toDecrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                return "";
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        
             public void insertForSterlingLat(string FirstName, string LastName, string MobileNumber, string BVN, string Category, string PayInAccount, string BeneficiaryName, string BeneficiaryAccount, DateTime DateOfEntry, string PayInAmount, string ReferenceID, string DAOCode, string BeneficiaryType, string Email, string Cusnum, string FullName, string pwd, string arrangementId, int term, DateTime effDate, DateTime matDate, DateTime DateOfBirth, string verCode)
        {
            DoubbleDBEntities1 db = new DoubbleDBEntities1();
            var forSter = new ForSterling { ARRANGEMENT_ID = arrangementId, BeneficiaryAccount = BeneficiaryAccount, BeneficiaryName = BeneficiaryName, BeneficiaryType = BeneficiaryType, BVN = BVN, Category = Category, Cusnum = Cusnum, DAOCode = DAOCode, DateOfBirth = DateOfBirth, DateOfEntry = DateTime.Now, EffectiveDate = effDate, Email = Email, FirstName = FirstName, FullName = FullName, LastName = LastName, MaturityDate = matDate, MobileNumber = MobileNumber, Password = pwd, PayInAccount = PayInAccount, PayInAmount = PayInAmount, ReferenceID = ReferenceID, Term = term, ValCode = verCode };
            db.ForSterlings.Add(forSter);
            db.SaveChanges();
        }

        public void updateForNonSterling(string categ, string amt, string bName, string bAcc, string bAccType, string refId, string daoCode, string verCode, string arrangementId, int term, DateTime effDate, DateTime matDate, string pwd)
        {
            string sql = "update ForNonSterlings set Category=@categ,PayInAmount=@amt,BeneficiaryName=@bName,BeneficiaryAccount=@bAcc,BeneficiaryType=@bAccType,ReferenceID=@refId,DAOCode=@daoCode,ARRANGEMENT_ID=@arrangementId,Password=@pwd,Term=@term,EffectiveDate=@effDate,MaturityDate=@matDate where ValCode=@verCode";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@categ", categ);
            c.AddParam("@amt", amt);
            c.AddParam("@bName", bName);
            c.AddParam("@bAcc", bAcc);
            c.AddParam("@bAccType", bAccType);
            c.AddParam("@refId", refId);
            c.AddParam("@daoCode", daoCode);
            c.AddParam("@verCode", verCode);
            c.AddParam("@arrangementId", arrangementId);
            c.AddParam("@term", term);
            c.AddParam("@effDate", effDate);
            c.AddParam("@matDate", matDate);
            c.AddParam("@pwd", pwd);
            c.Select("rec");
            //Mylogger.Info("Record for BVN, " + refId + " was saved to ForNonSterlings table completely.");
        }

    }
}