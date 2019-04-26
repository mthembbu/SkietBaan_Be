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
        public List<GraphData> GetUsersLastTwentyScores(string token) {

            List<GraphData> graphDataList = new List<GraphData>();
            var scores = context.Scores.Where(x => x.User.Token == token);
            DateTime maxScoreDate, minScoreDate, firstScoreDate, lastScoreDate;
            
            foreach (var competition in context.Competitions) {
                var scoreInComp = scores.Where(x => x.Competition.Id == competition.Id);
                if (scoreInComp.Count() == 0) continue;

                GraphData graphData = new GraphData();
                
                List<Data> dataList = new List<Data>();
                var sorted = scoreInComp.OrderBy(x => x.UserScore);
                maxScoreDate = sorted.Last().UploadDate.Value;
                minScoreDate = sorted.First().UploadDate.Value;
                scoreInComp = scoreInComp.OrderBy(x => x.UploadDate.Value.Date);
                firstScoreDate = scoreInComp.First().UploadDate.Value;
                lastScoreDate = scoreInComp.Skip(scoreInComp.Count() - 1).First().UploadDate.Value;

                int counter = 0;
                foreach (var score in scoreInComp) {
                    counter++;
                    if(score.UploadDate.Value == firstScoreDate) {
                        Data data = new Data {
                            Label = "1",
                            Value = score.UserScore
                        };
                        if (score.UploadDate.Value == maxScoreDate)
                            data.Description = "max";
                        else if (score.UploadDate.Value == minScoreDate)
                            data.Description = "min";
                        else
                            data.Description = "first";
                        dataList.Add(data);
                        continue;
                    }else if(score.UploadDate.Value == lastScoreDate) {
                        Data data = new Data {
                            Label = scoreInComp.Count().ToString(),
                            Value = score.UserScore
                        };
                        if (score.UploadDate.Value == maxScoreDate)
                            data.Description = "max";
                        else if (score.UploadDate.Value == minScoreDate)
                            data.Description = "min";
                        else
                            data.Description = "last";
                        dataList.Add(data);
                        continue;
                    }else if(score.UploadDate.Value == minScoreDate) {
                        Data data = new Data {
                            Label = counter.ToString(),
                            Value = score.UserScore,
                            Description = "min"
                        };
                        dataList.Add(data);
                        continue;
                    }else if(score.UploadDate.Value == maxScoreDate) {
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
                graphData.Data = dataList;
                graphDataList.Add(graphData);
            }

            return graphDataList;
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

        /*[HttpGet("competition-participants")]
        public Dictionary<string, int> GetUsersPerCompetition() {
            Dictionary<string, int> mapCompetitionToNumberOfUsers = new Dictionary<string, int>();
            var competitions = context.Competitions;

            try {
                foreach (var competition in competitions) {
                    int count = (from usergroup in context.UserGroups
                                 where usergroup.GroupId == groups.Id
                                 select usergroup.User.Id).Distinct().ToList().Count();

                    mapGroupToNumberOfUsers.Add(groups.Name, count);
                }
                return mapGroupToNumberOfUsers;
            } catch (Exception) {
                return new Dictionary<string, int>();
            }
        }*/
    }
}