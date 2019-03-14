using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using System.Net;
using System.Net.Mail;

using System.IO;

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
                MemoryStream memoryStream = new MemoryStream();

                streamReader = new StreamReader(Directory.GetCurrentDirectory().ToString() + "/Controllers/Documents/Certificate.html");

                if(streamReader != null)
                {
                    string content = streamReader.ReadToEnd();

                    content.ToString();

                    streamReader.Close();

                    var content1 = content.Replace("Nadeem", Member.Username)
                        .Replace("Front End Development", "Letter Of Status");

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit; ;

                    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(content1);

                
                    doc.Save(memoryStream);

                    byte[] bytes = memoryStream.ToArray();

                    memoryStream.Close();

                    sendMail.SendEmail(Member.Email, "Letter Of Status", new Attachment(new MemoryStream(bytes),"LOS.pdf"));

                    doc.Close();

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

            MemoryStream memoryStream = new MemoryStream();


            StreamReader streamReader;

            streamReader = new StreamReader("./Controllers/Documents/Certificate.html");

            string content = streamReader.ReadToEnd();

            content.ToString();

            streamReader.Close();

            var content1 = content.Replace("Name", Member.Username)
                .Replace("Type", "Letter Of Good Standing")
                .Replace("Date", "December 2019"); 

            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
  
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(content1);

            doc.Save(memoryStream);

            byte[] bytes = memoryStream.ToArray();

            memoryStream.Close();

            

            sendMail.SendEmail(Member.Email, "Letter Of Good Standing", new Attachment(new MemoryStream(bytes), "LOGS.pdf"));

            doc.Close();
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
