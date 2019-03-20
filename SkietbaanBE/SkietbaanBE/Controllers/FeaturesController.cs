using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[Action]")]
    public class FeaturesController : Controller
    {
        private ModelsContext _context;
        private NotificationMessages _notificationMessage;
        public FeaturesController(ModelsContext db, NotificationMessages notificationMessage)
        {
            _context = db;
            _notificationMessage = notificationMessage;
        }
        //api/features/getuserbytoken/{token}
        [HttpGet("{token}")]
        public User GetUserByToken(string token)
        {
            var user = _context.Users.FirstOrDefault(x => x.Token == token);
                if(user != null)
                    return user;
            else return null;
        }

        [HttpPost]
        public ActionResult Login ([FromBody]User user)
        {
            var dbUser = _context.Users.FirstOrDefault(x => x.Username == user.Username);
            if (dbUser == null)
            {
                return new NotFoundObjectResult($"{user.Username} not found");
            }
            if (Security.HashSensitiveData(user.Password) == dbUser.Password)
            {
                return Ok(dbUser);
            }
            else
            {
                return new BadRequestObjectResult("Invalid Password");
            }
        }

        //// GET: api/User/Search?Username=myusername
        [HttpGet]
        [ActionName("Search")]
        public User Search(string username)
        {
            List<User> users = _context.Users.ToList<User>();
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).Username.Equals(username))
                {
                    return users.ElementAt(i);
                }
            }
            return null;
        }

        //// GET: api/User/TimeLeft
        [HttpGet]
        [ActionName("TimeLeft")]
        public IEnumerable<int> TimeLeft()
        {
            var dbUsers = _context.Users.Where(u => u.MemberID != null && u.MemberID != "");
            DateTime current = DateTime.Now;
            var months = new List<int>();
            foreach (var user in dbUsers)
            {
                int expiryYear = user.MemberExpiryDate.Value.Year;
                int yearLeft = expiryYear - current.Year;
                if (yearLeft == 0)
                {
                    int monthsLeft = user.MemberExpiryDate.Value.Month - current.Month;
                    months.Add(monthsLeft);
                }
                else
                {
                    if (user.MemberExpiryDate.Value.Month > current.Month)
                    {
                        int diff = user.MemberExpiryDate.Value.Month - current.Month;
                        int monthsLeft = 12 - diff;
                        months.Add(monthsLeft);
                    }
                    else if (current.Month > user.MemberExpiryDate.Value.Month)
                    {
                        int diff = current.Month - user.MemberExpiryDate.Value.Month;
                        int monthsLeft = 12 - diff;
                        months.Add(monthsLeft);
                    }
                    else
                    {
                        months.Add(12);
                    }
                }
            }
            return months.ToArray();
        }

        //// GET: api/User/SearchMember
        [HttpGet]
        [ActionName("SearchMember")]
        public IEnumerable<User> SearchMember()
        {
            return (_context.Users.ToArray<User>().Where(u => u.MemberID != null && u.MemberID != "")).OrderBy(x=>x.Username);
        }

        //// POST: api/User/Update
        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> PutUserMember([FromBody] User user)
        {
            if (user.Username == null)
            {
                return new BadRequestObjectResult("No empty fields allowed");
            }
            User dbUser = _context.Users.Where(u => u.Username == user.Username)
                    .FirstOrDefault<User>();
            if (dbUser == null)
                {
                    return BadRequest("User is null");
                }
             dbUser.MemberID = user.MemberID;
             dbUser.MemberStartDate = user.MemberExpiryDate;
             dbUser.MemberStartDate = dbUser.MemberStartDate.Value.AddYears(-1);
             dbUser.MemberExpiryDate = user.MemberExpiryDate;
             _context.Users.Update(dbUser);
             await _context.SaveChangesAsync();
            _notificationMessage.ConfirmationNotification(_context, dbUser);
             return Ok("User update successful");
        }
        [HttpGet]
        public string TestExel()
        {
            

            ExelTestData exelTestData = new ExelTestData(_context);
            exelTestData.AddUsersFromExcel();
            exelTestData.AddCompetitionsFromExcel();
            exelTestData.AddScoreFromExcel();

            return "success";
        }
    }
}