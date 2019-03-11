using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Lib {
    public class CheckAward {
        private static string awardRequirements = "Total:gold 90,silver 80,bronze 70\n" +
                                            "Hours:gold 100,silver 70,bronze 50\n" +
                                            "Accuracy:gold 95,silver 85,bronze 75";
        public static TotalAward Total(int total, bool isLocked) {
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

        public static AccuracyAward Accuracy(int compAccuracy, bool isLocked) {
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

        public static HoursAward Hours() {
            /*HoursAward hours = new HoursAward();
            var requirements = ReadAwardsRules("award_rules.txt")["Hours"];
            foreach (string req in requirements.Split(',')) {
                if (req.Contains("gold")) {
                    hours.gold = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                } else if (req.Contains("silver")) {
                    hours.silver = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                } else {
                    hours.bronze = compAccuracy >= int.Parse(req.Split(' ')[1].Trim());
                }
            }*/
            HoursAward hours = new HoursAward {
                Gold = false,
                Bronze = false,
                Silver = false
            };

            return hours;
        }

        private static Dictionary<string, string> ReadAwardsRules(string filename) {
            string[] lines = System.IO.File.ReadAllLines("award_rules.txt");
            Dictionary<string, string> awardsMap = new Dictionary<string, string>();
            foreach (string line in lines) {
                awardsMap.Add(line.Split(':')[0], line.Split(':')[1]);
            }

            return awardsMap;
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
