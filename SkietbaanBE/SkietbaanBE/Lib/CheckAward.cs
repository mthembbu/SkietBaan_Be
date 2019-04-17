using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
using System.Linq;
using System.Collections.Generic;
using System;
using SkietbaanBE.Helper;
using Microsoft.AspNetCore.Mvc;

namespace SkietbaanBE.Lib {
    public class CheckAward { 
        public static TotalAward Total(double total, bool isLocked, string token, string compName, ModelsContext context) {
            TotalAward totalAward = new TotalAward();
            var requirements = context.Requirements.Where(x => x.Competition.Name == compName);
            if (!isLocked) {
                foreach (var requirement in requirements) {
                    switch (requirement.Standard) {
                        case "gold":
                        case "Gold":
                            totalAward.Gold = total >= requirement.Total;
                            if (totalAward.Gold) {
                                totalAward.GoldRequirementStatus = requirement.Total + " REACHED";
                            } else {
                                totalAward.GoldRequirementStatus = "REACH " + requirement.Total + "%";
                            }
                            break;
                        case "silver":
                        case "Silver":
                            totalAward.Silver = total >= requirement.Total;
                            if (totalAward.Silver) {
                                totalAward.SilverRequirementStatus = requirement.Total + " REACHED";
                            } else {
                                totalAward.SilverRequirementStatus = "REACH " + requirement.Total + "%";
                            }
                            break;
                        case "bronze":
                        case "Bronze":
                            totalAward.Bronze = total >= requirement.Total;
                            if (totalAward.Bronze) {
                                totalAward.BronzeRequirementStatus = requirement.Total + " REACHED";
                            } else {
                                totalAward.BronzeRequirementStatus = "REACH " + requirement.Total + "%";
                            }
                            break;
                        default:
                            break;
                    }
                }
            } else {
                foreach (var requirement in requirements) {
                    switch (requirement.Standard) {
                        case "gold":
                        case "Gold":
                            totalAward.Gold = false;
                            totalAward.GoldRequirementStatus = "REACH " + requirement.Total + "%";
                            break;
                        case "silver":
                        case "Silver":
                            totalAward.Silver = false;
                            totalAward.SilverRequirementStatus = "REACH " + requirement.Total + "%";
                            break;
                        case "bronze":
                        case "Bronze":
                            totalAward.Bronze = false;
                            totalAward.BronzeRequirementStatus = "REACH " + requirement.Total + "%";
                            break;
                        default:
                            break;
                    }
                }
            }
            return totalAward;
        }

        public static AccuracyAward Accuracy(double compAccuracy, bool isLocked, string token,
                                                string compName, ModelsContext context) {
            AccuracyAward accuracyAward = new AccuracyAward();
            var requirements = context.Requirements.Where(x => x.Competition.Name == compName);
            if (!isLocked) {
                foreach (var requirement in requirements) {
                    switch (requirement.Standard) {
                        case "gold":
                        case "Gold":
                            accuracyAward.Gold = compAccuracy >= requirement.Accuracy;
                            if (accuracyAward.Gold) {
                                accuracyAward.GoldRequirementStatus = requirement.Accuracy + "% REACHED";
                            } else {
                                accuracyAward.GoldRequirementStatus = "REACH " + requirement.Accuracy + "%";
                            }
                            break;
                        case "silver":
                        case "Silver":
                            accuracyAward.Silver = compAccuracy >= requirement.Accuracy;
                            if (accuracyAward.Silver) {
                                accuracyAward.SilverRequirementStatus = requirement.Accuracy + "% REACHED";
                            } else {
                                accuracyAward.SilverRequirementStatus = "REACH " + requirement.Accuracy + "%";
                            }
                            break;
                        case "bronze":
                        case "Bronze":
                            accuracyAward.Bronze = compAccuracy >= requirement.Accuracy;
                            if (accuracyAward.Bronze) {
                                accuracyAward.BronzeRequirementStatus = requirement.Accuracy + "% REACHED";
                            } else {
                                accuracyAward.BronzeRequirementStatus = "REACH " + requirement.Accuracy + "%";
                            }
                            break;
                        default:
                            break;
                    }
                }
            } else {
                foreach(var requirement in requirements) {
                    switch (requirement.Standard) {
                        case "gold":
                        case "Gold":
                            accuracyAward.Gold = false;
                            accuracyAward.GoldRequirementStatus = "REACH " + requirement.Accuracy + "%";
                            break;
                        case "silver":
                        case "Silver":
                            accuracyAward.Silver = false;
                            accuracyAward.SilverRequirementStatus = "REACH " + requirement.Accuracy + "%";
                            break;
                        case "bronze":
                        case "Bronze":
                            accuracyAward.Bronze = false;
                            accuracyAward.BronzeRequirementStatus = "REACH " + requirement.Accuracy + "%";
                            break;
                        default:
                            break;
                    }
                } 
            }

            return accuracyAward;
        }

        public static HoursAward Hours(string token, ModelsContext context) {
            HoursAward hours = null;
            try {
                var hoursRecord = context.TimeSpents
                            .FirstOrDefault(x => x.User.Token == token);
                hours = new HoursAward {
                    Gold = false,
                    Bronze = false,
                    Silver = false,
                    Hours = 0,
                    MembershipNumber = context.Users.Where(x => x.Token == token).First().MemberID,
                    Username = context.Users.Where(x => x.Token == token).First().Username
                };

                NotificationMessages notificationMessages = new NotificationMessages(context);
                //Has not added any score in skietbaan
                if (hoursRecord == null) return hours;

                hours.Hours = hoursRecord.HoursSpent;
                //MAKE THIS DYNAMIC
                if (hours.Hours >= 20) {
                    var record = context.Awards.Where(x => x.User.Token == token)
                                .Where(desc => desc.Description.StartsWith("Hours:Bronze")).FirstOrDefault();
                    if (record == null) {
                        Award award = new Award {
                            Competition = null,
                            User = context.Users.FirstOrDefault(x => x.Token == token),
                            Month = DateTime.Today.Month,
                            Year = DateTime.Today.Year,
                            Description = "Hours:Bronze " + 20
                        };
                        
                        context.Awards.Add(award);
                        context.SaveChanges();
                        hours.Bronze = true;
                        notificationMessages.HoursAwardNotification(token, "bronze", hours);
                    }
                }
                if (hours.Hours >= 40) {
                    var record = context.Awards.Where(x => x.User.Token == token)
                                .Where(desc => desc.Description.StartsWith("Hours:Silver")).FirstOrDefault();
                    if (record == null) {
                        Award award = new Award {
                            Competition = null,
                            User = context.Users.FirstOrDefault(x => x.Token == token),
                            Month = DateTime.Today.Month,
                            Year = DateTime.Today.Year,
                            Description = "Hours:Silver " + 40
                        };

                        context.Awards.Add(award);
                        context.SaveChanges();
                        hours.Silver = true;
                        notificationMessages.HoursAwardNotification(token, "silver", hours);
                    }
                }
                if (hours.Hours >= 60) {
                    var record = context.Awards.Where(x => x.User.Token == token)
                                .Where(desc => desc.Description.StartsWith("Hours:Gold")).FirstOrDefault();
                    if(record == null) {
                        Award award = new Award {
                            Competition = null,
                            User = context.Users.FirstOrDefault(x => x.Token == token),
                            Month = DateTime.Today.Month,
                            Year = DateTime.Today.Year,
                            Description = "Hours:Gold " + 60
                        };
                        context.Awards.Add(award);
                        context.SaveChanges();
                        hours.Gold = true;
                        notificationMessages.HoursAwardNotification(token, "gold", hours);
                    }
                }
            } catch (Exception) {
                //LOG THE ERROR
                return hours;
            }
            return hours;
        }

        public static void UpdateHoursSpent(ModelsContext context, Score score) {
            TimeSpent dbRecord = null;
            try {
                dbRecord = context.TimeSpents.FirstOrDefault(x => x.UserId == score.User.Id);
                if (dbRecord == null) {
                    TimeSpent timeSpent = new TimeSpent {
                        User = score.User,
                        HoursSpent = score.Competition.Hours
                    };

                    context.TimeSpents.Add(timeSpent);
                } else {
                    dbRecord.HoursSpent += score.Competition.Hours;
                    context.TimeSpents.Update(dbRecord);
                }
                context.SaveChanges();
            } catch (Exception) {
                //LOG THIS ERROR TO FILE   
            }
        }

        /*TODO: REMOVE THE HARDCODED REQUIREMENTS*/
        public static void UpdateAccuracyAndTotalAward(Score score, ModelsContext context) {
            try {
                double total = context.UserCompetitionTotalScores
                            .FirstOrDefault(x => x.UserId == score.User.Id && x.CompetitionId == score.Competition.Id)
                            .Total;

                double sum = context.Scores
                            .Where(x => x.User.Id == score.User.Id && x.Competition.Id == score.Competition.Id)
                            .Sum(sc => sc.UserScore);
                int numberOfScores = context.Scores
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Count();
                double accuracy = Math.Round((sum / (numberOfScores * score.Competition.MaximumScore)) * 100, 1);

                var requirements = context.Requirements.Where(x => x.Competition.Id == score.Competition.Id);
                bool changeMade = false;
                foreach(var requirement in requirements) {
                    switch (requirement.Standard) {
                        case "Gold":
                        case "gold":
                            var goldReqTotal = requirement.Total;
                            if (goldReqTotal == 0) goldReqTotal = 90; // In case default requirement is not set in db
                            if (total >= goldReqTotal) {
                                if (IsAwardGiven("Total:Gold", score.User.Token, score.Competition.Name, context)) break;
                                NotificationMessages notificationMessages = new NotificationMessages(context);
                                notificationMessages.TotalAwardNotification(score.User.Token, "gold", score.Competition.Name);
                                Award award = new Award {
                                    Competition = score.Competition,
                                    User = score.User,
                                    Month = score.UploadDate.Value.Month,
                                    Year = score.UploadDate.Value.Year,
                                    Description = "Total:Gold " + requirement.Total
                                };
                                context.Awards.Add(award);
                                changeMade = true;
                            }
                            var goldReqAccuracy = requirement.Accuracy;
                            if (goldReqAccuracy == 0) goldReqAccuracy = 90; // In case default requirement is not set in db
                            if (accuracy >= goldReqAccuracy) {
                                if (IsAwardGiven("Accuracy:Gold", score.User.Token, score.Competition.Name, context)) break;
                                NotificationMessages notificationMessages = new NotificationMessages(context);
                                notificationMessages.AccuracyAwardNotification(score.User.Token, "gold", score.Competition.Name);
                                Award award = new Award {
                                    Competition = score.Competition,
                                    User = score.User,
                                    Month = score.UploadDate.Value.Month,
                                    Year = score.UploadDate.Value.Year,
                                    Description = "Accuracy:Gold " + requirement.Accuracy
                                };
                                context.Awards.Add(award);
                                changeMade = true;
                            }
                            break;
                        case "Silver":
                        case "silver":
                            var silverReqTotal = requirement.Total;
                            if (silverReqTotal == 0) silverReqTotal = 80; // In case default requirement is not set in db
                            if (total >= silverReqTotal) {
                                if (IsAwardGiven("Total:Silver", score.User.Token, score.Competition.Name, context)) break;
                                NotificationMessages notificationMessages = new NotificationMessages(context);
                                notificationMessages.TotalAwardNotification(score.User.Token, "silver", score.Competition.Name);
                                Award award = new Award {
                                    Competition = score.Competition,
                                    User = score.User,
                                    Month = score.UploadDate.Value.Month,
                                    Year = score.UploadDate.Value.Year,
                                    Description = "Total:Silver " + requirement.Total
                                };
                                context.Awards.Add(award);
                                changeMade = true;
                            }
                            var silverReqAccuracy = requirement.Accuracy;
                            if (silverReqAccuracy == 0) silverReqAccuracy = 80; // In case default requirement is not set in db
                            if (accuracy >= silverReqAccuracy) {
                                if (IsAwardGiven("Accuracy:Silver", score.User.Token, score.Competition.Name, context)) break;
                                NotificationMessages notificationMessages = new NotificationMessages(context);
                                notificationMessages.AccuracyAwardNotification(score.User.Token, "silver", score.Competition.Name);
                                Award award = new Award {
                                    Competition = score.Competition,
                                    User = score.User,
                                    Month = score.UploadDate.Value.Month,
                                    Year = score.UploadDate.Value.Year,
                                    Description = "Accuracy:Silver " + requirement.Accuracy
                                };
                                context.Awards.Add(award);
                                changeMade = true;
                            }
                            break;
                        case "Bronze":
                        case "bronze":
                            var bronzeReqTotal = requirement.Total;
                            if (bronzeReqTotal == 0) bronzeReqTotal = 70;// In case default requirement is not set in db
                            if (total >= bronzeReqTotal) {
                                if (IsAwardGiven("Total:Bronze", score.User.Token, score.Competition.Name, context)) break;
                                NotificationMessages notificationMessages = new NotificationMessages(context);
                                notificationMessages.TotalAwardNotification(score.User.Token, "bronze", score.Competition.Name);
                                Award award = new Award {
                                    Competition = score.Competition,
                                    User = score.User,
                                    Month = score.UploadDate.Value.Month,
                                    Year = score.UploadDate.Value.Year,
                                    Description = "Total:Bronze " + requirement.Total
                                };
                                context.Awards.Add(award);
                                changeMade = true;
                            }
                            var bronzeReqAccuracy = requirement.Accuracy;
                            if (bronzeReqAccuracy == 0) bronzeReqAccuracy = 70;// In case default requirement is not set in db
                            if (accuracy >= bronzeReqAccuracy) {
                                if (IsAwardGiven("Accuracy:Bronze", score.User.Token, score.Competition.Name, context)) break;
                                NotificationMessages notificationMessages = new NotificationMessages(context);
                                notificationMessages.AccuracyAwardNotification(score.User.Token, "bronze", score.Competition.Name);
                                Award award = new Award {
                                    Competition = score.Competition,
                                    User = score.User,
                                    Month = score.UploadDate.Value.Month,
                                    Year = score.UploadDate.Value.Year,
                                    Description = "Accuracy:Bronze " + requirement.Accuracy
                                };
                                context.Awards.Add(award);
                                changeMade = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (changeMade) context.SaveChanges();
            } catch {
                //LOG THE ERROR TO SOME FILE
            }
        }

        public static bool IsAwardGiven(string award, string token, string competitionName, ModelsContext context) {
            bool exist = context.Awards
                .Where(x => x.Competition.Name == competitionName && x.User.Token == token)
                .Where(desc => desc.Description.StartsWith(award)).FirstOrDefault() != null;
            return exist;
        }

        public static string MonthBest(int compId, string token, ModelsContext context) {
            Award award = null;
            try {
                award = context.Awards.FirstOrDefault(x => x.Competition.Id == compId && x.User.Token == token
                                                    && x.Month == DateTime.Today.Month && x.Year == DateTime.Today.Year);
            } catch {
                return "No award";
            }
            if (award != null) return award.Description;
            return "No award";
        }
    }
}
