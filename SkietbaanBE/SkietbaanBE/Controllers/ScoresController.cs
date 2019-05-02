using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Lib;
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

        // GET: api/Scores
        [HttpGet("{id}/{token}")]
        public ActionResult GetScores(string id, string token)
        {
            Competition compId = _context.Competitions.FirstOrDefault(x => x.Id.ToString().Equals(id));
            if(compId != null)
            {
                User userId = _context.Users.FirstOrDefault(x => x.Token.Equals(token));
                if(userId != null)
                {
                    var scoreList = _context.Scores.Where(y => y.User.Id == userId.Id && y.Competition.Id == compId.Id);
                    return Json(scoreList.ToArray());
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            else
            {
                return NotFound("Competition not found");
            }

        }
        [Route("api/Scores/DeleteScoresById")]
        [HttpPost("{id}")]
        public async System.Threading.Tasks.Task DeleteScoresByIdAsync([FromBody] List<Score> list)
        {
             for(int i =0;i<list.Count;i++)
             {
                try
                {
                    Score tempScore = _context.Scores.Where(s => s.Id == list.ElementAt(i).Id).FirstOrDefault<Score>();
                    if(tempScore != null)
                    {
                        _context.Scores.Remove(tempScore);
                      
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e) { }
        }

        [HttpGet("GetUsers/{compId}")]
        public ActionResult GetUsers(int compId)
        {
            var query = from competition in _context.Competitions
                        join score in _context.Scores on competition.Id equals score.Competition.Id
                        join user in _context.Users on score.User.Id equals user.Id
                        where competition.Id == compId
                        select new
                        {
                            user.Username,
                            user.Email,
                            user.Token,
                        };

            if(query != null)
            {
                var scoresList = query.Distinct().ToList();
                return Json(scoresList);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult ScoreCapture([FromBody]ScoreCapture scoreCapture)
        {
            if (ModelState.IsValid)
            {
                var competition = _context.Competitions.Where(x => x.Name == scoreCapture.CompetitionName).FirstOrDefault<Competition>();
                if (competition == null)
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

                //update time spent at skietbaan
                CheckAward.UpdateHoursSpent(_context, score);

                //update User Competition Stats
                Calculations calculations = new Calculations(_context);
                calculations.performCalculations(user.Id, competition.Id);
                CheckAward.UpdateAccuracyAndTotalAward(score, _context);
                return Ok("Score Added Successfully");
            }
            else
            {
                return new BadRequestObjectResult("score cannot be null");
            }
        }
    }
}