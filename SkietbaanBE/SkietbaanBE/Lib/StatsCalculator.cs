using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib {
    public class StatsCalculator {

        public static int TotalScore(ModelsContext context, string username) {
            try {
                int userId = context.Users.Where(u => u.Username == username).FirstOrDefault().Id;
                var total = context.Scores.Where(u => u.User.Id == userId).Sum(x => x.UserScore);
                return total;
            }catch(Exception e) {
                return -1;
            }
        }

        public static IEnumerable<Award> GetAward(ModelsContext context, string username) {
            var awards = context.Awards.Where(x => x.User.Username == username);
            return awards;
        }

        public static double Accuracy(ModelsContext context, string username) {
            try {
                int userId = context.Users.Where(u => u.Username == username).FirstOrDefault().Id;
                double total = (double)context.Scores.Where(u => u.User.Id == userId).Sum(x => x.UserScore);
                double numberOfScores = (double)context.Scores.Where(u => u.User.Id == userId).Count();

                return (total / (numberOfScores * 50)) * 100;
            }
            catch (Exception e) {
                return -1;
            }
        }
    }
}
