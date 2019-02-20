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

            List<Competition1> competitions1s = new List<Competition1>();
            List<Group1> groups1s = new List<Group1>();

            leaderboardFilterData.competitions = competitions;
            for (int i = 0; i < competitions.Count; i++)
            {
                Competition1 competition1 = new Competition1();
                competition1.label = competitions.ElementAt(i).Name;
                competition1.value = competitions.ElementAt(i).Id;
                competitions1s.Add(competition1);
            }
            leaderboardFilterData.groups = groups;
            for (int i = 0; i < groups.Count; i++)
            {
                Group1 group1 = new Group1();
                group1.label = groups.ElementAt(i).Name;
                group1.value = groups.ElementAt(i).Id;
                groups1s.Add(group1);
            }
            leaderboardFilterData.competitions1 = competitions1s;
            leaderboardFilterData.groups1 = groups1s;
            leaderboardFilterData.user = user;

            return leaderboardFilterData;
        }
        [HttpGet]
        public List<RankResults> GetLeaderboardRankings(int competitionID, int groupID, string ScoreType)
        {
            List<RankResults> rankResults = new List<RankResults>();
            //
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        join UserCompStats in _context.UserCompStats on User.Id equals UserCompStats.User.Id
                        where (UserCompStats.Competition.Id == competitionID && Group.Id == (groupID))
                        //orderby UserCompStats.CompScore
                        select new
                        {
                            User.Username,
                            //UserCompStats.Total,
                           // UserCompStats.CompScore,
                            UserCompStats.BestScore
                        };
            //int rank = 1;
            foreach (var item in query)
            {
                RankResults rankResult = new RankResults();


                rankResult.Username = item.Username;
                switch (ScoreType.ToUpper())
                {
                    case "1":  //AVERAGE
                        //rankResult.Value = item.CompScore;
                        break;
                    case "2": //TOTAL
                        //rankResult.Value = item.Total;
                        break;
                    case "3": //BEST
                        rankResult.Value = item.BestScore;
                        break;
                    default:
                        break;
                }
                rankResults.Add(rankResult);
            }

            return sortAndRank(rankResults);
        }

        private List<RankResults> sortAndRank(List<RankResults> rankResults)
        {
            rankResults = rankResults.OrderByDescending(x => x.Value).ToList();
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


        //TestData
        [HttpGet]
        public void insertTestData()
        {

            new TestData().AddTestData(_context);
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
    }
}
