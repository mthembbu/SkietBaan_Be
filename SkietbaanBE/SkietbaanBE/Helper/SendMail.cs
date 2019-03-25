﻿using System;
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
        string emailFromAddress = "skietbaanskiet@gmail.com"; //Sender Email Address  
        string password = "skietbaan999"; //Sender Password   

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
                    string url = "https://skietbaan.retrotest.co.za/resetpassword/";
                    MailAssignment(mail, emailFromAddress, To, Subject, $"<a href='{url}{token}'>reset</a>");
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
            SmtpClient smtp = new SmtpClient(smtpAddress, portNumber);
            smtp.Credentials = new NetworkCredential(emailFromAddress, password);
            smtp.EnableSsl = enableSSL;
            smtp.Send(mail);
        }
    }
}
