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
            string membershipID = context.Users.Where(x => x.Id == 1).First().MemberID;
            string username = context.Users.Where(x => x.Id == 1).First().Username;

            var competitionsUserPartakesIn = from UserCompetitionTotalScore in context.UserCompetitionTotalScores
                                             where (UserCompetitionTotalScore.User.Token == token)
                                             select new {
                                                 UserCompetitionTotalScore.Competition,
                                                 UserCompetitionTotalScore.Average,
                                                 UserCompetitionTotalScore.Total
                                             };
            foreach(var comp in context.Competitions) {
                if(competitionsUserPartakesIn.Where(x => x.Competition.Id == comp.Id).Count() != 0) {
                    AwardObject awardObject = new AwardObject {
                        CompetitionName = comp.Name,
                        IsCompetitionLocked = false,
                        Total = GetTotalScore(token, comp.Name),
                        //REMOVE
                        Accuracy = (int)competitionsUserPartakesIn
                                        .Where(x => x.Competition.Id == comp.Id).First().Average,
                        MembershipNumber = membershipID,
                        Username = username,
                        HoursAward = CheckAward.Hours(),
                        TotalAward = CheckAward.Total(competitionsUserPartakesIn
                                        .Where(x => x.Competition.Id == comp.Id).First().Total, false),
                        AccuracyAward = CheckAward.Accuracy(((int)competitionsUserPartakesIn
                                        .Where(x => x.Competition.Id == comp.Id).First().Average), false)
                    };
                    
                    awardCompetitions.Add(awardObject);
                } else {
                    AwardObject awardObject = new AwardObject {
                        CompetitionName = comp.Name,
                        IsCompetitionLocked = true,
                        Total = "0",
                        Accuracy = 0,
                        MembershipNumber = membershipID,
                        Username = username,
                        HoursAward = CheckAward.Hours(),
                        TotalAward = CheckAward.Total(0, true),
                        AccuracyAward = CheckAward.Accuracy(0, true)
                    };
                    awardCompetitions.Add(awardObject);
                }
            }

            return awardCompetitions;
        }
    }
}