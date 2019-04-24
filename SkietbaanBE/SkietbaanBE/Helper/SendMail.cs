using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace SkietbaanBE.Helper
{

    public class SendMail
    {
        string smtpAddress = "smtp.gmail.com";
        int portNumber = 587;
        bool enableSSL = true;
        string emailFromAddress = Environment.GetEnvironmentVariable("emailFromAddress"); //Sender Email Address  
        string password = Environment.GetEnvironmentVariable("emailpassword"); //Sender Password   

        public bool SendEmail(string To, string Subject, Attachment file)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    MailAssignment(mail, emailFromAddress, To, Subject, "<h>view attachment</h1>");
                    mail.Attachments.Add(file);
                    SmtpSend(mail);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool SendPasswordEmail(string To, string Subject, string token)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {

                    string url = Environment.GetEnvironmentVariable("resetUrl");
                    MailAssignment(mail, emailFromAddress, To, Subject, $"<div style='text-align: center;'><img src='https://firebasestorage.googleapis.com/v0/b/skietbaan-351ab.appspot.com/o/password%20reset.png?alt=media&token=b989fdf4-b19b-4e64-82aa-cf2ba41cc7ef'></img><div><h2>Click here to reset your password: <a href='{url}{token}'>Reset Password</a></h2><div></div>");
                    SmtpSend(mail);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public void MailAssignment(MailMessage mailMessage, string From, string To, string Subject, string Body)
        {
            mailMessage.From = new MailAddress(From);
            mailMessage.To.Add(To);
            mailMessage.Subject = Subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = Body;
        }

        public void SmtpSend(MailMessage mail)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);
                smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                smtp.EnableSsl = enableSSL;
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                e.ToString();
            }
           
        }
    }
}
