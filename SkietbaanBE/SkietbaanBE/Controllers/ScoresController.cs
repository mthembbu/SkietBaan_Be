using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Scores")]
    public class ScoresController : Controller
    {
        private ModelsContext _context;
        public ScoresController(ModelsContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult ScoreCapture([FromBody]ScoreCapture scoreCapture)
        { 
            if (ModelState.IsValid)
            {
                var competition = _context.Competitions.Where(x => x.Name == scoreCapture.CompetitionName).FirstOrDefault<Competition>();
                if(competition == null)
                {
                    return new NotFoundObjectResult("Competition not found");
                }
                var user = _context.Users.Where(x => x.Username == scoreCapture.Username).FirstOrDefault<User>();
                if (user == null)
                {
                    return new NotFoundObjectResult("User not found");
                }
                var score = new Score() {
                    UserScore = scoreCapture.UserScore,
                    PictureURL = scoreCapture.PictureURL,
                    Competition = competition,
                    User = user,
                    UploadDate = DateTime.Today.AddMonths(3) //DONT FORGET TO REMOVE ADDMONTH!!!!!!!!!!!!!1
                };
                _context.Scores.Add(score);
                _context.SaveChanges();

                UpdateUserCompStats(score);
                UpdateTotalAndAverage(score);
                return Ok("Score Added Successfully");
            }
            else
            {
                return new BadRequestObjectResult("score cannot be null");
            }   
        }

        private void GetNumberOneRanked(Score score) {
            var compStats = _context.UserCompStats.Where(x => x.Competition.Id == score.Id);
            if (compStats.Count() == 0) return;

            User leader = compStats.OrderByDescending(x => x.Best).First().User;

        }

        private void UpdateUserCompStats(Score score) {         
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                            ucs.Competition.Id == score.Competition.Id &&
                                            ucs.Month == score.UploadDate.Value.Month &&
                                            ucs.Year == score.UploadDate.Value.Year);
            //New month
            if (userCompStatsRecords.Count() == 0) {
                UserCompStats userCompStats = new UserCompStats();
                userCompStats.Competition = score.Competition;
                userCompStats.User = score.User;
                userCompStats.Best = score.UserScore;
                userCompStats.Month = score.UploadDate.Value.Month;
                userCompStats.Year = score.UploadDate.Value.Year;

                _context.UserCompStats.Add(userCompStats);
            } else {
                var userCompStats = userCompStatsRecords.First();
                if (userCompStats.Best < score.UserScore) {
                    userCompStats.Best = score.UserScore;
                    _context.UserCompStats.Update(userCompStats);
                }
            }

            _context.SaveChanges();
        }

        private void UpdateTotalAndAverage(Score score) {
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                        ucs.Competition.Id == score.Competition.Id);

            if(userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() <= score.Competition.BestScoresNumber) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                if(userCompetitionTotalScore == null) {
                    userCompetitionTotalScore = new UserCompetitionTotalScore {
                        Competition = score.Competition,
                        User = score.User,
                        Total = userCompStatsRecords.Sum(ucs => ucs.Best)
                    };
                    userCompetitionTotalScore.Average = (double)userCompetitionTotalScore.Total;
                    _context.Add(userCompetitionTotalScore);
                } else {
                    userCompetitionTotalScore.Total = userCompStatsRecords.Sum(ucs => ucs.Best);
                    userCompetitionTotalScore.Average = (double)userCompetitionTotalScore.Total / (double)userCompStatsRecords.Count();
                    _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                }

                _context.SaveChanges();
            } else if(userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() > score.Competition.BestScoresNumber) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                userCompetitionTotalScore.Total = userCompStatsRecords.OrderByDescending(x => x.Best)
                                                   .Take(score.Competition.BestScoresNumber).Sum(x => x.Best);
                userCompetitionTotalScore.Average = (double)userCompetitionTotalScore.Total / (double)score.Competition.BestScoresNumber;
                _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                _context.SaveChanges();
            }

        }
    }
}
