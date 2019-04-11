using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
using System.Linq;
using System.Collections.Generic;
using System;
using SkietbaanBE.Helper;
using Microsoft.AspNetCore.Mvc;

namespace SkietbaanBE.Lib {
    public class CheckAward {
        private static string awardRequirements = "Total:gold 90,silver 80,bronze 70\n" +
                                            "Hours:gold 100,silver 70,bronze 50\n" +
                                            "Accuracy:gold 95,silver 85,bronze 75";
        public static TotalAward Total(double total, bool isLocked, string compName, ModelsContext context) {
            TotalAward totalAward = new TotalAward();
            if (!isLocked) {
                var requirements = ReadAwardsRules()["Total"];
                foreach (string req in requirements.Split(',')) {
                    if (req.Contains("gold")) {
                        totalAward.Gold = total >= int.Parse(req.Split(' ')[1].Trim());
                        if (totalAward.Gold) {
                            NotificationMessages notificationMessages = new NotificationMessages(context);
                            notificationMessages.TotalAwardNotification("gold", compName);
                        }
                        if (totalAward.Gold)
                            totalAward.GoldRequirementStatus = req.Split(' ')[1].Trim() + " REACHED";
                        else
                            totalAward.GoldRequirementStatus = "REACH " + req.Split(' ')[1].Trim();
                    } else if (req.Contains("silver")) {
                        totalAward.Silver = total >= int.Parse(req.Split(' ')[1].Trim());
                        if (totalAward.Silver) {
                            NotificationMessages notificationMessages = new NotificationMessages(context);
                            notificationMessages.TotalAwardNotification("silver", compName);
                        }
                        if (totalAward.Silver)
                            totalAward.SilverRequirementStatus = req.Split(' ')[1].Trim() + " REACHED";
                        else
                            totalAward.SilverRequirementStatus = "REACH " + req.Split(' ')[1].Trim();
                    } else {
                        totalAward.Bronze = total >= int.Parse(req.Split(' ')[1].Trim());
                        if (totalAward.Bronze) {
                            NotificationMessages notificationMessages = new NotificationMessages(context);
                            notificationMessages.TotalAwardNotification("bronze", compName);
                        }
                        if (totalAward.Bronze)
                            totalAward.BronzeRequirementStatus = req.Split(' ')[1].Trim() + " REACHED";
                        else
                            totalAward.BronzeRequirementStatus = "REACH " + req.Split(' ')[1].Trim();
                    }
                }
            } else {
                var requirements = ReadAwardsRules()["Total"];
                totalAward.Gold = false;
                totalAward.GoldRequirementStatus = "REACH " + requirements.Split(',')[0].Split(' ')[1];
                totalAward.Silver = false;
                totalAward.SilverRequirementStatus = "REACH " + requirements.Split(',')[1].Split(' ')[1];
                totalAward.Bronze = false;
                totalAward.BronzeRequirementStatus = "REACH " + requirements.Split(',')[2].Split(' ')[1];
            }
            return totalAward;
        }

        public static AccuracyAward Accuracy(double compAccuracy, bool isLocked, string compName, ModelsContext context) {
            AccuracyAward accuracyAward = new AccuracyAward();
            if (!isLocked) {
                var requirements = ReadAwardsRules()["Accuracy"];
                foreach (string req in requirements.Split(',')) {
                    if (req.Contains("gold")) {
                        accuracyAward.Gold = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                        if (accuracyAward.Gold) {
                            NotificationMessages notificationMessages = new NotificationMessages(context);
                            notificationMessages.AccuracyAwardNotification("gold", compName);
                        }
                        if (accuracyAward.Gold)
                            accuracyAward.GoldRequirementStatus = req.Split(' ')[1].Trim() + "% REACHED";
                        else
                            accuracyAward.GoldRequirementStatus = "REACH " + req.Split(' ')[1].Trim() + "%";
                    } else if (req.Contains("silver")) {
                        accuracyAward.Silver = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                        if (accuracyAward.Silver) {
                            NotificationMessages notificationMessages = new NotificationMessages(context);
                            notificationMessages.AccuracyAwardNotification("silver", compName);
                        }
                        if (accuracyAward.Silver)
                            accuracyAward.SilverRequirementStatus = req.Split(' ')[1].Trim() + "% REACHED";
                        else
                            accuracyAward.SilverRequirementStatus = "REACH " + req.Split(' ')[1].Trim() + "%";
                    } else {
                        accuracyAward.Bronze = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                        if (accuracyAward.Bronze) {
                            NotificationMessages notificationMessages = new NotificationMessages(context);
                            notificationMessages.AccuracyAwardNotification("bronze", compName);
                        }
                        if (accuracyAward.Bronze)
                            accuracyAward.BronzeRequirementStatus = req.Split(' ')[1].Trim() + "% REACHED";
                        else
                            accuracyAward.BronzeRequirementStatus = "REACH " + req.Split(' ')[1].Trim() + "%";
                    }
                }
            } else {
                var requirements = ReadAwardsRules()["Accuracy"];
                accuracyAward.Gold = false;
                accuracyAward.GoldRequirementStatus = "REACH " + requirements.Split(',')[0].Split(' ')[1] + "%";
                accuracyAward.Silver = false;
                accuracyAward.SilverRequirementStatus = "REACH " + requirements.Split(',')[1].Split(' ')[1] + "%";
                accuracyAward.Bronze = false;
                accuracyAward.BronzeRequirementStatus = "REACH " + requirements.Split(',')[2].Split(' ')[1] + "%";
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
                        notificationMessages.HoursAwardNotification("bronze", hours);
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
                        notificationMessages.HoursAwardNotification("silver", hours);
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
                        notificationMessages.HoursAwardNotification("gold", hours);
                    }
                }
            } catch (Exception) {
                //LOG THE ERROR
                return hours;
            }
            return hours;
        }

        public static IActionResult UpdateHoursSpent(ModelsContext context, Score score) {
            TimeSpent dbRecord = null;
            try {
                dbRecord = context.TimeSpents.FirstOrDefault(x => x.UserId == score.User.Id);
            } catch (Exception) {
                return new BadRequestObjectResult("Something went wrong");
            }

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
            return new OkObjectResult("Updated succesfully");
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
                            if (goldReqTotal == 0) goldReqTotal = 90;
                            if (total >= goldReqTotal) {
                                var record = context.Awards
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Where(desc => desc.Description.StartsWith("Total:Gold")).FirstOrDefault();
                                if (record != null) break;
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
                            if (goldReqAccuracy == 0) goldReqAccuracy = 90;
                            if (accuracy >= goldReqAccuracy) {
                                var record = context.Awards
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Where(desc => desc.Description.StartsWith("Accuracy:Gold")).FirstOrDefault();
                                if (record != null) break;
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
                            if (silverReqTotal == 0) silverReqTotal = 80;
                            if (total >= silverReqTotal) {
                                var record = context.Awards
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Where(desc => desc.Description.StartsWith("Total:Silver")).FirstOrDefault();
                                if (record != null) break;
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
                            if (silverReqAccuracy == 0) silverReqAccuracy = 80;
                            if (accuracy >= silverReqAccuracy) {
                                var record = context.Awards
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Where(desc => desc.Description.StartsWith("Accuracy:Silver")).FirstOrDefault();
                                if (record != null) break;
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
                            if (bronzeReqTotal == 0) bronzeReqTotal = 70;
                            if (total >= bronzeReqTotal) {
                                var record = context.Awards
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Where(desc => desc.Description.StartsWith("Total:Bronze")).FirstOrDefault();
                                if (record != null) break;
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
                            if (bronzeReqAccuracy == 0) bronzeReqAccuracy = 80;
                            if (accuracy >= bronzeReqAccuracy) {
                                var record = context.Awards
                                    .Where(x => x.Competition.Id == score.Competition.Id && x.User.Id == score.User.Id)
                                    .Where(desc => desc.Description.StartsWith("Accuracy:Bronze")).FirstOrDefault();
                                if (record != null) break;
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

        public static IActionResult MonthBest(int compId, string token, ModelsContext context) {
            Award award = null;
            try {
                award = context.Awards.FirstOrDefault(x => x.Competition.Id == compId && x.User.Token == token
                                                    && x.Month == DateTime.Today.Month && x.Year == DateTime.Today.Year);
            } catch {
                return new BadRequestObjectResult("Something went wrong");
            }
            if (award != null) return new OkObjectResult(award.Description);
            return new OkObjectResult("No award");
        }

        private static Dictionary<string, string> ReadAwardsRules() {
            string[] lines = awardRequirements.Split('\n');

            Dictionary<string, string> awardsMap = new Dictionary<string, string>();
            foreach (string line in lines) {
                awardsMap.Add(line.Split(':')[0], line.Split(':')[1]);
            }

            return awardsMap;
        }

    }
}
