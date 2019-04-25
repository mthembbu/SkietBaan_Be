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
            try
            {
                return _context.Groups.ToArray<Group>();
            }
            catch(Exception e)
            {
                return new List<Group>();
            }
        }
        // GET: api/Leaderboard/
        [HttpGet]
        public IEnumerable<Competition> GetCompetitions()
        {
            try
            {
                return _context.Competitions.ToArray<Competition>();
            }catch(Exception e)
            {
                return new List<Competition>();
            }
        }


        [HttpGet]
        public LeaderboardFilterData GetLeaderboardFilterData(string token)
        {
            if(token == null)
            {
                token = "";
            }
            LeaderboardFilterData leaderboardFilterData = new LeaderboardFilterData();
            List<Competition> competitions = null;
            List<Group> groups = null;
            User user = null;
            try
            {
                competitions = _context.Competitions.Where(c => c.Status == true).ToList<Competition>();
                groups = _context.Groups.Where(c => c.IsActive == true).ToList<Group>();
            }
            catch(Exception e)
            {
                if(competitions == null) { competitions = new List<Competition>(); }
                if (groups == null) { groups = new List<Group>(); }
            }
            finally
            {
                try
                {
                    user = _context.Users.Where(u => u.Token.Equals(token)).FirstOrDefault<User>();
                    if(user == null)
                    {
                        user = new User();
                        user.Username = "Invalid Tokken";
                    }
                }
                catch(Exception e)
                {
                    user = new User();
                    user.Username = "Invalid Tokken";
                }
            }

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
            leaderboardFilterData.rankResult = new RankResults() {
                Rank = 0,
                Username = user.Username,
                DisplayName = getDisplayName(user.Name,user.Surname),
                Best = 0,
                Total = 0,
                Average = 0,
                RankStatus = "equal",
                isCompetitiveShooter = false
            };
            if (!user.Username.Equals("Invalid Tokken"))
            {
                if(user.MemberID != null)
                {
                    if(user.MemberID.Length > 0)
                    {
                        leaderboardFilterData.rankResult.isMember = true;

                    }
                }
                string displayName = getDisplayName(user.Name, user.Surname);
                if(displayName != null)
                {
                    if (displayName.Length > 0)
                    {
                        leaderboardFilterData.user.Username = displayName;
                    }
                }
            }

            return leaderboardFilterData;
        }
        [HttpGet]
        public string CalcAll() {

            List<UserCompetitionTotalScore> stats = _context.UserCompetitionTotalScores.ToList<UserCompetitionTotalScore>();
            for(int i = 0; i < stats.Count; i++) {

                new Calculations(_context).
                                    performCalculations(stats.ElementAt(i).UserId, stats.ElementAt(i).CompetitionId);

            }
            return "success";
        }
        [HttpGet]
        public LeaderboardResults GetLeaderboardRankings(int competitionID, int groupID, string userToken,string selectedRank)
        {
            List<Competition> comps = _context.Competitions.Where(c => c.Status == true).ToList<Competition>();
            if(competitionID == -1)
            {
                if (comps.Count() > 0)
                {
                    competitionID = comps.ElementAt(0).Id;
                }
            }
            //from arry index to data base ID;
            LeaderboardResults leaderboardResults = new LeaderboardResults();
            try
            {
                List<Competition> competitions = _context.Competitions.Where(c => c.Status == true).ToList<Competition>();
                if(competitions != null)
                {
                    if(competitions.Count == 0)
                    {
                        leaderboardResults.RankResults = new List<RankResults>();
                        leaderboardResults.UserResults = new RankResults() {
                            Rank = 0,
                            Username = "",
                            DisplayName = "",
                            Best = 0,
                            Total = 0,
                            Average = 0,
                            RankStatus = "equal",
                            isCompetitiveShooter = false,
                            isMember = false,
                        };
                        return leaderboardResults;
                    }
                }
            }catch(Exception e)
            {
                leaderboardResults.RankResults = new List<RankResults>();
                return leaderboardResults;
            }

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
            leaderboardResults.RankResults = sortAndRank(rankResults, selectedRank);

            //Current User's results
            User currentUser = new FeaturesController(_context, null).GetUserByToken(userToken);
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
                        Rank = 0,
                        RankStatus = "equal",
                    };
                }
            }
            //final results*/
            return leaderboardResults;
        }

        private List<RankResults> sortAndRank(List<RankResults> rankResults,string ranking)
        {
            List<RankResults> results = rankResults;
            switch (ranking) {
                case "total":
                    results = rankResults.OrderByDescending(x => x.Total).ToList();
                    break;
                case "average":
                    results = rankResults.OrderByDescending(x => x.Average).ToList();
                    break;
                case "best":
                    results = rankResults.OrderByDescending(x => x.Best).ToList();
                    break;
                default:
                    results = rankResults.OrderByDescending(x => x.Best).ToList();
                    break;
            }

            for (int i = 0; i < results.Count; i++)
            {
                results.ElementAt(i).Rank = i + 1;
                if(results.ElementAt(i).Best == 0)
                {
                    results.ElementAt(i).Rank = 0;
                }
            }
            return results;
        }
        private List<RankResults> individualRankings(int competitionID)
        {
            //need to update with Outer join query
            try
            {
                var query = from User in _context.Users
                            join UserCompetitionTotalScore in _context.UserCompetitionTotalScores on User.Id equals UserCompetitionTotalScore.User.Id
                            where (UserCompetitionTotalScore.Competition.Id == competitionID)
                            select new
                            {
                                User.Username,
                                User.Name,
                                User.Surname,
                                User.MemberID,
                                UserCompetitionTotalScore.Average,
                                UserCompetitionTotalScore.Total,
                                UserCompetitionTotalScore.Best,
                                UserCompetitionTotalScore.PreviousTotal
                            };
                var queryAllCustomers = from user in _context.Users
                                        select user.Username;
                List<string> users = queryAllCustomers.ToList<string>();

                //saving results in an List which will make sorting easier(ArrayList)
                List<RankResults> ranklist = new List<RankResults>();
                foreach (var item in query)
                {
                    if (item.Average > 0 && item.Best > 0)
                    {
                        RankResults rankResult = new RankResults();
                        rankResult.Username = item.Username;
                        rankResult.DisplayName = getDisplayName(item.Name, item.Surname);
                        rankResult.Best = item.Best;
                        rankResult.Total = item.Total;
                        rankResult.Average = item.Average;
                        rankResult.Rank = 0;
                        rankResult.RankStatus = getRankStatus(item.Average, item.PreviousTotal);
                        if (item.MemberID != null)
                        {
                            rankResult.isMember = true;
                        }
                        else
                        {
                            rankResult.isMember = false;
                        }
                        ranklist.Add(rankResult);
                        //remove user from users without scores
                        users.RemoveAll(x => x.Equals(item.Username));
                    }

                }
                for (int i = 0; i < users.Count; i++)
                {
                    RankResults tempRankResult = new RankResults();
                    User user = _context.Users.Where(u => u.Username.Equals(users.ElementAt(i))).FirstOrDefault<User>();
                    tempRankResult.Username = user.Username;
                    tempRankResult.DisplayName = getDisplayName(user.Name, user.Surname);
                    tempRankResult.Total = 0;
                    tempRankResult.Best = 0;
                    tempRankResult.Average = 0;
                    tempRankResult.Rank = 0;
                    tempRankResult.RankStatus = "equal";
                    if(user.MemberID != null)
                    {
                        tempRankResult.isMember = true;
                    }
                    ranklist.Add(tempRankResult);
                }
                return ranklist;
            }
            catch(Exception e)
            {
                return new List<RankResults>();
            }
        }
       
        private List<RankResults> groupRankings(int competitionID, int groupID)
        {
            try
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
                                User.Name,
                                User.Surname,
                                User.MemberID,
                                UserCompetitionTotalScore.Average,
                                UserCompetitionTotalScore.Total,
                                UserCompetitionTotalScore.Best,
                                UserCompetitionTotalScore.PreviousTotal
                            };
                //saving results in an List which will make sorting easier(ArrayList)
                int rank = 1;
                List<RankResults> ranklist = new List<RankResults>();
                foreach (var item in query)
                {
                    if (item.Average > 0 && item.Best > 0)
                    {
                        RankResults rankResult = new RankResults();
                        rankResult.Username = item.Username;
                        rankResult.DisplayName = getDisplayName(item.Name, item.Surname);
                        rankResult.Best = item.Best;
                        rankResult.Total = item.Total;
                        rankResult.Average = item.Average;
                        rankResult.Rank = rank;
                        rankResult.RankStatus = getRankStatus(item.Average, item.PreviousTotal);
                        if (item.MemberID != null)
                        {
                            rankResult.isMember = true;
                        }
                        else
                        {
                            rankResult.isMember = false;
                        }
                        ranklist.Add(rankResult);
                        rank++;
                    }
                }
                //rank and return results
                return ranklist;
            }catch(Exception e)
            {
                return new List<RankResults>();
            }
        }
        private string getDisplayName(string name,string surname)
        {
            string displayName = "";
            if(name != null && !name.Equals(""))
            {
                string[] names = name.Split(" "); //incase user has more than one name
                displayName += names[0];
                if (names.Count() > 1)
                {
                    displayName +=" "+ names[1]; ;
                }
            }
            if (surname != null && !surname.Equals(""))
            {
                displayName += " "+ surname;
            }
            // if member does not have a name/surname or combination name and surname is less than 2 characters
            if (displayName.Equals("") || displayName.Length< 2) 
            {
                displayName = null;
            }
            return displayName;
        }
        private string getRankStatus(double average,double preAverage) // funtion to check if member's total increased,decreased or still the same(equal or 0)
        {
            if(average > preAverage && preAverage > 0)
            {
                return "up";
            }else if(average < preAverage)
            {
                return "down";
            }
            else
            {
                return "equal";
            }
        }
    }
}

