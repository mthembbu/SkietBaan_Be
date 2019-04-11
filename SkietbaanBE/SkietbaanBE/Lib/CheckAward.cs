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
                if (hours.Hours >= 5) {
                    hours.Bronze = true;
                    notificationMessages.HoursAwardNotification("bronze", hours);
                }
                if (hours.Hours >= 10) {
                    hours.Silver = true;
                    notificationMessages.HoursAwardNotification("silver", hours);
                }
                if (hours.Hours >= 15) {
                    hours.Gold = true;
                    notificationMessages.HoursAwardNotification("gold", hours);
                }
            } catch (Exception) {
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
                            if (total >= requirement.Total) {
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

                            if(accuracy >= requirement.Accuracy) {
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
                            if (total >= requirement.Total) {
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

                            if (accuracy >= requirement.Accuracy) {
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
                            if (total >= requirement.Total) {
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

                            if (accuracy >= requirement.Accuracy) {
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
