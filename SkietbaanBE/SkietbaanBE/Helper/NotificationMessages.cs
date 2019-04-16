using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Controllers;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;

namespace SkietbaanBE.Helper
{
    public class NotificationMessages
    {
        ModelsContext _context;
        string response = string.Empty;
        public NotificationMessages(ModelsContext context)
        {
            _context = context;
        }

        public void ConfirmationNotification(User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Confirmation",
                TimeOfArrival = DateTime.Now.ToString(),
                NotificationMessage = "Welcome to the club " + user.Username + ". You are now a member."
            };

            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();

                response = "successful";
            }
            catch (Exception e)
            {
                response = e.Message;
            }
        }

        public void RenewalNotification(User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Renewal",
                TimeOfArrival = DateTime.Now.ToString(),
                NotificationMessage = "Hello " + user.Username + ". Your membership has been renewed."
            };

            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();

                response = "successful";
            }
            catch (Exception e)
            {
                response = e.Message;
            }
        }

        public void ExpiryNotification(List<User> users){
            var notification = new Notifications();
            string expiryDate;
            foreach(User user in users)
            {
                expiryDate = user.MemberExpiryDate.Value.ToString("DD/MM/YYYY");
                notification.User = user;
                notification.IsRead = false;
                notification.TypeOfNotification = "Expiry";
                notification.TimeOfArrival = DateTime.Now.ToString();
                notification.NotificationMessage = "Hello " + user.Username + ". You're membership expires on ." + user.MemberExpiryDate;
            }

            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();

                response = "successful";
            }
            catch (Exception e)
            {
                response = e.Message;
            }
        }

        public void CompetitionNotification(Competition comp)
        {
            var userlist = _context.Users.ToList();
            foreach (var user in userlist)
            {
                var notification = new Notifications();
                notification.User = user;
                notification.IsRead = false;
                notification.TimeOfArrival = DateTime.Now.ToString();
                notification.TypeOfNotification = "Competition";
                notification.NotificationMessage = comp.Name + ", has been created. Check it out!";
                _context.Notifications.Add(notification);
            }
            _context.SaveChanges();
        }

        public void GroupNotification(Group group, User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Group",
                NotificationMessage = group.Name + ", group created and they added you!"
            };

            try
            {
                _context.Notifications.Add(notification);
                response = "successful";
            }
            catch (Exception e)
            {
                response = e.Message;
            }

            _context.SaveChanges();
        }

        public void LOS(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Document",
                NotificationMessage = "Hello " + user.Username + " Letter of Status received. Check your documents"
            };

            try
            {
                _context.Add(notification);
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            _context.SaveChanges();
        }

        public void LOGS(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Document",
                NotificationMessage = "Hello " + user.Username + " Letter of Good Standing received. Check your documents"
            };

            try
            {
                _context.Add(notification);
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            _context.SaveChanges();
        }

        public void DocumenstNotification(string token)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Document",
                NotificationMessage = "Hello " + user.Username + " Letter of Status and Letter of Good Standing received. Check your documents"
            };
            try
            {
                _context.Add(notification);
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            _context.SaveChanges();
        }

        public void TotalAwardNotification(string token, string award, string competitionName)
        {
            var user = _context.Users.FirstOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received in overall total for: " + competitionName
            };
            
            try
            {
                var dbNotitifications = _context.Notifications.Where(x => x.NotificationMessage.StartsWith(award)
                            && x.NotificationMessage.EndsWith(competitionName));
                if (dbNotitifications.Count() == 0) {
                    _context.Notifications.Add(notification);
                    _context.SaveChanges();
                }
                foreach (var dbNotification in dbNotitifications) {
                    if (!dbNotification.NotificationMessage.Contains("total")) {
                        _context.Notifications.Add(notification);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                response = e.Message;
            }
            
            _context.SaveChanges();
        }

        public void AccuracyAwardNotification(string token, string award, string competitionName)
        {
            var user = _context.Users.FirstOrDefault(x => x.Token == token);
            var notification = new Notifications {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received in accuracy for: " + competitionName
            };
            
            try
            {
                var dbNotitifications = _context.Notifications.Where(x => x.NotificationMessage.StartsWith(award)
                                            && x.NotificationMessage.EndsWith(competitionName));
                if (dbNotitifications.Count() == 0) {
                    _context.Notifications.Add(notification);
                    _context.SaveChanges();
                }
                foreach (var dbNotification in dbNotitifications) {
                    if (!dbNotification.NotificationMessage.Contains("accuracy")) {
                        _context.Notifications.Add(notification);
                        _context.SaveChanges();
                    }
                }
            }catch(Exception e)
            {
                response = e.Message;
            }
            
        }

        public void HoursAwardNotification(string token, string award, HoursAward hours)
        {
            var user = _context.Users.FirstOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received for " + hours.Hours + " hours"
            };

            try {
                var dbNotitifications = _context.Notifications.FirstOrDefault(x => x.NotificationMessage.StartsWith(award));
                if (dbNotitifications == null) {
                    _context.Notifications.Add(notification);
                    _context.SaveChanges();
                }
            } catch (Exception e) {
                response = e.Message;
            }
            
        }

        public void MonthAwardNotification(string token, string description)
        {
            var user = _context.Users.SingleOrDefault(x => x.Token == token);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = DateTime.Now.ToString(),
                TypeOfNotification = "Award",
                NotificationMessage = description 
            };
            
            try
            {
                _context.Add(notification);
            }
            catch (Exception e)
            {
                response = e.Message;
            }
            _context.SaveChanges();
        }

        public void MakeAnnouncement(string message)
        {
            List<User> userList = _context.Users.ToList();
           
            foreach (User user in userList)
            {
                var notification = new Notifications();
                notification.User = user;
                notification.IsRead = false;
                notification.TimeOfArrival = DateTime.Now.ToString();
                notification.TypeOfNotification = "Announcement";
                notification.NotificationMessage = message;
                _context.Add(notification);
            }
            _context.SaveChanges();
        }
    }
}

