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
        [HttpGet("{token}")]
        public Array GetScores(string token)
        {
            User userid = _context.Users.FirstOrDefault(x => x.Token.Equals(token));
            var scoreList = _context.Scores.Where(x => x.User.Id == userid.Id);
            return scoreList.ToArray();
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