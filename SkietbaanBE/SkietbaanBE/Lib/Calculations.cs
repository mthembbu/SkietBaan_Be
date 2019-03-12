using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib
{
    public class Calculations
    {
        private ModelsContext _context;
        public Calculations(ModelsContext contex)
        {
            _context = contex;
        }

        private List<double> YearBestScores(int userID, int competitionID)
        {
            List<double> listBestScores = new List<double>();
            for (int i = 1; i < 13; i++)
            {
                var query = from Scores in _context.Scores
                            where (Scores.User.Id == userID && Scores.Competition.Id == competitionID && Scores.UploadDate.Value.Month == i)
                            select new
                            {
                                Scores.UserScore
                            };
                List<double> listScores = new List<double>();
                foreach (var item in query)
                {
                    listScores.Add(item.UserScore);
                }
                if (listScores.Count > 0)
                {
                    listScores.Sort();
                    listBestScores.Add(listScores.Last());
                }

            }
            return listBestScores;
        }
        public void performCalculations(int userID,int competitionID)
        {
            
            
            Competition competition = _context.Competitions.Where(c => c.Id == competitionID).FirstOrDefault<Competition>();

            List<double> bestScores = YearBestScores(userID, competitionID).TakeLast(competition.BestScoresNumber).ToList<double>();
            //calculate total
            double total = bestScores.Sum();
            double average = total / competition.BestScoresNumber;
            double best = bestScores.Last();

            UserCompetitionTotalScore stats = _context.UserCompetitionTotalScores
                                                      .Where(u => u.UserId == userID && u.CompetitionId == competitionID)
                                                      .FirstOrDefault<UserCompetitionTotalScore>();
            if(stats == null)
            {
                stats = new UserCompetitionTotalScore()
                {
                    CompetitionId = competitionID,
                    UserId = userID,
                    Total = total,
                    Average = average,
                    Best = best
                };
                _context.UserCompetitionTotalScores.Add(stats);
                _context.SaveChanges();
            }
            else
            {
                stats.Average = average;
                stats.Best = best;
                stats.Total = total;
                _context.UserCompetitionTotalScores.Update(stats);
                _context.SaveChanges();
            }
        }
    }
}
