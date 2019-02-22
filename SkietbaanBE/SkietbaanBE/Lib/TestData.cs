using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib {
    public class TestData {
        private ModelsContext _context;
        public void AddTestData(ModelsContext db) {
            _context = db;
            InsertGroups();
            LinkUserGroups();
            InsertCompetitions();
            InsertScores();
            insertUserCompetitionDataData();
            //calculateTotalAverageCompetitionScore();

        }
        public string InsertGroups() {
            List<Group> groups = new List<Group>();
            Group g1 = new Group();
            g1.Name = "Ratro Rabbit";
            Group g2 = new Group();
            g2.Name = "One-Hit-Wonder";
            Group g3 = new Group();
            g3.Name = "Joburg Champs";
            groups.Add(g1);
            groups.Add(g2);
            groups.Add(g3);
            _context.Groups.AddRange(groups);
            _context.SaveChanges();

            return "successfully added Groups";
        }
        public string LinkUserGroups() {
            if (_context.Users.ToList<User>() != null && _context.Groups.ToList<Group>() != null) {
                List<UserGroup> usergroups = new List<UserGroup>();
                //1
                UserGroup ug1 = new UserGroup();
                ug1.Group = _context.Groups.Find(1);
                ug1.User = _context.Users.Find(1);
                usergroups.Add(ug1);

                //2
                UserGroup ug2 = new UserGroup();
                ug2.Group = _context.Groups.Find(2);
                ug2.User = _context.Users.Find(1);
                usergroups.Add(ug2);

                //3
                UserGroup ug3 = new UserGroup();
                ug3.Group = _context.Groups.Find(1);
                ug3.User = _context.Users.Find(2);
                usergroups.Add(ug3);

                //4
                UserGroup ug4 = new UserGroup();
                ug4.Group = _context.Groups.Find(3);
                ug4.User = _context.Users.Find(2);
                usergroups.Add(ug4);

                //5
                UserGroup ug5 = new UserGroup();
                ug5.Group = _context.Groups.Find(1);
                ug5.User = _context.Users.Find(3);
                usergroups.Add(ug5);

                //6
                UserGroup ug6 = new UserGroup();
                ug6.Group = _context.Groups.Find(2);
                ug6.User = _context.Users.Find(3);
                usergroups.Add(ug6);

                //7
                UserGroup ug7 = new UserGroup();
                ug7.Group = _context.Groups.Find(3);
                ug7.User = _context.Users.Find(3);
                usergroups.Add(ug7);

                //8
                UserGroup ug8 = new UserGroup();
                ug8.Group = _context.Groups.Find(2);
                ug8.User = _context.Users.Find(4);
                usergroups.Add(ug8);

                //9
                UserGroup ug9 = new UserGroup();
                ug9.Group = _context.Groups.Find(3);
                ug9.User = _context.Users.Find(4);
                usergroups.Add(ug9);

                //10
                UserGroup ug10 = new UserGroup();
                ug10.Group = _context.Groups.Find(1);
                ug10.User = _context.Users.Find(5);
                usergroups.Add(ug10);

                //11
                UserGroup ug11 = new UserGroup();
                ug11.Group = _context.Groups.Find(2);
                ug11.User = _context.Users.Find(5);
                usergroups.Add(ug11);
                //12
                UserGroup ug12 = new UserGroup();
                ug12.Group = _context.Groups.Find(3);
                ug12.User = _context.Users.Find(5);
                usergroups.Add(ug12);

                _context.UserGroups.AddRange(usergroups);
                _context.SaveChanges();
                return "sucesss";
            }
            else {
                return "failed";
            }

        }
        public string InsertCompetitions() {
            List<Competition> compts = new List<Competition>();
            Competition compt1 = new Competition();
            compt1.Name = "Pistol 100m";
            compt1.Status = true;
            compt1.BestScoresNumber = 4;
            compts.Add(compt1);

            Competition compt2 = new Competition();
            compt2.Name = "Pistol 50m";
            compt2.Status = true;
            compt2.BestScoresNumber = 12;
            compts.Add(compt2);

            Competition compt = new Competition();
            compt.Name = "Rifle 100m";
            compt.Status = false;
            compt.BestScoresNumber = 6;
            compts.Add(compt);

            Competition compt3 = new Competition();
            compt3.Name = "Rifle 150m";
            compt3.Status = true;
            compt3.BestScoresNumber = 4;
            compts.Add(compt3);

            _context.Competitions.AddRange(compts);
            _context.SaveChanges();
            return "sucess";
        }
        public string InsertScores() {
            List<Score> scores = new List<Score>();

            Score score1 = new Score();
            //1
            score1.Competition = _context.Competitions.Find(1);
            score1.User = _context.Users.Find(1);
            score1.UploadDate = DateTime.Now;
            score1.PictureURL = null;
            score1.UserScore = 54;
            scores.Add(score1);

            Score score2 = new Score();
            //2
            score2.Competition = _context.Competitions.Find(2);
            score2.User = _context.Users.Find(1);
            score2.UploadDate = DateTime.Now;
            score2.PictureURL = null;
            score2.UserScore = 35;
            scores.Add(score2);

            Score score3 = new Score();
            //3
            score3.Competition = _context.Competitions.Find(3);
            score3.User = _context.Users.Find(1);
            score3.UploadDate = DateTime.Now;
            score3.PictureURL = null;
            score3.UserScore = 87;
            scores.Add(score3);

            Score score4 = new Score();
            //4
            score4.Competition = _context.Competitions.Find(1);
            score4.User = _context.Users.Find(2);
            score4.UploadDate = DateTime.Now;
            score4.PictureURL = null;
            score4.UserScore = 67;
            scores.Add(score4);

            Score score5 = new Score();
            //5
            score5.Competition = _context.Competitions.Find(2);
            score5.User = _context.Users.Find(2);
            score5.UploadDate = DateTime.Now;
            score5.PictureURL = null;
            score5.UserScore = 86;
            scores.Add(score5);

            Score score6 = new Score();
            //6
            score6.Competition = _context.Competitions.Find(1);
            score6.User = _context.Users.Find(3);
            score6.UploadDate = DateTime.Now;
            score6.PictureURL = null;
            score6.UserScore = 68;
            scores.Add(score6);

            Score score7 = new Score();
            //7
            score7.Competition = _context.Competitions.Find(2);
            score7.User = _context.Users.Find(3);
            score7.UploadDate = DateTime.Now;
            score7.PictureURL = null;
            score7.UserScore = 58;
            scores.Add(score7);

            Score score8 = new Score();
            //8
            score8.Competition = _context.Competitions.Find(4);
            score8.User = _context.Users.Find(3);
            score8.UploadDate = DateTime.Now;
            score8.PictureURL = null;
            score8.UserScore = 83;
            scores.Add(score8);

            Score score9 = new Score();
            //9
            score9.Competition = _context.Competitions.Find(1);
            score9.User = _context.Users.Find(4);
            score9.UploadDate = DateTime.Now;
            score9.PictureURL = null;
            score9.UserScore = 94;
            scores.Add(score9);

            Score score10 = new Score();
            //10
            score10.Competition = _context.Competitions.Find(4);
            score10.User = _context.Users.Find(4);
            score10.UploadDate = DateTime.Now;
            score10.PictureURL = null;
            score10.UserScore = 84;
            scores.Add(score10);

            Score score11 = new Score();
            //11
            score11.Competition = _context.Competitions.Find(2);
            score11.User = _context.Users.Find(5);
            score11.UploadDate = DateTime.Now;
            score11.PictureURL = null;
            score11.UserScore = 35;
            scores.Add(score11);

            Score score12 = new Score();
            //12
            score12.Competition = _context.Competitions.Find(3);
            score12.User = _context.Users.Find(5);
            score12.UploadDate = DateTime.Now;
            score12.PictureURL = null;
            score12.UserScore = 73;
            scores.Add(score12);

            Score score13 = new Score();
            //13
            score13.Competition = _context.Competitions.Find(4);
            score13.User = _context.Users.Find(5);
            score13.UploadDate = DateTime.Now;
            score13.PictureURL = null;
            score13.UserScore = 37;
            scores.Add(score13);

            _context.Scores.AddRange(scores);
            _context.SaveChanges();
            return "success";
        }
        public string delete() {
            List<Group> groups = _context.Groups.ToList<Group>();
            _context.Groups.RemoveRange(groups);
            _context.SaveChanges();


            List<UserGroup> usergroups = _context.UserGroups.ToList<UserGroup>();
            _context.UserGroups.RemoveRange(usergroups);
            _context.SaveChanges();

            List<Competition> competitions = _context.Competitions.ToList<Competition>();
            _context.Competitions.RemoveRange(competitions);
            _context.SaveChanges();

            List<Score> scores = _context.Scores.ToList<Score>();
            _context.Scores.RemoveRange(scores);
            _context.SaveChanges();

            return "success";
        }

        public string insertUserCompetitionDataData() {
            foreach (var score in _context.Scores) {
                UpdateUserCompStats(score);
                UpdateTotal(score);
            }

            return "success";
        }
        
        private void UpdateUserCompStats(Score score) {
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                            ucs.Competition.Id == score.Competition.Id &&
                                            ucs.Month == score.UploadDate.Value.Month &&
                                            ucs.Year == score.UploadDate.Value.Year);

            if (userCompStatsRecords.Count() < 1) {
                UserCompStats userCompStats = new UserCompStats();
                userCompStats.Competition = score.Competition;
                userCompStats.User = score.User;
                userCompStats.Best = score.UserScore;
                userCompStats.Month = score.UploadDate.Value.Month;
                userCompStats.Year = score.UploadDate.Value.Year;

                _context.UserCompStats.Add(userCompStats);
            } else {
                var userCompStats = userCompStatsRecords.First();
                if (userCompStats.Best < score.UserScore) {
                    userCompStats.Best = score.UserScore;
                    _context.UserCompStats.Update(userCompStats);
                }
            }

            _context.SaveChanges();
        }

        private void UpdateTotal(Score score) {
            var userCompStatsRecords = _context.UserCompStats.Where(ucs => ucs.User.Id == score.User.Id &&
                                        ucs.Competition.Id == score.Competition.Id);

            if (userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() <= score.Competition.BestScoresNumber) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                if (userCompetitionTotalScore == null) {
                    userCompetitionTotalScore = new UserCompetitionTotalScore {
                        Competition = score.Competition,
                        User = score.User,
                        Total = userCompStatsRecords.Sum(ucs => ucs.Best),
                        Average = (double)userCompStatsRecords.Sum(ucs => ucs.Best),
                    };

                    _context.Add(userCompetitionTotalScore);
                } else {
                    userCompetitionTotalScore.Total = userCompStatsRecords.Sum(ucs => ucs.Best);
                    userCompetitionTotalScore.Average = (double)userCompetitionTotalScore.Total / (double)userCompStatsRecords.Count();
                    _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                }
                _context.SaveChanges();
            } else if (userCompStatsRecords.Count() > 0 && userCompStatsRecords.Count() > score.Competition.BestScoresNumber) {
                UserCompetitionTotalScore userCompetitionTotalScore = _context.UserCompetitionTotalScores
                                                    .Where(ucs => ucs.User.Id == score.User.Id &&
                                                    ucs.Competition.Id == score.Competition.Id).FirstOrDefault();
                userCompetitionTotalScore.Total = userCompStatsRecords.OrderByDescending(x => x.Best)
                                                   .Take(score.Competition.BestScoresNumber).Sum(x => x.Best);
                userCompetitionTotalScore.Average = (double)userCompetitionTotalScore.Total / (double)score.Competition.BestScoresNumber;
                _context.UserCompetitionTotalScores.Update(userCompetitionTotalScore);
                _context.SaveChanges();
            }
        }
    }
}
