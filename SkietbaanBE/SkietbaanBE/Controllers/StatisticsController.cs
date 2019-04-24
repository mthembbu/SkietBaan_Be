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
    [Route("api/Statistics")]
    public class StatisticsController : Controller
    {
        private ModelsContext context;

        public StatisticsController(ModelsContext context) {
            this.context = context;
        }

        [HttpGet("{token}")]
        public List<UserLastTwentyScores> GetUsersLastTwentyScores(string token) {
            List<UserLastTwentyScores> userLastTwentyScores = new List<UserLastTwentyScores>();
            var scores = context.Scores.Where(x => x.User.Token == token);

            foreach (var competition in context.Competitions) {
                var scoreInComp = scores.Where(x => x.Competition.Id == competition.Id);
                if (scoreInComp.Count() == 0) continue;
                scoreInComp = scoreInComp.OrderBy(x => x.UploadDate.Value.Date);
                UserLastTwentyScores lastTwentyScores = new UserLastTwentyScores {
                    CompetitionName = competition.Name
                };
                if (scoreInComp.Count() > 20)
                    lastTwentyScores.Scores = scoreInComp.Select(x => x.UserScore).Skip(scoreInComp.Count() - 20).ToList();
                else
                    lastTwentyScores.Scores = scoreInComp.Select(x => x.UserScore).ToList();
                userLastTwentyScores.Add(lastTwentyScores);
            }

            return userLastTwentyScores;
        }
    }
}