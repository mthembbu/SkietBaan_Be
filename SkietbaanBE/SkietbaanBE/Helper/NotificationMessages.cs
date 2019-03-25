﻿using System;
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
        ModelsContext _context;
        string response = string.Empty;
        public NotificationMessages(ModelsContext context)
        {
            _context = context;
        }

        public string GetTimeOfArrival()
        {
            var date = DateTime.Now;
            var timeOfArrivalInMinutes = TimeSpan.FromMinutes(date.Minute);
            var timeOfArrivalInHours = string.Format("{0:00}", (int)timeOfArrivalInMinutes.TotalHours, timeOfArrivalInMinutes.Minutes);
            return timeOfArrivalInHours;
        }

        public void ConfirmationNotification(ModelsContext _context, User user)
        {
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TypeOfNotification = "Confirmation",
                TimeOfArrival = GetTimeOfArrival(),
                NotificationMessage = "Welcome to the club " + user.Username + ". You are now a member."
            };

            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();

                response = "successful";
            }
            catch(Exception e)
            {
                response = e.Message;
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
                notification.TimeOfArrival = GetTimeOfArrival();
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
                TimeOfArrival = GetTimeOfArrival(),
                TypeOfNotification = "Group",
                NotificationMessage = group.Name + ", created. Check it out!"
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
                TimeOfArrival = GetTimeOfArrival(),
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
                TimeOfArrival = GetTimeOfArrival(),
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
                TimeOfArrival = GetTimeOfArrival(),
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

        public void TotalAwardNotification(string award)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = GetTimeOfArrival(),
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received in overall total"
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

        public void AccuracyAwardNotification(string award)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = GetTimeOfArrival(),
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received in accuracy"
            };
            
            try
            {
                _context.Add(notification);
            }catch(Exception e)
            {
                response = e.Message;
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
                TimeOfArrival = GetTimeOfArrival(),
                TypeOfNotification = "Award",
                NotificationMessage = award + " award received "
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

        public void MonthAwardNotification(string description)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == 1);
            var notification = new Notifications
            {
                User = user,
                IsRead = false,
                TimeOfArrival = GetTimeOfArrival(),
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
    }
}

