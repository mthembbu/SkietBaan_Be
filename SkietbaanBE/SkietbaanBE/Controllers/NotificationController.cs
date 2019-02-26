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

        [HttpGet("{id}", Name = "GetNotificationById")]
        public async Task<Notifications> GetNotificationById(int id)
        {
            var notification = _context.Notifications.SingleOrDefault(bright => bright.Id == id);
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
            var user = _context.Users.FirstOrDefault(x => x.Token == token);
            var notifications = _context.Notifications.Where(x => x.User == user);
           
            if (notifications != null)
            {
                return notifications.ToList();
            }
            else
                return null;
        }

        [HttpDelete("{id}")]

        public async Task DeleteAsync(int id)
        {
            var notification = await GetNotificationById(id);
           _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
