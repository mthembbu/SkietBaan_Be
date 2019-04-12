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
using SelectPdf;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[Action]")]
    public class DocumentsController : Controller
    {
        private ModelsContext _context;
        private SendMail sendMail = new SendMail();
        private static int numberShots = 0;
        private static int competitionID = 0;

        public DocumentsController(ModelsContext db)
        {
            _context = db;
        }

        [HttpPost]
        public void changeShots(int shots)
        {
            numberShots = shots;
        }

        [HttpPost]
        public void getGroup(int ID)
        {
            if (competitionID == ID)
            {
                competitionID = 0;
                numberShots = 0;
            }
            else {
                competitionID = ID;
            }            
        }
        
        [HttpGet]
        public int StatusCompetition()
        {
            return competitionID;
        }

        [HttpGet]
        public int NumberOFShots()
        {
            return numberShots;
        }

        [HttpPost] 
        [Route("{Token}")]
        public string SendLOS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            try
            {
                if (Member != null)
                {
                    MemoryStream memoryStream = new MemoryStream();

                    StreamReader streamReader;

                    streamReader = new StreamReader(Directory.GetCurrentDirectory().ToString() + "\\Controllers\\Documents\\Certificate.html");

                    if (streamReader != null)
                    {
                        string content = streamReader.ReadToEnd();

                        if (content != null)
                        {
                            content.ToString();

                            streamReader.Close();
                            string content1;

                            if (Member.Name != null || Member.Surname != null)
                            {
                                content1 = content.Replace("Name", Member.Name + " " + Member.Surname)
                                .Replace("Type", "Letter Of Dedicated Status")
                                .Replace("Date", "December 2019");
                            }
                            else
                            {
                                content1 = content.Replace("Name", Member.Username)
                                .Replace("Type", "Letter Of Good Standing")
                                .Replace("Date", "December 2019");

                            }

                            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                            converter.Options.PdfPageSize = PdfPageSize.A4;
                            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                            converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;

                            PdfDocument doc = new PdfDocument();
                            doc = converter.ConvertHtmlString(content1);


                            if (doc != null)
                            {
                                doc.Save(memoryStream);

                                byte[] bytes = memoryStream.ToArray();

                                memoryStream.Close();

                                if (bytes != null)
                                {
                                    sendMail.SendEmail(Member.Email, "Letter Of Dedicated Status", new Attachment(new MemoryStream(bytes), "LOS.pdf"));

                                    doc.Close();

                                    return ("Document Sent");

                                }

                                return ("Document Not Sent");


                            }

                            return ("Document Not Sent");

                        }

                        return ("Document Not Sent");

                    }
                    return ("Document Not Sent");
                }
            }

            catch (Exception e)
            {
                return ("Document Not Sent");
            }            
            return ("Document Not Sent");
        }

        [HttpPost]
        [Route("{Token}")]
        public string SendLOGS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if(Member != null)
            {
                MemoryStream memoryStream = new MemoryStream();

                StreamReader streamReader;

                streamReader = new StreamReader(Directory.GetCurrentDirectory().ToString() + "\\Controllers\\Documents\\Certificate.html");

                if (streamReader != null)
                {
                    string content = streamReader.ReadToEnd();

                    if (content != null)
                    {
                        content.ToString();

                        streamReader.Close();
                        string content1;

                        if (Member.Name != null || Member.Surname !=null)
                        {
                            content1 = content.Replace("Name", Member.Name + " " + Member.Surname)
                            .Replace("Type", "Letter Of Good Standing")
                            .Replace("Date", "December 2019");
                        }
                        else
                        {
                            content1 = content.Replace("Name", Member.Username)
                            .Replace("Type", "Letter Of Good Standing")
                            .Replace("Date", "December 2019");

                        }

                         
                        
                        SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                       converter.Options.PdfPageSize = PdfPageSize.A4;
                        converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                        converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                        converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;

                        PdfDocument doc = new PdfDocument();
                        doc = converter.ConvertHtmlString(content1);

           
                        if (doc != null)
                        {
                            doc.Save(memoryStream);

                            byte[] bytes = memoryStream.ToArray();

                            memoryStream.Close();

                            if (bytes != null)
                            {
                                sendMail.SendEmail(Member.Email, "Letter Of Good Standing", new Attachment(new MemoryStream(bytes), "LOGS.pdf"));

                                doc.Close();

                                return ("Document Sent");

                            }

                            return ("Document Not Sent");

                           
                        }

                        return ("Document Not Sent");

                    }

                    return ("Document Not Sent");

                }
                return ("Document Not Sent");
            }
            return ("Document Not Sent");
        }



        [HttpGet]
        [Route("{Token}")]
        public string UserLOGS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if (Member != null){

            
                if (Member.MemberID != null)
                {
                    var comp = _context.Scores.FirstOrDefault(x => x.User.Id == Member.Id);

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
            return ("No Document");

        }

        [HttpGet]
        [Route("{Token}")]
        public string UserLOS(string Token)
        {

            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            try
            {
                int counts = 0;

                Competition compSelected = _context.Competitions.FirstOrDefault(x => x.Id == competitionID);
                
                if (compSelected != null)
                {
                    string compName = compSelected.Name;


                    if (Member != null)
                    {
                        if (Member.MemberID != null)

                        {


                            var comp = from score in _context.Scores
                                       where (score.Competition.Id == competitionID && score.User.Id == Member.Id)
                                       select new
                                       {
                                           score.UserScore
                                       };
                            counts += comp.ToList().Count;


                            if (counts > numberShots)
                            {
                                return ("Document");
                            }
                            return ("requires: " + numberShots.ToString() + " Shots, in " + compName + " competition");
                        }
                    }
                    return ("requires: " + numberShots.ToString() + " Shots, in " + compName + " competition");
                }
                return ("Admin has not set requirements for letter of dedicated status");

            }
            catch (Exception e)
            {
                return ("Admin has not set requirements for letter of dedicated status");
            }
            
            
        }
    }
}
