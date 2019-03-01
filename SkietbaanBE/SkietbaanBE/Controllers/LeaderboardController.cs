using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[Action]")]
    public class LeaderboardController : Controller
    {
        private ModelsContext _context;
        public LeaderboardController(ModelsContext db)
        {
            _context = db;
        }

        // GET: api/Leaderboard/GetGroups
        [HttpGet]
        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups.ToArray<Group>();
        }
        // GET: api/Leaderboard/
        [HttpGet]
        public IEnumerable<Competition> GetCompetitions()
        {
            return _context.Competitions.ToArray<Competition>();
        }


        [HttpGet]
        public LeaderboardFilterData GetLeaderboardFilterData(int userID)
        {
            LeaderboardFilterData leaderboardFilterData = new LeaderboardFilterData();
            List<Competition> competitions = _context.Competitions.ToList<Competition>();
            List<Group> groups = _context.Groups.ToList<Group>();
            User user = _context.Users.Find(userID);

            List<CompetitionLabel> competitionsLabels = new List<CompetitionLabel>();
            List<GroupLabel> groupsLabels = new List<GroupLabel>();

            
            for (int i = 0; i < competitions.Count; i++)
            {
                CompetitionLabel competition = new CompetitionLabel();
                competition.label = competitions.ElementAt(i).Name;
                competition.value = competitions.ElementAt(i).Id;
                competitionsLabels.Add(competition);
            }

            for (int i = 0; i < groups.Count; i++)
            {
                GroupLabel group = new GroupLabel();
                group.label = groups.ElementAt(i).Name;
                group.value = groups.ElementAt(i).Id;
                groupsLabels.Add(group);
            }
            leaderboardFilterData.competitions1 = competitionsLabels;
            leaderboardFilterData.groups1 = groupsLabels;
            leaderboardFilterData.user = user;

            return leaderboardFilterData;
        }
        [HttpGet]
        public LeaderboardResults GetLeaderboardRankings(int competitionID, int groupID, string userToken)
        {
            //from arry index to data base ID;
            LeaderboardResults leaderboardResults = new LeaderboardResults();
            
            //ranking results for specific group's specific compentition
            List<RankResults> rankResults = new List<RankResults>();       
            if(groupID == -1)//Individual grankings
            {
                rankResults = this.individualRankings(competitionID);
            }
            else//Groups rankings
            {
                competitionID += 1;
                groupID += 1;
                rankResults = this.groupRankings(competitionID,groupID);
            }
            //sort and rank results
            leaderboardResults.RankResults = sortAndRank(rankResults);

            //Current User's results
            User currentUser = new FeaturesController(_context).GetUserByToken(userToken);
            //find current user in ranking results
            if (currentUser != null)
            {
                for (int i = 0; i < leaderboardResults.RankResults.Count; i++)
                {
                    if (leaderboardResults.RankResults.ElementAt(i).Username.Equals(currentUser.Username))
                    {
                        leaderboardResults.UserResults = leaderboardResults.RankResults.ElementAt(i);
                    }
                }
            }
            else
            {
                RankResults rankResult = new RankResults();
                rankResult.Username = "Not logged in";
                rankResult.Rank = 0;
                rankResult.Best = 0;
                rankResult.Total = 0;
                rankResult.Average = 0;
                leaderboardResults.UserResults = rankResult;

            }
            //final results*/
            return leaderboardResults;
        }

        private List<RankResults> sortAndRank(List<RankResults> rankResults)
        {
            rankResults = rankResults.OrderByDescending(x => x.Best).ToList();
            for (int i = 0; i < rankResults.Count; i++)
            {
                rankResults.ElementAt(i).Rank = i + 1;
            }
            return rankResults;
        }

        //Get Users Scores stats for a specific competition
        [HttpGet]
        public IEnumerable<UserCompStats> GetUsersCompetitionsScores(int competitionID)
        {
            List<UserCompStats> userscompStats = new List<UserCompStats>();
            var competitionScoresQuery = from cust in _context.UserCompStats
                                         where cust.Competition.Id == competitionID
                                         select cust;
            userscompStats = competitionScoresQuery.ToList<UserCompStats>();
            return userscompStats;
        }
        //helper methods
        public void calculateTotalAverageCompetitionScore()
        {
            List<User> users = _context.Users.ToList<User>();
            for (int u = 0; u < users.Count; u++)
            {
                //get current user's competition(where the user has scores)
                var competitionIDsQuery = from cust in _context.Scores
                                          where cust.User.Id == users.ElementAt(u).Id
                                          select cust.Competition.Id;
                List<int> competitionsIDs = competitionIDsQuery.ToList<int>();
                for (int c = 0; c < competitionsIDs.Count; c++)
                {
                    //get user competition total score
                    var competitionScoresQuery = from cust in _context.Scores
                                                 where (cust.User.Id == users.ElementAt(u).Id && cust.Competition.Id == competitionsIDs.ElementAt(c))
                                                 select cust.UserScore;
                    List<int> competitionScores = competitionScoresQuery.ToList<int>();
                    //calculate average
                    int total = competitionScores.Sum();
                    //calculate average
                    double average = (double)total / (double)competitionScores.Count;

                    //get competition object
                    Competition competition = _context.Competitions.Find(competitionsIDs.ElementAt(c));

                    //update
                    var userCompStats = _context.UserCompStats.Where(us => us.User.Id == users.ElementAt(u).Id && us.Competition.Id == competitionsIDs.ElementAt(c))
                                                  .FirstOrDefault<UserCompStats>();
                    //userCompStats.Total = total;
                    userCompStats.User = users.ElementAt(u);
                    //save
                    _context.UserCompStats.Update(userCompStats);
                    _context.SaveChanges();
                }
            }
        }
        private List<RankResults> individualRankings(int competitionID)
        {
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        join UserCompStats in _context.UserCompStats on User.Id equals UserCompStats.User.Id
                        join Competition in _context.Competitions on UserCompStats.Competition.Id equals Competition.Id
                        join UserCompetitionTotalScore in _context.UserCompetitionTotalScores on User.Id equals UserCompetitionTotalScore.User.Id
                        where (UserCompStats.Competition.Id == competitionID)
                        select new {
                            User.Username,
                            User.Id,
                            UserCompetitionTotalScore.Average,
                            UserCompetitionTotalScore.Total,
                            UserCompStats.Best
                        };
            //saving results in an List which will make sorting easier(ArrayList)
            Dictionary<int, RankResults> mapIdToBest = new Dictionary<int, RankResults>();

            foreach (var item in query) {
                RankResults rankResult = new RankResults();
                rankResult.Username = item.Username;
                rankResult.Best = item.Best;
                rankResult.Total = item.Total;
                rankResult.Average = item.Average;

                if (mapIdToBest.ContainsKey(item.Id)) {
                    RankResults res = mapIdToBest.GetValueOrDefault(item.Id);
                    if (res.Best < item.Best) {
                        res.Best = item.Best;
                        mapIdToBest[item.Id] = res;
                    }
                } else {
                    mapIdToBest.Add(item.Id, rankResult);
                }
            }

            return mapIdToBest.Values.ToList();
        }
        private List<RankResults> groupRankings(int competitionID, int groupID)
        {
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        join UserCompStats in _context.UserCompStats on User.Id equals UserCompStats.User.Id
                        join Competition in _context.Competitions on UserCompStats.Competition.Id equals Competition.Id
                        join UserCompetitionTotalScore in _context.UserCompetitionTotalScores on User.Id equals UserCompetitionTotalScore.User.Id
                        where (UserCompStats.Competition.Id == competitionID && Group.Id == groupID)
                        select new {
                            User.Username,
                            User.Id,
                            UserCompetitionTotalScore.Average,
                            UserCompetitionTotalScore.Total,
                            UserCompStats.Best
                        };
            //saving results in an List which will make sorting easier(ArrayList)
            Dictionary<int, RankResults> mapIdToBest = new Dictionary<int, RankResults>();

            foreach (var item in query) {
                RankResults rankResult = new RankResults();
                rankResult.Username = item.Username;
                rankResult.Best = item.Best;
                rankResult.Total = item.Total;
                rankResult.Average = item.Average;

                if (mapIdToBest.ContainsKey(item.Id)) {
                    RankResults res = mapIdToBest.GetValueOrDefault(item.Id);
                    if (res.Best < item.Best) {
                        res.Best = item.Best;
                        mapIdToBest[item.Id] = res;
                    }
                } else {
                    mapIdToBest.Add(item.Id, rankResult);
                }
            }

            return mapIdToBest.Values.ToList();
        }
    }
}
