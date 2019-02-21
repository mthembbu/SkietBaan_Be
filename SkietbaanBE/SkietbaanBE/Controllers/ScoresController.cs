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
                    UploadDate = DateTime.Today
                };
                _context.Scores.Add(score);
                _context.SaveChanges();

                UpdateUserCompStats(score);
                UpdateTotal(score);
                return Ok("Score Added Successfully");
            }
            else
            {
                return new BadRequestObjectResult("score cannot be null");
            }   
        }

        private void UpdateUserCompStats(Score score) {         
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                            ucs.Competition.Id == score.Competition.Id &&
                                            ucs.Month == score.UploadDate.Value.Month &&
                                            ucs.Year == score.UploadDate.Value.Year);

            if (userCompStatsRecords.Count() < 1) {
                UserCompStats userCompStats = new UserCompStats();
                userCompStats.Competition = score.Competition;
                userCompStats.User = score.User;
                userCompStats.BestScore = score.UserScore;
                userCompStats.Month = score.UploadDate.Value.Month;
                userCompStats.Year = score.UploadDate.Value.Year;

                _context.UserCompStats.Add(userCompStats);
            } else {
                var userCompStats = userCompStatsRecords.First();
                if (userCompStats.BestScore < score.UserScore) {
                    userCompStats.BestScore = score.UserScore;
                    _context.UserCompStats.Update(userCompStats);
                }
            }

            _context.SaveChanges();
        }

        private void UpdateTotal(Score score) {
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
                        Total = userCompStatsRecords.Sum(ucs => ucs.BestScore)
                    };
                    _context.Add(userCompetitionTotalScore);
                } else {
                    userCompetitionTotalScore.Total = userCompStatsRecords.Sum(ucs => ucs.BestScore);
                    _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                }
                _context.SaveChanges();
            } else if(userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() > score.Competition.BestScoresNumber) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                userCompetitionTotalScore.Total = userCompStatsRecords.OrderByDescending(x => x.BestScore)
                                                   .Take(score.Competition.BestScoresNumber).Sum(x => x.BestScore);
                _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                _context.SaveChanges();
            }

        }
    }
}
