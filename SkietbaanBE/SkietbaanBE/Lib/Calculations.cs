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

        private YearScores GetYearScores(int userID, int competitionID)
        {
            List<double> listBestScores = new List<double>();
            List<double> listYearAllScores = new List<double>();
            int year = 0;
            for (int i = 1; i < 13; i++)
            {
                var query = from Scores in _context.Scores
                            where (Scores.User.Id == userID && Scores.Competition.Id == competitionID && Scores.UploadDate.Value.Month == i)
                            select new
                            {
                                Scores.UserScore,
                               Scores.UploadDate.Value.Year
                            };
                List<double> listScores = new List<double>();
                foreach (var item in query)
                {
                    listScores.Add(item.UserScore);
                    year = item.Year;
                }
                if (listScores.Count > 0)
                {
                    listScores.Sort();
                    listBestScores.Add(listScores.Last());
                    listYearAllScores.AddRange(listScores);
                }


            }

            YearScores yearScores = new YearScores();
            yearScores.listYearBest = listBestScores;
            yearScores.listYearScores = listYearAllScores;
            if(listBestScores.Count > 0 && listYearAllScores.Count > 0) {
                yearScores.Year = year;
            }
            return yearScores;
        }
        public void performCalculations(int userID,int competitionID)
        {

            Competition competition = _context.Competitions.Where(c => c.Id == competitionID).FirstOrDefault<Competition>();

            YearScores yearScores = GetYearScores(userID, competitionID);
            List<double> bestScores= yearScores.listYearBest.TakeLast(competition.BestScoresNumber).ToList<double>();
            //calculate total
            double total = Math.Round(bestScores.Sum() / competition.BestScoresNumber,2);
            double average = Math.Round(yearScores.listYearScores.Sum() / yearScores.listYearScores.Count,2);
            double best = Math.Round(bestScores.Last(),2);
            int year = yearScores.Year;
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
                    Best = best,
                    Year = year,
                    PreviousTotal = 0
                };
                _context.UserCompetitionTotalScores.Add(stats);
                _context.SaveChanges();
            }
            else
            {
                stats.PreviousTotal = stats.Average; //save the old average as previous average
                stats.Average = average;
                stats.Best = best;
                stats.Total = total;
                _context.UserCompetitionTotalScores.Update(stats);
                _context.SaveChanges();
            }
        }
    }
}
