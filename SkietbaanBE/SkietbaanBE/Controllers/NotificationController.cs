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
        public void AddNotification([FromBody] string token)
        {
            var _document = new DocumentsController(_context);
            var doccieLOS = _document.UserLOS(token);
            var doccieLOGS = _document.UserLOGS(token);

            if (doccieLOGS == "Document" && doccieLOS == "Document")
            {
                _notificationMessage.DocumenstNotification(token);
            }
            else if ((doccieLOGS == "Document" && doccieLOS == "No Document") || (doccieLOGS == "Document" && doccieLOS == "Admin has not set requirements for letter of dedicated status"))
            {
                _notificationMessage.LOGS(token);
            }
            else if ((doccieLOGS == "No Document" && doccieLOS == "Document") || (doccieLOGS == "Admin has not set requirements for letter of dedicated status" && doccieLOS == "Document"))
            {
                _notificationMessage.LOS(token);
            }
            else
            {
                _notificationMessage.LOGS(token);
            }
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
                    var notificationsList = notifications.OrderBy(x => x.TypeOfNotification == "Deleted");
                    var newList = new List<Notifications>();
                    foreach(var notification in notificationsList)
                    {
                        if (notification.TypeOfNotification != "Deleted")
                        {
                            newList.Add(notification);
                        }
                        else
                            break;
                    }
                    if (newList != null)
                    {
                        return newList.OrderByDescending(x => x.Id);
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
                List<Notifications> newList = new List<Notifications>();
                foreach (var notification in list)
                {
                    if(notification.TypeOfNotification == "Expiry")
                    {
                        notification.TypeOfNotification = "Deleted";
                        _context.Notifications.Update(notification);
                    }
                    else
                    {
                        newList.Add(notification);
                    }
                }
                _context.Notifications.RemoveRange(newList);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
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
