using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Awards")]
    public class AwardsController : Controller
    {
        private ModelsContext context;

        public AwardsController(ModelsContext context) {
            this.context = context;
        }

        [HttpGet("total")]
        public string GetTotalScore(string token, string competitionName) {
            string total;
            try {
                total = Convert.ToString(context.UserCompetitionTotalScores
                            .Where(x => x.Competition.Name == competitionName && x.User.Token == token).FirstOrDefault().Total);
            } catch (Exception) {
                return "0";
            }

            return total;
        }

        [HttpGet("{token}")]
        public List<AwardObject> GetAllAwards(string token) {
            List<AwardObject> awardCompetitions = new List<AwardObject>();
            bool notValid = context.Users.Where(x => x.Token == token).FirstOrDefault() == null;
            if (notValid) return awardCompetitions;

            var competitionsUserPartakesIn = from UserCompetitionTotalScore in context.UserCompetitionTotalScores
                                             where (UserCompetitionTotalScore.User.Token == token)
                                             select new {
                                                 UserCompetitionTotalScore.Competition,
                                                 UserCompetitionTotalScore.Average,
                                                 UserCompetitionTotalScore.Total
                                             };
            foreach (var comp in context.Competitions) {
                try {
                    if (competitionsUserPartakesIn.Where(x => x.Competition.Id == comp.Id).Count() != 0) {
                        AwardObject awardObject = new AwardObject {
                            CompetitionName = comp.Name,
                            IsCompetitionLocked = false,
                            Total = GetTotalScore(token, comp.Name),
                            Accuracy = context.Scores.Sum(x => x.UserScore) / (double)comp.MaximumScore,
                            TotalAward = CheckAward.Total(competitionsUserPartakesIn
                                            .Where(x => x.Competition.Id == comp.Id).First().Total, false, comp.Name, context),
                            AccuracyAward = CheckAward.Accuracy(((int)competitionsUserPartakesIn
                                            .Where(x => x.Competition.Id == comp.Id).First().Average), false, comp.Name, context),
                            BestInMonth = CheckAward.MonthBest(comp.Id, token, context).ToString()
                        };

                        awardCompetitions.Add(awardObject);
                    } else {
                        AwardObject awardObject = new AwardObject {
                            CompetitionName = comp.Name,
                            IsCompetitionLocked = true,
                            Total = "0",
                            Accuracy = 0,
                            TotalAward = CheckAward.Total(0, true, comp.Name, context),
                            AccuracyAward = CheckAward.Accuracy(0, true, comp.Name, context),
                            BestInMonth = "No Award"
                        };
                        awardCompetitions.Add(awardObject);
                    }
                } catch {
                    return null;
                }
            }

            return awardCompetitions;
        }

        [HttpGet("hours/{token}")]
        public HoursAward GetHours(string token) {
            return CheckAward.Hours(token, context);
        }
    }
}