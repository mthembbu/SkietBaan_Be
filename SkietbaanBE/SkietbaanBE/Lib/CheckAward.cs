using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
using System.Linq;
using System.Collections.Generic;

namespace SkietbaanBE.Lib {
    public class CheckAward {
        private static string awardRequirements = "Total:gold 90,silver 80,bronze 70\n" +
                                            "Hours:gold 100,silver 70,bronze 50\n" +
                                            "Accuracy:gold 95,silver 85,bronze 75";
        public static TotalAward Total(double total, bool isLocked) {
            TotalAward totalAward = new TotalAward();
            if (!isLocked) {
                var requirements = ReadAwardsRules()["Total"];
                foreach (string req in requirements.Split(',')) {
                    if (req.Contains("gold")) {
                        totalAward.Gold = total >= int.Parse(req.Split(' ')[1].Trim());
                    } else if (req.Contains("silver")) {
                        totalAward.Silver = total >= int.Parse(req.Split(' ')[1].Trim());
                    } else {
                        totalAward.Bronze = total >= int.Parse(req.Split(' ')[1].Trim());
                    }
                }
            } else {
                totalAward.Gold = false;
                totalAward.Silver = false;
                totalAward.Bronze = false;
            }
            return totalAward;
        }

        public static AccuracyAward Accuracy(double compAccuracy, bool isLocked) {
            AccuracyAward accuracyAward = new AccuracyAward();
            if (!isLocked) {
                var requirements = ReadAwardsRules()["Accuracy"];
                foreach (string req in requirements.Split(',')) {
                    if (req.Contains("gold")) {
                        accuracyAward.Gold = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                    } else if (req.Contains("silver")) {
                        accuracyAward.Silver = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                    } else {
                        accuracyAward.Bronze = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                    }
                }
            } else {
                accuracyAward.Gold = false;
                accuracyAward.Silver = false;
                accuracyAward.Bronze = false;
            }

            return accuracyAward;
        }

        public static HoursAward Hours(string token, ModelsContext context) {
            var hoursRecord = context.TimeSpents
                            .FirstOrDefault(x => x.User.Token == token);

            HoursAward hours = new HoursAward {
                Gold = false,
                Bronze = false,
                Silver = false,
                Hours = 0,
                MembershipNumber = context.Users.Where(x => x.Token == token).First().MemberID,
                Username = context.Users.Where(x => x.Token == token).First().Username
        };
            //Has not added any score in skietbaan
            if (hoursRecord == null) return hours;

            hours.Hours = hoursRecord.HoursSpent;
            //MAKE THIS DYNAMIC
            if (hours.Hours >= 5) hours.Bronze = true;
            if (hours.Hours >= 10) hours.Silver = true;
            if (hours.Hours >= 15) hours.Gold = true;

            return hours;
        }

        public static void UpdateHoursSpent(ModelsContext context, Score score) {
            var dbRecord = context.TimeSpents.FirstOrDefault(x => x.UserId == score.User.Id);

            if(dbRecord == null) {
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
