using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Models;

namespace SkietbaanBE.Helper
{
    public class NotificationMessages
    {
        public NotificationMessages()
        {

        }

        public void ConfirmationNotification(ModelsContext _context, User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                typeOfNotification = "Confirmation",
                NotificationMessage = "Welcome to the club " + user.Username + ". You are now a member."
            };

            string message = string.Empty;
            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();

                message = "successful";
            }
            catch(Exception e)
            {
                message = e.Message;
            }
        }

        public void CompetitionNotification(ModelsContext _context, Competition comp)
        {
            var userlist = _context.Users.ToList();
            foreach (var user in userlist)
            {
                var notification = new Notifications();
                notification.User = user;
                notification.IsRead = false;
                notification.typeOfNotification = "Competition";
                notification.NotificationMessage = "Check it out! Competition " + comp.Name + " has been created.";
                _context.Notifications.Add(notification);
            }
            _context.SaveChanges();
        }

        public void GroupNotification(ModelsContext _context, Group group, User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                typeOfNotification = "Group",
                NotificationMessage = group.Name + " created. Check it out!"
            };
            string message = string.Empty;
            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();

                message = "successful";
            }
            catch (Exception e)
            {
                message = e.Message;
            }
        }
    }
}

