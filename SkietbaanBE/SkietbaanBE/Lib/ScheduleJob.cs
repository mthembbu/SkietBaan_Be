using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SkietbaanBE.Lib {
    public class ScheduleJob {

        private ModelsContext _context;
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

        private long GetMilliSecondsToNextMonth() {
            int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            var target = new DateTime(DateTime.Today.Year, DateTime.Today.Month, daysInMonth);
            var current = DateTime.Today;

            return (long)target.Subtract(current).TotalMilliseconds;
        }

        //Add a record in the database of the user who has been top ranked at the end of the month
        private void AwardUserJob(Object source) {
            //join all competitions where the user participates, but only the active competitions

            var query = (from User in _context.Users
                        join UserCompetitionTotalScore in _context.UserCompetitionTotalScores on User.Id
                            equals UserCompetitionTotalScore.UserId
                        join Competition in _context.Competitions on UserCompetitionTotalScore.CompetitionId 
                            equals Competition.Id
                        where (Competition.Status == true && UserCompetitionTotalScore.Year == DateTime.Today.Year)
                        orderby Competition.Id, UserCompetitionTotalScore.Total descending
                        select new{
                            User,
                            Competition.Id,
                            Competition,
                            UserCompetitionTotalScore.Total
                        }).Distinct();

            //award the users who ranked first in their respective competitions
            var allCompIds = query.Select(x => x.Id).Distinct();
            foreach(var i in allCompIds){
                var top = query.Where(x => x.Id == i).First();
                var comp = _context.Competitions.Find(top.Id);
                var user = _context.Users.Find(top.User.Id);
                Award award = new Award {
                    Competition = comp,
                    User = user,
                    Description = $"Best shooter in {GetMonthNameByMonthNumber(DateTime.Today.Month)}" + " " +
                                    $"{DateTime.Today.Year}",
                    Month = DateTime.Today.Month,
                    Year = DateTime.Today.Year
                };
                _notificationMessage.MonthAwardNotification(award.Description);
                _context.Awards.Add(award);
            }

            _context.SaveChanges();

            //run the updateAwardTimer once again after 20 days
            updateAwardTimer.Change((int)TimeSpan.FromDays(20).TotalMilliseconds, 0);
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
    }
}
