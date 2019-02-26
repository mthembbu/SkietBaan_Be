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
            Notifications notification = new Notifications
            {
                User = user,
                IsRead = false,
                typeOfNotification = "Confirmation",
                NotificationMessage = "Welcome " + user.Username + " to the club! You are now a member."
            };

            string message = "";
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

        public void RenewalNotification(ModelsContext _context, User user)
        {
            //var userMonth = user.MemberExpiryDate;
            //var currentMonth = DateTime.Now.Month;
            //var result = currentMonth - userMonth;
            //Notifications notification = new Notifications();
            //if (result < 2)
            //{
            //    notification.User = user;
            //    notification.IsRead = false;
            //    notification.typeOfNotification = "Renewal";
            //    notification.NotificationMessage = "Dear " + user.Username + " your membership is due on " + user.MemberExpiry + " .";
            //}

            //string message = "";
            //try
            //{
            //    _context.Notifications.Add(notification);
            //    _context.SaveChanges();

            //    message = "successful";
            //}
            //catch (Exception e)
            //{
            //    message = e.Message;
            //}
        }

        public void CompetitionNotification(ModelsContext _context, Competition comp)
        {
            Notifications notification = new Notifications();
            var userlist = _context.Users.ToList();
            foreach (var user in userlist)
            {
                notification.User = user;
                notification.IsRead = false;
                notification.typeOfNotification = "Competition";
                notification.NotificationMessage = "Hello valued member. Please note there is a new competition that has been created: " + comp.Name;
            }

            string message = "";
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

        public void GroupNotification(ModelsContext _context, Group group)
        {
            Notifications notification = new Notifications
            {
                IsRead = false,
                typeOfNotification = "Group",
                NotificationMessage = "Hello valued member. Please note you have been invited to join a group named: " + group.Name
            };
            string message = "";
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

        public void AwardNotification(ModelsContext _context /*, Awards award*/)
        {
            Notifications notification = new Notifications
            {
                IsRead = false,
                typeOfNotification = "Award",
                NotificationMessage = "Congratulation you have received a award.name award! Keep shooting."
            };
            string message = "";
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
