﻿using Microsoft.AspNetCore.Mvc;
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
            if (!ModelState.IsValid) return null;
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
        public IActionResult GetAllAwards(string token) {
            if (!ModelState.IsValid) return null;

            List<AwardObject> awardCompetitions = new List<AwardObject>();
            bool notValid = context.Users.Where(x => x.Token == token).FirstOrDefault() == null;
            if (notValid) return new BadRequestObjectResult("Invalid Token. Please Logout and Login again");
            if (context.Competitions.Count() == 0)
                return new BadRequestObjectResult("No competitions available at the moment");
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
                        double sum = context.Scores
                            .Where(x => x.User.Token ==  token && x.Competition.Id == comp.Id)
                            .Sum(sc => sc.UserScore);
                        int numberOfScores = context.Scores
                                    .Where(x => x.Competition.Id == comp.Id && x.User.Token == token)
                                    .Count();
                        double accuracy = Math.Round((sum / (numberOfScores * comp.MaximumScore)) * 100, 1);
                        double total = competitionsUserPartakesIn
                                            .Where(x => x.Competition.Id == comp.Id).First().Total;
                        AwardObject awardObject = new AwardObject {
                            CompetitionName = comp.Name,
                            IsCompetitionLocked = false,
                            Total = GetTotalScore(token, comp.Name),
                            Accuracy = accuracy,
                            TotalAward = CheckAward.Total(total, false, token, comp.Name, context),
                            AccuracyAward = CheckAward.Accuracy(accuracy, false, token, comp.Name, context),
                            BestInMonth = CheckAward.MonthBest(comp.Id, token, context).ToString()
                        };

                        awardCompetitions.Add(awardObject);
                    } else {
                        AwardObject awardObject = new AwardObject {
                            CompetitionName = comp.Name,
                            IsCompetitionLocked = true,
                            Total = "0",
                            Accuracy = 0,
                            TotalAward = CheckAward.Total(0, true, token, comp.Name, context),
                            AccuracyAward = CheckAward.Accuracy(0, true, token, comp.Name, context),
                            BestInMonth = "No Award"
                        };
                        awardCompetitions.Add(awardObject);
                    }
                } catch {
                    if(context.Competitions.Count() != 0) {
                        foreach(Competition competition in context.Competitions) {
                            AwardObject awardObject = new AwardObject {
                                CompetitionName = comp.Name,
                                IsCompetitionLocked = true,
                                Total = "0",
                                Accuracy = 0,
                                TotalAward = CheckAward.Total(0, true, token, comp.Name, context),
                                AccuracyAward = CheckAward.Accuracy(0, true, token, comp.Name, context),
                                BestInMonth = "No Award"
                            };
                            awardCompetitions.Add(awardObject);
                        }
                        return new OkObjectResult(awardCompetitions);
                    }
                    return null;
                }
            }
            return new OkObjectResult(awardCompetitions);
        }

        [HttpGet("hours/{token}")]
        public HoursAward GetHours(string token) {
            try {
                if (!ModelState.IsValid) return null;
                return CheckAward.Hours(token, context);
            } catch (Exception) {
                return null;
            }
            

        }
    }
}