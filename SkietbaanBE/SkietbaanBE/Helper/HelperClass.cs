using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Models;

namespace SkietbaanBE.Helper
{
    public class HelperClass
    {
        public void Notification(ModelsContext _context, User user)
        {
            Notifications notification = new Notifications();
            notification.IsRead = false;
            notification.NotificationsHeading = "Membership Confirmation";
            notification.NotificationContent = "Welcome " + user.Username + " to the club! You are now a memeber. You're membership ID is: " + user.MemberID;
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public void Notification(ModelsContext _context, Competition comp)
        {
            Notifications notification = new Notifications();
            notification.IsRead = false;
            notification.NotificationsHeading = "New Competition Alert!";
            notification.NotificationContent = "Hello valued member. Please note there is a new competition that gas been created: " + comp.Name;
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public void Notification(ModelsContext _context, Group group)
        {
            Notifications notification = new Notifications();
            notification.IsRead = false;
            notification.NotificationsHeading = "New Group Alert!";
            notification.NotificationContent = "Hello valued member. Please note you have been invited to join a group named: " + group.Name;
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
    }
}
