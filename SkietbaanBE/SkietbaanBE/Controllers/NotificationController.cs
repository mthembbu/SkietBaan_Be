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
            Notifications notification = new Notifications();
            try
            {
                notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    NotFound();
                }
            }
            catch(Exception ex)
            {
                var message = ex.Message;
            }
            return notification;
        }

        [HttpGet]
        public IEnumerable<Notifications> GetNotificationsByUser([FromQueryAttribute] string token)
        {
            try
            {
                if(token != null)
                {
                    var notifications = _context.Notifications.Where(x => x.User.Token == token);
                    var notificationsList = notifications.OrderByDescending(x => x.TimeOfArrival);
                    if (notificationsList != null)
                    {
                        return notificationsList.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        [HttpGet]
        public int GetNumberOfNotifications(string token)
        {
            try
            {
                var notificationsList = GetNotificationsByUser(token);
                if (notificationsList == null)
                {
                    return 0;
                }
                else
                {
                    var unReadNotifications = notificationsList.Where(x => x.IsRead == false);
                    return unReadNotifications.Count();
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            
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
                return new OkObjectResult("Invaild type for Id");
            }
            else
            {
                notification = await GetNotificationById(id);
            }

            if (notification == null)
            {
                return new OkObjectResult("Notification not found"); ;
            }
            else {
                try
                {
                    notification.IsRead = true;
                    _context.Notifications.Update(notification);
                }
                catch (Exception ex)
                {
                    var result = ex.Message;

                }
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
