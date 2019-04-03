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
    [Route("api/[Controller]/[Action]")]
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

            if (doccieLOGS == "Document" && doccieLOS == "Document")
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
            if (notification == null)
            {
                NotFound();
            }
            else
            {
                Ok();
            }
            return await _context.Notifications.FindAsync(id);
        }

        [HttpGet]
        public IEnumerable<Notifications> GetNotificationsByUser([FromQueryAttribute] string token)
        {
            var notifications = _context.Notifications.Where(x => x.User.Token == token);
            var query = notifications.OrderBy(x => x.IsRead == true);
            var notificationsList = notifications.OrderByDescending(x => x.TimeOfArrival);
            if (notificationsList != null)
            {
                return notificationsList.ToList();
            }
            else
                return null;
        }

        [HttpGet]
        public int GetNumberOfNotifications(string token)
        {
            var notificationsList = GetNotificationsByUser(token);
            var unReadNotifications = notificationsList.Where(x => x.IsRead == false);
            return unReadNotifications.Count();
        }

        [HttpPost]
        public void DeleteNotificationById([FromBody] List<Notifications> list)
        {
            try
            {
                _context.Notifications.RemoveRange(list);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            _context.SaveChanges();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateIsReadProperty(int id)
        {
            var notification = new Notifications();
            if (id.Equals(""))
            {
                return new BadRequestObjectResult("Invaild type for Id");
            }
            else
            {
                notification = await GetNotificationById(id);
            }

            notification.IsRead = true;
            try
            {
                _context.Notifications.Update(notification);
            }
            catch (Exception ex)
            {
                var result = ex.Message;
            }
            await _context.SaveChangesAsync();
            return new OkObjectResult("IsRead Property Updated Successfully");
        }

        [HttpPost]
        public void Announcements([FromBody] string message)
        {
            _notificationMessage.MakeAnnouncement(message);
        }
    }
}
