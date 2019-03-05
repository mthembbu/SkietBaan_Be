using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using System.Net;
using System.Net.Mail;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.html.simpleparser;
using Microsoft.AspNetCore.Http;
using IronPdf;
using System.Xml.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using SelectPdf;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[Action]")]
    public class DocumentsController : Controller
    {
        private ModelsContext _context;
        private SendMail sendMail = new SendMail();

        public DocumentsController(ModelsContext db)
        {
            _context = db;
        }  


        [HttpGet]
        [Route("{Token}")]
        public string SendLOS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if(Member != null)
            {
                StreamReader streamReader;

                streamReader = new StreamReader(Directory.GetCurrentDirectory().ToString() +"/Controllers/Documents/edit.htm");

                if(streamReader != null)
                {
                    string content = streamReader.ReadToEnd();

                    

                    content.ToString();

                    streamReader.Close();

                    var content1 = content.Replace("Nadeem", Member.Username)
                        .Replace("Front End Development", "Letter Of Status");

                    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(content1);
                    doc.Save(Directory.GetCurrentDirectory().ToString()+"/Controllers/Documents/LOSWorking.pdf");
                    doc.Close();
                    //sendMail.SendEmail(Member.Email, "Letter Of Status", "./Controllers/Documents/LOS.pdf");

                    return ("document saved");

                }

                return (Directory.GetCurrentDirectory().ToString());


            }

            return (Directory.GetCurrentDirectory().ToString());

           
        }

        [HttpGet]
        [Route("{Token}")]
        public void SendLOGS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            StreamReader streamReader;

            streamReader = new StreamReader("./Controllers/Documents/edit.htm");

            string content = streamReader.ReadToEnd();

            content.ToString();

            streamReader.Close();

            var content1 = content.Replace("Nadeem", Member.Username)
                .Replace("Front End Development", "Letter Of Good Standing"); ;

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(content1);
            doc.Save("./Controllers/Documents/LOGS.pdf");
            doc.Close();

            sendMail.SendEmail(Member.Email, "Letter Of Good Standing","./Controllers/Documents/LOGS.pdf");
        }

        [HttpGet]
        [Route("{Token}")]
        public string UserLOGS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if (Member.MemberID != null)
            {
                var comp = _context.UserCompetitionTotalScores.FirstOrDefault(x => x.User.MemberID == Member.MemberID);

                if(comp != null)
                {
                    return ("Document");
                }
                return ("No Document");
            }
            else
            {
                return ("No Document");
            }
            
        }

        [HttpGet]
        [Route("{Token}")]
        public string UserLOS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if (Member.MemberID != null)
            {
                var comp = _context.UserCompetitionTotalScores.Count(x => x.User.MemberID == Member.MemberID);

                if (comp > 6)
                {
                    return ("Document");
                }
                return ("No Document");
            }            
            return ("No Document");
        }
    }
}
