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
            try {
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

                    scoreInComp = scoreInComp.OrderBy(x => x.UploadDate);
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
                        } else if (score.UploadDate.Value == lastScoreDate) {
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
                        } else if (minScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
                            Data data = new Data {
                                Label = counter.ToString(),
                                Value = score.UserScore,
                                Description = "min"
                            };
                            dataList.Add(data);
                            continue;
                        } else if (maxScoreDates.FirstOrDefault(x => x.Value == score.UploadDate.Value) != null) {
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
                        "statistics to be available yet. Time to get shooting!");
                return new OkObjectResult(graphDataList);
            } catch (Exception) {
                return null;
            }
        }

        [HttpGet("accuracy-data")]
        public IActionResult GetAccuracyData(string token, string competitionName, string groupName) {
            if (!ModelState.IsValid) return null;
            GraphData graphData = new GraphData();
            List<Data> dataList = new List<Data>();
            Dictionary<int, List<double>> mapAverages = new Dictionary<int, List<double>>();
            try {
                IQueryable<User> usersInGroup = null;
                if (groupName == "overall")
                    usersInGroup = context.UserCompetitionTotalScores.Where(x => x.Competition.Name == competitionName)
                                    .Select(uc => uc.User);
                else
                    usersInGroup = context.UserGroups.Where(x => x.Group.Name == groupName).Select(ug => ug.User);

                Competition competition = context.Competitions.FirstOrDefault(x => x.Name == competitionName);
                
                var scores = context.Scores.Where(x => x.User.Token == token && x.Competition.Name == competitionName);
                var dateOrderedScores = scores.OrderBy(x => x.UploadDate);
                if (scores.Count() > 10) {
                    dateOrderedScores = (IOrderedQueryable<Score>)dateOrderedScores.Skip(dateOrderedScores.ToList().Count() - 10);
                }

                for (int i = 0; i < dateOrderedScores.ToList().Count(); i++) {
                    foreach (var userInGroup in usersInGroup) {
                        var userInGroupScore = context.Scores.Where(x => x.User.Id == userInGroup.Id &&
                                                x.Competition.Name == competitionName).OrderBy(x => x.UploadDate);

                        if (i >= userInGroupScore.ToList().Count()) continue;
                        var score = userInGroupScore.ToList().ElementAt(i);
                        
                        double userInGroupAvg = 0;
                        userInGroupAvg = (score.UserScore / competition.MaximumScore) * 100;

                        if (mapAverages.GetValueOrDefault(i) == null || mapAverages.GetValueOrDefault(i).Count() == 0) {
                            mapAverages.Add(i, new List<double> { userInGroupAvg });
                        } else {
                            var list = mapAverages.GetValueOrDefault(i);
                            list.Add(userInGroupAvg);
                            mapAverages[i] = list;
                        }
                    }
                }

                double max = dateOrderedScores.Take(mapAverages.Count).OrderByDescending(x => x.UserScore).FirstOrDefault().UserScore;
                double min = dateOrderedScores.Take(mapAverages.Count).OrderBy(x => x.UserScore).FirstOrDefault().UserScore;
                
                var maxScoreDates = dateOrderedScores.Take(mapAverages.Count).Where(x => x.UserScore == max)
                                        .Select(u => u.UploadDate);
                var minScoreDates = dateOrderedScores.Take(mapAverages.Count).Where(x => x.UserScore == min)
                                        .Select(u => u.UploadDate);

                graphData.CompetitionName = competitionName;

                List<double> userAvgList = new List<double>();
                for (int i = 0; i < mapAverages.Count; i++) {
                    double avgInGroup = mapAverages.GetValueOrDefault(i).Sum() / usersInGroup.ToList().Count();
                    double userAcc = (dateOrderedScores.ToList().ElementAt(i).UserScore / competition.MaximumScore) * 100;
                    userAvgList.Add(userAcc);
                    Data data = new Data {
                        Value = Math.Round(userAcc, 2),
                        GroupValue = Math.Round(avgInGroup, 2),
                        Label = Convert.ToString(i + 1)
                    };
                    if (maxScoreDates.Contains(dateOrderedScores.ToList().ElementAt(i).UploadDate))
                        data.Description = "max";
                    else if (minScoreDates.Contains(dateOrderedScores.ToList().ElementAt(i).UploadDate))
                        data.Description = "min";
                    else
                        data.Description = "";

                    dataList.Add(data);
                }
                graphData.Max = Math.Round(userAvgList.OrderByDescending(x => x).FirstOrDefault(), 2);
                graphData.Min = Math.Round(userAvgList.OrderBy(x => x).FirstOrDefault(), 2);
                graphData.Average = Math.Round(userAvgList.Sum() / userAvgList.Count(), 2);
                graphData.Data = dataList;

            } catch (Exception e) {
                return new BadRequestObjectResult("There was an error fetching your data");
            }

            return new OkObjectResult(graphData);
        }

        private Dictionary<string, List<Data>> GroupsGraphData(string competitionName, string token) {
            Dictionary<string, List<Data>> mapGroupToData = new Dictionary<string, List<Data>>();
            var groups = context.UserGroups.Where(x => x.User.Token == token).Select(s => s.Group.Name);
            
            foreach (var groupName in groups) {
                var users = context.UserGroups.Where(x => x.Group.Name == groupName).Select(u => u.User);
                var dataList = GroupData(users.ToList(), competitionName);
                mapGroupToData.Add(groupName, dataList);
            }

            return mapGroupToData;
        }

        private List<Data> GroupData(List<User> usersInGroup, string competitionName) {
            List<Data> data = new List<Data>();
            List<double> groupMin = new List<double>();
            List<double> groupMax = new List<double>();
            List<double> groupFirst = new List<double>();
            List<double> groupLast = new List<double>();

            foreach (var user in usersInGroup) {
                var scores = context.Scores.Where(x => x.User.Id == user.Id && x.Competition.Name == competitionName);
                if (scores.Count() == 0) continue;
                double min = -1, max = -1, first = -1, last = -1;
                var dateOrderedScores = scores.OrderBy(x => x.UploadDate);
                if (dateOrderedScores.Count() <= 20) {
                    min = GetMinMax(scores, "min", false);
                    first = GetFirstLast(scores, "first", false);
                    last = GetFirstLast(scores, "last", false);
                    max = GetMinMax(scores, "max", false);
                } else { 
                    min = GetMinMax(scores, "min", true);
                    first = GetFirstLast(scores, "first", true);
                    last = GetFirstLast(scores, "last", true);
                    max = GetMinMax(scores, "max", true);
                }

                groupFirst.Add(first);
                groupLast.Add(last);
                groupMin.Add(min);
                groupMax.Add(max);
            }

            Data avgFirst = new Data {
                Description = "group first",
                Label = "",
                GroupValue = groupFirst.Sum() / groupFirst.Count(),
                Value = -1
            };
            Data avgLast = new Data {
                Description = "group last",
                Label = "",
                GroupValue = groupLast.Sum() / groupLast.Count(),
                Value = -1
            };
            Data avgMin = new Data {
                Description = "group min",
                Label = "",
                GroupValue = groupMin.Sum() / groupMin.Count(),
                Value = -1
            };
            Data avgMax = new Data {
                Description = "group last",
                Label = "",
                GroupValue = groupMax.Sum() / groupMax.Count(),
                Value = -1
            };
            data.Add(avgFirst);
            data.Add(avgLast);
            data.Add(avgMin);
            data.Add(avgMax);
            
            return data;
        }

        private List<Data> OverallData(string competitionName, string token) {
            List<Data> dataList = new List<Data>();
            List<double> overallMin = new List<double>();
            List<double> overallMax = new List<double>();
            List<double> overallFirst = new List<double>();
            List<double> overallLast = new List<double>();
            try {
                var users = context.Users.Where(x => x.Token != token);
                foreach (var user in users) {
                    var scores = context.Scores.Where(x => x.User.Id == user.Id && x.Competition.Name == competitionName);
                    if (scores.Count() > 0) {
                        double min = -1, max = -1, first = -1, last = -1;
                        if (scores.Count() <= 20) {
                            min = GetMinMax(scores, "min", false);
                            first = GetFirstLast(scores, "first", false);
                            last = GetFirstLast(scores, "last", false);
                            max = GetMinMax(scores, "max", false);
                        } else {
                            min = GetMinMax(scores, "min", true);
                            first = GetFirstLast(scores, "first", true);
                            last = GetFirstLast(scores, "last", true);
                            max = GetMinMax(scores, "max", true);
                        }

                        Data avgFirst = new Data {
                            Description = "overall first",
                            Label = "",
                            Value = overallFirst.Sum() / overallFirst.Count()
                        };
                        Data avgLast = new Data {
                            Description = "overall last",
                            Label = "",
                            Value = overallLast.Sum() / overallLast.Count()
                        };
                        Data avgMin = new Data {
                            Description = "overall min",
                            Label = "",
                            Value = overallMin.Sum() / overallMin.Count()
                        };
                        Data avgMax = new Data {
                            Description = "overall last",
                            Label = "",
                            Value = overallMax.Sum() / overallMax.Count()
                        };
                        dataList.Add(avgFirst);
                        dataList.Add(avgLast);
                        dataList.Add(avgMin);
                        dataList.Add(avgMax);
                    }
                }

                return dataList;
            } catch (Exception) {
                return dataList;
            }
        }

        private double GetMinMax(IQueryable<Score> scores, string description, bool lastTwenty) {
            if (!ModelState.IsValid) return -1;
            double value = -2;
            try {
                if(scores.Count() > 0) {
                    var dateOrderedScores = scores.OrderBy(x => x.UploadDate);
                    if (description == "min") {
                        if (lastTwenty) {
                            value = dateOrderedScores.Skip(dateOrderedScores.Count() - 20).OrderBy(s => s.UserScore)
                                    .FirstOrDefault().UserScore;
                        } else {
                            value = dateOrderedScores.OrderBy(x => x.UserScore).FirstOrDefault().UserScore;
                        }
                    } else {
                        if (lastTwenty) {
                            value = dateOrderedScores.Skip(dateOrderedScores.Count() - 20).OrderBy(s => s.UserScore)
                                    .LastOrDefault().UserScore;
                        } else {
                            value = dateOrderedScores.OrderBy(x => x.UserScore).LastOrDefault().UserScore;
                        }
                    }

                    return value;
                } else {
                    return -2; //not a participant of this competition
                }
            } catch (Exception) {
                return -1;
            }
        }

        

        private double GetFirstLast(IQueryable<Score> scores, string description, bool lastTwenty) {
            if (!ModelState.IsValid) return -1;
            double value = -2;
            try {
                if (scores.Count() > 0) {
                    var dateOrderedScores = scores.OrderBy(x => x.UploadDate);
                    if (description == "first") {
                        if (lastTwenty) {
                            value = dateOrderedScores.Skip(dateOrderedScores.Count() - 20).FirstOrDefault().UserScore;
                        } else {
                            value = dateOrderedScores.FirstOrDefault().UserScore;
                        }
                    } else {
                        if (lastTwenty) {
                            value = dateOrderedScores.Skip(dateOrderedScores.Count() - 20).LastOrDefault().UserScore;
                        } else {
                            value = dateOrderedScores.LastOrDefault().UserScore;
                        }
                    }

                    return value;
                } else {
                    return -2; //not a participant of this competition
                }
            } catch (Exception) {
                return -1;
            }
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