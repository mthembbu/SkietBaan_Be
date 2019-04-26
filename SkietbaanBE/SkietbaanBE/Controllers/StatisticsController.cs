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
        public IActionResult GetUsersLastTwentyScores(string token) {
            if (!ModelState.IsValid) return null;
            bool notValid = context.Users.Where(x => x.Token == token).FirstOrDefault() == null;
            if (notValid) return new BadRequestObjectResult("Invalid Token. Please Logout and Login again");

            if (context.Competitions.Count() == 0)
                return new BadRequestObjectResult("No competitions available at the moment");

            List<GraphData> graphDataList = new List<GraphData>();
            var scores = context.Scores.Where(x => x.User.Token == token);
            DateTime firstScoreDate, lastScoreDate;
            
            foreach (var competition in context.Competitions) {
                var scoreInComp = scores.Where(x => x.Competition.Id == competition.Id);
                GraphData graphData = new GraphData();
                if (scoreInComp.Count() == 0) continue;
                

                graphData.Average = Math.Round(scoreInComp.Sum(x => x.UserScore) / scoreInComp.Count(), 2);
                graphData.Max = scoreInComp.OrderByDescending(x => x.UserScore).FirstOrDefault().UserScore;
                graphData.Min = scoreInComp.OrderBy(x => x.UserScore).FirstOrDefault().UserScore;

                List<Data> dataList = new List<Data>();

                var sorted = scoreInComp.OrderBy(x => x.UserScore);

                scoreInComp = scoreInComp.OrderBy(x => x.UploadDate.Value.Date);
                //take the last 20 scores if they are more than 20
                
                if (scoreInComp.Count() > 20) scoreInComp = scoreInComp.Skip(scoreInComp.ToList().Count() - 20);
                var maxScoreDates = scoreInComp
                .Where(x => x.UserScore == graphData.Max).Select(date => date.UploadDate);
                var minScoreDates = scoreInComp
                                .Where(x => x.UserScore == graphData.Min).Select(date => date.UploadDate);

                firstScoreDate = scoreInComp.First().UploadDate.Value;
                lastScoreDate = scoreInComp.ToList().Skip(scoreInComp.ToList().Count() - 1).First().UploadDate.Value;

                int counter = 0;
                foreach (var score in scoreInComp) {
                    counter++;
                    if (score.UploadDate.Value == firstScoreDate) {
                        Data data = new Data {
                            Label = "1",
                            Value = score.UserScore
                        };

                        if (maxScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                            data.Description = "max";
                        } else if (minScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                            data.Description = "min";
                        } else
                            data.Description = "first";

                        dataList.Add(data);
                        continue;
                    }else if(score.UploadDate.Value == lastScoreDate) {
                        Data data = new Data {
                            Label = scoreInComp.ToList().Count().ToString(),
                            Value = score.UserScore
                        };
                        if (maxScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                            data.Description = "max";
                        } else if (minScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                            data.Description = "min";
                        } else
                            data.Description = "last";
                        dataList.Add(data);
                        continue;
                    }else if(minScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                        Data data = new Data {
                            Label = counter.ToString(),
                            Value = score.UserScore,
                            Description = "min"
                        };
                        dataList.Add(data);
                        continue;
                    }else if(maxScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                        Data data = new Data {
                            Label = counter.ToString(),
                            Value = score.UserScore,
                            Description = "max"
                        };
                        dataList.Add(data);
                        continue;
                    } else {
                        Data data = new Data {
                            Label = "",
                            Value = score.UserScore,
                            Description = ""
                        };
                        dataList.Add(data);
                    }
                }
                graphData.CompetitionName = competition.Name;
                graphData.CompetitionMaximum = competition.MaximumScore;
                graphData.IsParticipating = true;
                graphData.Data = dataList;
                graphDataList.Add(graphData);
            }
            if (graphDataList.Count() == graphDataList.Where(x => x.IsParticipating == false).Count())
                return new OkObjectResult("You haven’t participated in any competitions for any shooting " +
                    "statistics  to be available yet. Time to get shooting!");
            return new OkObjectResult(graphDataList);
        }

        [HttpGet("participants")]
        public Dictionary<string, int> GetUsersPerGroup() {
            Dictionary<string, int> mapGroupToNumberOfUsers = new Dictionary<string, int>();
            var GroupList = context.Groups;

            try {
                foreach (var groups in GroupList) {
                    int count = (from usergroup in context.UserGroups
                                 where usergroup.GroupId == groups.Id
                                 select usergroup.User.Id).Distinct().ToList().Count();

                    mapGroupToNumberOfUsers.Add(groups.Name, count);
                }
                return mapGroupToNumberOfUsers;
            } catch (Exception) {
                return new Dictionary<string, int>();
            }
        }

        [HttpGet("competition-participants")]
        public Dictionary<string, int> GetUsersPerCompetition() {
            Dictionary<string, int> mapCompetitionToNumberOfUsers = new Dictionary<string, int>();
            var competitions = context.Competitions;

            try {
                foreach (var competition in competitions) {
                    int count = (from scores in context.Scores
                                 where scores.Competition.Id == competition.Id
                                 select scores.User.Id).Distinct().ToList().Count();

                    mapCompetitionToNumberOfUsers.Add(competition.Name, count);
                }
                return mapCompetitionToNumberOfUsers;
            } catch (Exception) {
                return new Dictionary<string, int>();
            }
        }
    }
}