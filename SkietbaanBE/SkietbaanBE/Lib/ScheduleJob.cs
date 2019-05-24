using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SkietbaanBE.Lib {
    public class ScheduleJob {

        private static ModelsContext _context;
        private Timer awardTimer; //this timer runs only once at the end of every month
        private Timer updateAwardTimer; //this timer updates the awardTimer on how many days are in the current month
        private NotificationMessages _notificationMessage;

        public ScheduleJob(ModelsContext context) {
            _context = context;
            _notificationMessage = new NotificationMessages(context);
            updateAwardTimer = new Timer(
                callback: new TimerCallback(RunAwardUserJob),
                state: "",
                dueTime: 10000, //run RunAwardUserJob after 10s of this object creation
                period: 0
            );
        }
        
        /*TODO: ADD FEW MILLISECONDS TO MAKE SURE THAT THIS FUNCTION*/
        private long GetMilliSecondsToNextMonth() {
            int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            var target = new DateTime(DateTime.Today.Year, DateTime.Today.Month, daysInMonth);
            var current = DateTime.Today;

            return (long)target.Subtract(current).TotalMilliseconds;
        }

        //Add a record in the database of the user who has been top ranked at the end of the month
        private void AwardUserJob(Object source) {
            //join all competitions where the user participates, but only the active competitions
            try {
                var competitions = _context.Competitions.Where(x => x.Status == true);
                foreach (var competition in competitions) {
                    var dbTotalScoreRecord = _context.UserCompetitionTotalScores.Where(x => x.Competition.Name == competition.Name)
                        .OrderByDescending(x => x.Total).FirstOrDefault();
                    if (dbTotalScoreRecord == null) continue;
                    double maxTotal = dbTotalScoreRecord.Total;
                    var usersWithSameMax = _context.UserCompetitionTotalScores.Where(x => x.Total == maxTotal);
                    if (usersWithSameMax.Count() > 3) {
                        usersWithSameMax = usersWithSameMax.OrderByDescending(x => x.Total)
                                .OrderByDescending(x => x.Average).Take(3);
                        var users = usersWithSameMax.Select(x => x.User).ToList();
                        int counter = 0;
                        foreach (var topUser in usersWithSameMax) {
                            topUser.User = users.ElementAt(counter);
                            counter++;
                            Award award = new Award {
                                Competition = competition,
                                User = topUser.User,
                                Month = DateTime.Today.Month,
                                Year = DateTime.Today.Year,
                                Description = $"Month:{topUser.Total}"
                            };
                            var dbRecord = _context.Awards.Where(x => x.Competition.Name == award.Competition.Name &&
                                x.User.Id == award.User.Id && x.Month == award.Month && x.Year == award.Year).FirstOrDefault();
                            if(dbRecord == null)
                                _context.Awards.Add(award);
                        }
                    } else {
                        var topUsers = _context.UserCompetitionTotalScores.Where(x => x.Competition.Name == competition.Name)
                            .OrderByDescending(x => x.Total).Take(3);
                        var users = topUsers.Select(x => x.User).ToList();
                        int counter = 0;
                        foreach (var topUser in topUsers) {
                            topUser.User = users.ElementAt(counter);
                            counter++;
                            Award award = new Award {
                                Competition = competition,
                                User = topUser.User,
                                Month = DateTime.Today.Month,
                                Year = DateTime.Today.Year,
                                Description = $"Month:{topUser.Total}"
                            };

                            var dbRecord = _context.Awards.Where(x => x.Competition.Name == award.Competition.Name &&
                                x.User.Id == award.User.Id && x.Month == award.Month && x.Year == award.Year).FirstOrDefault();
                            if (dbRecord == null)
                                _context.Awards.Add(award);
                        }
                    }
                }
                _context.SaveChanges();
                //run the updateAwardTimer once again after 20 days
                updateAwardTimer.Change((int)TimeSpan.FromDays(20).TotalMilliseconds, 0);
            } catch (Exception e) {
                // log to file
            }
        }   

        public void RunAwardUserJob(Object sender) {
            if (awardTimer == null) {
                awardTimer = new Timer(
                    callback: new TimerCallback(AwardUserJob),
                    state: "",
                    dueTime: GetMilliSecondsToNextMonth(),
                    period: 0
                );
            } else {
                awardTimer.Change(GetMilliSecondsToNextMonth(), 0);
            }
        }

        private string GetMonthNameByMonthNumber(int monthNumber) {
            string strMonth = "";
            switch (monthNumber) {
                case 1:
                    strMonth = "January";
                    break;
                case 2:
                    strMonth = "February";
                    break;
                case 3:
                    strMonth = "March";
                    break;
                case 4:
                    strMonth = "April";
                    break;
                case 5:
                    strMonth = "May";
                    break;
                case 6:
                    strMonth = "June";
                    break;
                case 7:
                    strMonth = "July";
                    break;
                case 8:
                    strMonth = "August";
                    break;
                case 9:
                    strMonth = "September";
                    break;
                case 10:
                    strMonth = "October";
                    break;
                case 11:
                    strMonth = "November";
                    break;
                case 12:
                    strMonth = "December";
                    break;
            }
            return strMonth;
        }

        public static void ReNewUserMemberShip(string tokens, DateTime target)
        {
            SendMail sendMail = new SendMail();
            sendMail.EmailDebugger("mandlamasombuka21@gmail.com", "job scheduled", "scheduled for " + tokens +" for day " + target.ToString());
            long time;
            {
                User dbUser = _context.Users.FirstOrDefault(x => x.Token == tokens);
                var current = DateTime.Now;
                time = (long)target.Subtract(current).TotalMilliseconds;  
            }
            sendMail.EmailDebugger("mandlamasombuka21@gmail.com", "time", "target in milliseconds: " + time);
            void sendDetails(Object source)
            {
                SendMail sendMailTimer = new SendMail();
                try {
                    User dbUser = _context.Users.FirstOrDefault(x => x.Token == tokens);
                    sendMailTimer.EmailDebugger("mandlamasombuka21@gmail.com", "job is running", "user: " + dbUser.Username);
                    dbUser.MemberExpiryDate = dbUser.AdvanceExpiryDate.Value.AddYears(+1);
                    dbUser.AdvanceExpiryDate = null;
                    _context.Users.Update(dbUser);
                    _context.SaveChanges();
                    sendMailTimer.EmailDebugger("mandlamasombuka21@gmail.com", "job is done", "new expiry: " + dbUser.MemberExpiryDate.ToString());
                } catch (Exception e) {
                    sendMailTimer.EmailDebugger("mandlamasombuka21@gmail.com", "exception", e.Message);
                }
            }
            new Timer(
               callback: new TimerCallback(sendDetails),
                      state: "",
                      dueTime: time,
                      period: 0
                  );
        }
    }
}
