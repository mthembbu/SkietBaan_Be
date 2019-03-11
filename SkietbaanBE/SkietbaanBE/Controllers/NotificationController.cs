using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        private ModelsContext _context;
        private NotificationMessages _notificationMessage;

        public NotificationController(ModelsContext context, NotificationMessages notificationMessage)
        {
            _context = context;
            _notificationMessage = notificationMessage;
        }

        [HttpPost]
        public NotificationMessages AddNotification(string token)
        {
            var _document = new DocumentsController(_context);
            var doccieLOS = _document.UserLOS(token); 
            var doccieLOGS = _document.UserLOGS(token);

            if(doccieLOGS == "Document" && doccieLOS == "Document")
            {
                _notificationMessage.DocumenstNotification(token);
                return _notificationMessage;
            }
            else if (doccieLOGS == "Document" && doccieLOS == "No Document")
            {
                _notificationMessage.LOGS(token);
                return _notificationMessage;
            }
            else if (doccieLOGS == "No Document" && doccieLOS == "Document")
            {
                _notificationMessage.LOS(token);
                return _notificationMessage;
            }

            return null;
        }

        [HttpGet("{id}", Name = "GetNotificationById")]
        public async Task<Notifications> GetNotificationById(int id)
        {
            var notification = _context.Notifications.SingleOrDefault(x => x.Id == id);
            if(notification == null)
            {
               NotFound();
            }
            else
            {
                Ok();
            }
            return await _context.Notifications.FindAsync(id);
        }

        public IEnumerable<Notifications> GetNotificationsByUser([FromQueryAttribute] string token)
        {
            var notifications = _context.Notifications.Where(x => x.User.Token == token);
            if (notifications != null)
            {
                return notifications.ToList();
            }
            else
                return null;
        }

        [HttpPost("{id}")]

        public async Task DeleteAsync(int id)
        {
            var notification = await GetNotificationById(id);
           _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
