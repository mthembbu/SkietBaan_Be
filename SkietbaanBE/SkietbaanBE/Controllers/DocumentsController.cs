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

        private static int numberShots = 0;
        private static List<int> GroupList=new List<int>(); 



        public DocumentsController(ModelsContext db)
        {
            _context = db;
        }  

        [HttpPost]

        public void changeShots(int num)
        {
            numberShots = num;
        }

        [HttpPost]
        public void getGroup([FromBody] List<Models.Competition> group)
        {
            GroupList.Clear();
            for(int i = 0; i < group.Count; i++)
            {
                GroupList.Add(group.ElementAt(i).Id);
            }
        } 
        
        [HttpPost] 
        [Route("{Token}")]
        public string SendLOS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if(Member != null)
            {
                StreamReader streamReader;
                MemoryStream memoryStream = new MemoryStream();


                try
                {
                    streamReader = new StreamReader(Directory.GetCurrentDirectory().ToString() + @"\Controllers\Documents\Certificate.html");


                    if (streamReader != null)
                    {
                        string content = streamReader.ReadToEnd();

                        content.ToString();

                        streamReader.Close();

                        var content1 = content.Replace("Name", Member.Username)
                               .Replace("Type", "Letter Of Status")
                               .Replace("Date", "December 2019");

                        HtmlToPdf converter = new HtmlToPdf();
                        converter.Options.PdfPageSize = PdfPageSize.A4;
                        converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                        converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                        converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit; ;

                        SelectPdf.PdfDocument doc = converter.ConvertHtmlString(content1);


                        doc.Save(memoryStream);

                        byte[] bytes = memoryStream.ToArray();

                        memoryStream.Close();

                        sendMail.SendEmail(Member.Email, "Letter of Status", new Attachment(new MemoryStream(bytes), "LOS.pdf"));

                        doc.Close();

                        return ("yes");

                    }

                }
                catch (System.IO.DirectoryNotFoundException)
                {

                }    
                return (Directory.GetCurrentDirectory().ToString() + "\\Controllers\\Documents\\Certificate.html");

            }

            return (Directory.GetCurrentDirectory().ToString());

           
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

                         var content1 = content.Replace("Name", Member.Username)
                            .Replace("Type", "Letter Of Good Standing")
                            .Replace("Date", "December 2019");
                        
                        SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                       converter.Options.PdfPageSize = PdfPageSize.A4;
                        converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                        converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                        converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;

                        
                        SelectPdf.PdfDocument doc = converter.ConvertHtmlString(content1);
                        /*
                        if (doc != null)
                        {
                            doc.Save(memoryStream);

                            byte[] bytes = memoryStream.ToArray();

                            memoryStream.Close();

                            if (bytes != null)
                            {
                                sendMail.SendEmail(Member.Email, "Letter Of Good Standing", new Attachment(new MemoryStream(bytes), "LOGS.pdf"));

                                doc.Close();

                                return (content);

                            }

                            return ("array fails");

                           
                        }*/

                        return (content1);

                    }

                    return (content);

                }
                return (Directory.GetCurrentDirectory().ToString()+ streamReader.ToString());
            }
            return (Directory.GetCurrentDirectory().ToString());
        }



        [HttpGet]
        [Route("{Token}")]
        public string UserLOGS(string Token)
        {
            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if (Member != null){

            
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
            return ("No Document");

        }

        [HttpGet]
        [Route("{Token}")]
        public string UserLOS(string Token)
        {

            var Member = _context.Users.FirstOrDefault(x => x.Token == Token);

            if (Member != null)
            {
                if (Member.MemberID != null)

                {
                    int counts = 0;
                    foreach (var item in GroupList)
                    {
                        var comp = from score in _context.Scores
                                   where (score.Competition.Id == item && score.User.Id == 5)
                                   select new
                                   {
                                       score.UserScore
                                   };
                        counts += comp.ToList().Count;
                    }

                    if (counts > numberShots)
                    {
                        return ("Document");
                    }
                    return ("No Document");
                }
            }
            return ("No Document");

        }


    }
}
