using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        private ModelsContext _context;

        public NotificationController(ModelsContext context)
        {
            _context = context;
        }

        // GET: api/Notification
        [HttpGet]
        public IEnumerable<Notifications> Get()
        {
            var notificationQuery = from noti in _context.Notifications
                                   where noti.IsRead == false
                                   select noti;
           //var userQuery = from user in _context.Users
                           // where user.Username == username
                           // select user;
            List < Notifications > notificationList = notificationQuery.ToList<Notifications>();
            return notificationList;
        }
    }
}
