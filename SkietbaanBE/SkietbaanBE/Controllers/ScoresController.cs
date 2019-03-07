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
                var user = _context.Users.Where(x => x.Token == scoreCapture.Token).FirstOrDefault<User>();
                if (user == null)
                {
                    return new NotFoundObjectResult("User not found");
                }
                var score = new Score() {
                    UserScore = scoreCapture.UserScore,
                    PictureURL = scoreCapture.PictureURL,
                    Competition = competition,
                    User = user,
                    Latitude = scoreCapture.Latitude,
                    Longitude = scoreCapture.Longitude,
                    UploadDate = DateTime.Now
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

        public void UpdateUserCompStats(Score score) {  
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                            ucs.Competition.Id == score.Competition.Id &&
                                            ucs.Month == score.UploadDate.Value.Month &&
                                            ucs.Year == score.UploadDate.Value.Year);

            if (userCompStatsRecords.Count() < 1) {
                UserCompStats userCompStats = new UserCompStats();
                userCompStats.Competition = score.Competition;
                userCompStats.User = score.User;    
                userCompStats.MonthBestScore = score.UserScore;
                userCompStats.Month = score.UploadDate.Value.Month;
                userCompStats.Year = score.UploadDate.Value.Year;

                _context.UserCompStats.Add(userCompStats);
                //update overal best score
                UserCompetitionTotalScore overallStats = _context.UserCompetitionTotalScores.Where(ucs => ucs.Competition.Id == score.Competition.Id && ucs.User.Id == score.User.Id).FirstOrDefault<UserCompetitionTotalScore>();
                if(overallStats == null) {
                    overallStats = new UserCompetitionTotalScore();
                    overallStats.Best = score.UserScore;
                    _context.UserCompetitionTotalScores.Add(overallStats);
                 } else {
                      overallStats.Best = score.UserScore;
                      _context.UserCompetitionTotalScores.Update(overallStats);
                 }
            }
            else {
                var userCompStats = userCompStatsRecords.First();
                if (userCompStats.MonthBestScore < score.UserScore) {
                    userCompStats.MonthBestScore = score.UserScore;
                    _context.UserCompStats.Update(userCompStats);
                    //update overal best score
                    UserCompetitionTotalScore overallStats = _context.UserCompetitionTotalScores.Where(ucs => ucs.Competition.Id == score.Competition.Id && ucs.User.Id == score.User.Id).FirstOrDefault<UserCompetitionTotalScore>();
                    overallStats.Best = score.UserScore;
                    _context.UserCompetitionTotalScores.Update(overallStats);
                }
            }

            _context.SaveChanges();
        }

        public void UpdateTotal(Score score) {
            int bestOf = score.Competition.BestScoresNumber;
            var allScore = _context.Scores.Where(x => x.User.Id == score.User.Id && x.Competition.Id == score.Competition.Id);
            double average;
            if (allScore.Count() == 0) {
                average = (double)score.UserScore;
            } else {
                average = (double)allScore.Sum(X => X.UserScore) / (double)allScore.Count();
            }
            
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                        ucs.Competition.Id == score.Competition.Id);
            if(userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() <= bestOf) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                if(userCompetitionTotalScore == null) {
                    userCompetitionTotalScore = new UserCompetitionTotalScore {
                        Competition = score.Competition,
                        User = score.User,
                        Total = userCompStatsRecords.Sum(ucs => ucs.MonthBestScore),
                        Average = average
                    };
                     
                    _context.Add(userCompetitionTotalScore);
                } else {
                    //MAKE TOTAL A DOUBLE
                    userCompetitionTotalScore.Total = userCompStatsRecords.Sum(ucs => ucs.MonthBestScore) / userCompStatsRecords.Count();
                    userCompetitionTotalScore.Average = average;
                    _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                }
                _context.SaveChanges();
            } else if(userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() > bestOf) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                userCompetitionTotalScore.Total = userCompStatsRecords.OrderByDescending(x => x.MonthBestScore)
                                                   .Take(bestOf).Sum(x => x.MonthBestScore) / score.Competition.BestScoresNumber;
                userCompetitionTotalScore.Average = average;
                _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                _context.SaveChanges();
            }
        }
    }
}
