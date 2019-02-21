using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
                var score = new Score()
                {
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

                UpdateUserCompStats(user, score, competition);
                return Ok("Score Added Successfully");
            }
            else
            {
                return new BadRequestObjectResult("score cannot be null");
            }
            
        }

        public void UpdateUserCompStats(User user, Score score, Competition competition) {
            
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == user.Id &&
                                            ucs.Month == score.UploadDate.Value.Month);

            if (userCompStatsRecords.Count() < 1) {
                UserCompStats userCompStats = new UserCompStats();
                userCompStats.Competition = competition;
                userCompStats.User = user;

                _context.UserCompStats.Add(userCompStats);
                _context.SaveChanges();
            } else {
                var userCompStats = userCompStatsRecords.First();
                if (userCompStats.BestScore < score.UserScore) {
                    userCompStats.BestScore = score.UserScore;
                    _context.UserCompStats.Add(userCompStats);
                    _context.SaveChanges();
                }
            }
        }
    }
}
