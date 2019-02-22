using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Linq;
using SkietbaanBE.Models;
using System.Net.Mime;
using System.IO;

namespace SkietbaanBE
{
    public class EmailService
    {

        readonly string smtpAddress = "smtp.gmail.com";
        readonly int portNumber = 587;
        readonly bool enableSSL = true;
        readonly string emailFromAddress = "bathande704@gmail.com"; //Sender Email Address
        readonly string password = "22842633"; //Sender Password

        public bool SendMail(string email)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential("bathande704@gmail.com", "22842633");
                MailMessage msg = new MailMessage();
                msg.To.Add(email);
                msg.From = new MailAddress(emailFromAddress);
                msg.Subject = "test";
                msg.Body = "awe";
                client.Send(msg);
               

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

    }

}
