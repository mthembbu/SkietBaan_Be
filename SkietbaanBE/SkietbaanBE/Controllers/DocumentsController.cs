using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using System.Net;
using System.Net.Mail;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[Action]")]
    public class DocumentsController : Controller
    {
        private ModelsContext _context;
        private SendMail sendEmail = new SendMail();

        public DocumentsController(ModelsContext db)
        {
            _context = db;
        }

        [Route("{mail}/{url}")]
        public void Email(string mail,string url)
        {
            var ismember = _context.Users.FirstOrDefault(x => x.Email == mail);
            var stream = new WebClient().OpenRead(url);
            Attachment attachement = new Attachment(stream, "");
            if (ismember != null && attachement != null)
            {
                sendEmail.SendEmail(mail, "Congratulations", attachement);
            }                           
        }
    }
}
