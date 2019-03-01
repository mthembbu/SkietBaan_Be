using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
using System.Collections.Generic;

namespace SkietbaanBE.Lib {
    public class CheckAward {
        public static TotalAward Total(UserCompetitionTotalScore userCompetitionTotalScore) {
            TotalAward totalAward = new TotalAward();
            return null;    
        }

        private static Dictionary<string, string> ReadAwardsRules(string filename) {
            string[] lines = System.IO.File.ReadAllLines(@"award_rules.txt");
            Dictionary<string, string> awardsMap = new Dictionary<string, string>();
            foreach (string line in lines) {
                awardsMap.Add(line.Split(":")[0], line.Split(":")[1]);
            }

            return awardsMap;
        }
    }
}
