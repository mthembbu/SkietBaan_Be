using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using OfficeOpenXml;
using SkietbaanBE.Models;

namespace SkietbaanBE.Lib
{
    public class ExelTestData
    {
        private ModelsContext _context;
        private List<ExelData> listData;
        public ExelTestData(ModelsContext modelsContext)
        {
            _context = modelsContext;
            listData = getExelData();
        }
        public List<ExelData> getExelData()
        {
            string testData = "";
            string filePath = @"SkietbaanTestData.xlsx";
            FileInfo file = new FileInfo(filePath);
            List<ExelData> exelDataList = new List<ExelData>();
            using(ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                var rawText = string.Empty;
                for (int row = 2; row <= rowCount; row++)
                {
                    ExelData exelData = new ExelData();
                    exelData.CompetitionName = validateText(worksheet.Cells[row, 1].Value.ToString());
                    exelData.Score = int.Parse(validateNumber(worksheet.Cells[row, 2].Value.ToString()));
                    exelData.Name = validateText(worksheet.Cells[row, 3].Value.ToString());
                    exelData.Surname = validateText(worksheet.Cells[row, 4].Value.ToString());
                    exelData.ScoreEntryDate = DateTime.Parse(validateText(worksheet.Cells[row, 5].Text.ToString()));
                    exelData.UserId = long.Parse(validateNumber(worksheet.Cells[row, 6].Value.ToString()));
                    exelDataList.Add(exelData);
                }
            }


            return exelDataList;
        }
        public List<User> AddUsersFromExcel()
        {
            List<User> users = new List<User>();
            for(int i = 0; i < listData.Count; i++)
            {
                ExelData exelData = listData.ElementAt(i);
                User user = new User()
                {
                    Username = exelData.Name + " " + exelData.Surname,
                    MemberID = exelData.UserId + ""
                };
                if (!UserExist(users,user))
                {
                    users.Add(user);
                }
            }
            _context.AddRange(users);
            _context.SaveChanges();
            return users;
        }
        public List<Competition> AddCompetitionsFromExcel()
        {
            List<Competition> competitions = new List<Competition>();
            for (int i = 0; i < listData.Count; i++)
            {
                ExelData exelData = listData.ElementAt(i);
                Competition competition = new Competition()
                {
                    Name = exelData.CompetitionName,
                    Status = true,
                    BestScoresNumber = 4
                };
                if (!CompetitionExist(competitions,competition))
                {
                    competitions.Add(competition);
                }
            }
            
            _context.Competitions.AddRange(competitions);
            _context.SaveChanges();
            return competitions;
        }
        public void AddScoreFromExcel()
        {
            List<Score> scores = new List<Score>();
            for (int i = 0; i < listData.Count; i++)
            {
                Competition competition = _context.Competitions.Where(x => x.Name.Equals(listData.ElementAt(i).CompetitionName)).FirstOrDefault<Competition>();
                User user = _context.Users.Where(x => x.MemberID == listData.ElementAt(i).UserId + "").FirstOrDefault<User>();
                Score score = new Score()
                {
                    Competition = competition,
                    User = user,
                    UserScore = listData.ElementAt(i).Score,
                    UploadDate = listData.ElementAt(i).ScoreEntryDate,
                };
                //scores.Add(score);
                _context.Scores.Add(score);
                _context.SaveChanges();
                Calculations calc = new Calculations(_context);
                calc.performCalculations(score.User.Id, score.Competition.Id);
            }

        }
        private bool UserExist(List<User> users,User user)
        {
            for(int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).MemberID == user.MemberID)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CompetitionExist(List<Competition> competitions,Competition competition)
        {
            for (int i = 0; i < competitions.Count; i++)
            {
                if (competitions.ElementAt(i).Name.Equals(competition.Name))
                {
                    return true;
                }
            }
            return false;
        }
        //validation
        private string validateNumber(string value)
        {
            if(value.Length <= 6)
            {
                if (value.Equals("NULL"))
                {
                    return "0";
                }
                else if (value.Contains(","))
                {
                    value = value.Remove(value.IndexOf(','));
                    if (int.Parse(value) > 100)
                    {
                        return "100";

                    }
                    else
                    {
                        return value;
                    }
                }
                /*else if (int.Parse(value) > 100)
                {
                    return "100";
                }*/
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }
        private string validateText(string value)
        {
            if (value.Equals("NULL"))
            {
                return "";
            }
            else
            {
                return value;
            }
        }
        private string validateDate(string value)
        {
            string date = value.Replace("-", "/");
            return date;
        }
    }
    //excel object 
    public class ExelData
    {
        public string CompetitionName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Score { get; set; }
        public DateTime ScoreEntryDate { get; set; }
        public long UserId { get; set; }
    }

}


