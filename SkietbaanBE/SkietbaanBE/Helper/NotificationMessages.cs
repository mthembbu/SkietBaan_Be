using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Controllers;
using SkietbaanBE.Models;

namespace SkietbaanBE.Helper
{
    public class NotificationMessages
    {
        enum TYPEOFNOTIFICATIONS
        {
            CONFIRMATION,
            RENEWAL,
            COMPETITION,
            GROUP,
            DOCUMENT,
            AWARD
        }
        ModelsContext _context;
        public NotificationMessages(ModelsContext context)
        {
            _context = context;
        }

        public void ConfirmationNotification(ModelsContext _context, User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Confirmation",
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
                notification.TypeOfNotification = "Competition";
                notification.NotificationMessage = comp.Name + ", has been created. Check it out!";
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
                TypeOfNotification = "Group",
                NotificationMessage = group.Name + ", created. Check it out!"
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

        public void LOS(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Document",
                NotificationMessage = "Hello " + user.Username + " Letter of Status received. Check your documents"
            };
            _context.Add(notification);
            _context.SaveChanges();
        }

        public void LOGS(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Document",
                NotificationMessage = "Hello " + user.Username + " Letter of Good Standing received. Check your documents"
            };
            _context.Add(notification);
            _context.SaveChanges();
        }

        public void DocumenstNotification(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Document",
                NotificationMessage = "Hello " + user.Username + " Letter of Status and Letter of Good Standing received. Check your documents"
            };
            _context.Add(notification);
            _context.SaveChanges();
        }

        public void TotalAwardNotification(string award)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received in overall total"
            };

            string message;
            try
            {
                _context.Add(notification);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            
            _context.SaveChanges();
        }

        public void AccuracyAwardNotification(string award)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received in accuracy"
            };

            string message;
            try
            {
                _context.Add(notification);
            }catch(Exception e)
            {
                message = e.Message;
            }
            _context.SaveChanges();
        }

        public void HoursAwardNotification(string award)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received "
            };

            string message;
            try
            {
                _context.Add(notification);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            _context.SaveChanges();
        }

        public void MonthAwardNotification(string description)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Award",
                NotificationMessage = description 
            };

            string message;
            try
            {
                _context.Add(notification);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            _context.SaveChanges();
        }
    }
}

