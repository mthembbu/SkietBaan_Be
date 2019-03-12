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
            List<Competition> competitions = _context.Competitions.Where(c=>c.Status == true).ToList<Competition>();
            List<Group> groups = _context.Groups.Where(c => c.IsActive == true).ToList<Group>();
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
            if (groupID == -1)//Individual grankings
            {
                rankResults = this.individualRankings(competitionID);
            }
            else//Groups rankings
            {
                rankResults = this.groupRankings(competitionID, groupID);
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
                if (leaderboardResults.UserResults == null)
                {
                    leaderboardResults.UserResults = new RankResults()
                    {
                        Username = currentUser.Username,
                        Best = 0,
                        Total = 0,
                        Average = 0,
                        Rank = 0
                    };
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
           List<RankResults> results = rankResults.OrderByDescending(x => x.Best).ToList();
            for(int i = 0; i < results.Count; i++)
            {
                results.ElementAt(i).Rank = i + 1;
            }
            return results;
        }
        private List<RankResults> individualRankings(int competitionID)
        {
            //need to update with Outer join query
            var query = from User in _context.Users
                        join UserCompetitionTotalScore in _context.UserCompetitionTotalScores on User.Id equals UserCompetitionTotalScore.User.Id
                        where (UserCompetitionTotalScore.Competition.Id == competitionID)
                        select new
                        {
                            User.Username,
                            UserCompetitionTotalScore.Average,
                            UserCompetitionTotalScore.Total,
                            UserCompetitionTotalScore.Best
                        };
            var queryAllCustomers = from user in _context.Users
                                    select user.Username;
            List<string> users = queryAllCustomers.ToList<string>();

            //saving results in an List which will make sorting easier(ArrayList)
            List<RankResults> ranklist = new List<RankResults>();
            foreach (var item in query)
            {
                RankResults rankResult = new RankResults();
                rankResult.Username = item.Username;
                rankResult.Best = item.Best;
                rankResult.Total = item.Total;
                rankResult.Average = item.Average;
                rankResult.Rank = 0;
                ranklist.Add(rankResult);
                //remove user from users without scores
                users.Remove(item.Username);
            }
            for (int i = 0; i < users.Count; i++)
            {
                RankResults tempRankResult = new RankResults();
                tempRankResult.Username = users.ElementAt(i);
                tempRankResult.Total = 0;
                tempRankResult.Best = 0;
                tempRankResult.Average = 0;
                tempRankResult.Rank = 0;
                ranklist.Add(tempRankResult);
            }
            return ranklist;
        }
        private List<RankResults> groupRankings(int competitionID, int groupID)
        {
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        join UserCompetitionTotalScore in _context.UserCompetitionTotalScores on User.Id equals UserCompetitionTotalScore.User.Id
                        join Competition in _context.Competitions on UserCompetitionTotalScore.Competition.Id equals Competition.Id
                        where (UserCompetitionTotalScore.Competition.Id == competitionID && Group.Id == groupID)
                        select new
                        {
                            User.Username,
                            UserCompetitionTotalScore.Average,
                            UserCompetitionTotalScore.Total,
                            UserCompetitionTotalScore.Best
                        };
            //saving results in an List which will make sorting easier(ArrayList)
            int rank = 1;
            List<RankResults> ranklist = new List<RankResults>();
            foreach (var item in query)
            {
                RankResults rankResult = new RankResults();
                rankResult.Username = item.Username;
                rankResult.Best = item.Best;
                rankResult.Total = item.Total;
                rankResult.Average = item.Average;
                rankResult.Rank = rank;
                ranklist.Add(rankResult);
                rank++;
            }
            //rank and return results
            return ranklist;
        }
    }
}
