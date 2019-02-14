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
            List<Notifications> notificationList = notificationQuery.ToList<Notifications>();
            return notificationList;
        }

        // GET: api/Notification/5
        [HttpGet("{id}", Name = "GetNotifications")]
        public string Get(int id)
        {
            return "value";
        }
        
        private string Notifications(User user)
        {
            return "";
        }

        // POST: api/Notification
        [HttpPost]
        public void Post([FromBody]string value)
        {

        }
        
        // PUT: api/Notification/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
