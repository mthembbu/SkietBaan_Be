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
        [HttpGet]
        [HttpPost]
        public IActionResult ScoreCapture([FromBody]ScoreCapture scoreCapture)
        {
            if (ModelState.IsValid)
            {
                Competition competition = _context.Competitions.Where(x => x.Name == scoreCapture.CompetitionName).FirstOrDefault<Competition>();
                User user = _context.Users.Where(x => x.Username == scoreCapture.Username).FirstOrDefault<User>();

                var score = new Score()
                {
                    UserScore = scoreCapture.UserScore,
                    PictureURL = scoreCapture.PictureURL,
                    Competition = competition,
                    User = user,
                    UploadDate = DateTime.Today
                };
                _context.Scores.Add(score);
                _context.SaveChanges();
                return Ok("Score Added Successfully");
            }
            else
            {
                return new BadRequestObjectResult("score cannout be null");
            }
            
        }
    }
}
