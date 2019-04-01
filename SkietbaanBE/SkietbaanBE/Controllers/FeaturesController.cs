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

        private SendMail sendMail = new SendMail();

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
            if (user != null)
                return user;
            else return null;
        }

        [HttpPost]
        public string ForgotPassword(string user)
        {
            var username = _context.Users.FirstOrDefault(x => x.Username == user);

            if (username == null)
            {
                var email = _context.Users.FirstOrDefault(x => x.Email == user);

                if (email == null)
                {
                    return ("user not registered");
                }

                sendMail.SendPasswordEmail(user, "reset Password", email.Token);

                return ("Email Sent To: "+ email.Email);
            }

            sendMail.SendPasswordEmail(username.Email, "reset Password", username.Token);

            return ("Email Sent To: " + username.Email);
        }    
        
        [HttpPost]

        public string ResetPassword(string token, string password)
        {

            var user = _context.Users.FirstOrDefault(x => x.Token == token);

            user.Password = Security.HashSensitiveData(password);
            _context.Update(user);
            _context.SaveChanges();
            return ("changed");

        }

        [HttpPost]
        public string UpdateDetails([FromBody]User user)
        {
            var tempUser = _context.Users.FirstOrDefault(x => x.Token == user.Token);
            tempUser.Email = user.Email;
            tempUser.PhoneNumber = user.PhoneNumber;
            tempUser.Name = user.Name;
            _context.Update(tempUser);
            _context.SaveChanges();
            return ("updated");
        }


        [HttpPost]
        public ActionResult Login ([FromBody]User user)
        {
            User dbUser = null;
            if(!String.IsNullOrEmpty(user.Username))
            {
                dbUser = _context.Users.FirstOrDefault(x => x.Username == user.Username);
            }
            else
            {
                dbUser = _context.Users.FirstOrDefault(x => x.Email == user.Email);
            }
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
            var dbUsers =( _context.Users.Where(u => u.MemberID != null && u.MemberID != "")).OrderBy(x=>x.Username);
            DateTime current = DateTime.Now;
            var months = new List<int>();
            foreach (var user in dbUsers)
            {
                if(user.MemberExpiryDate.HasValue)
                {
                    DateTime expiry = (DateTime) user.MemberExpiryDate;
                    TimeSpan span = expiry.Subtract(current);
                    months.Add((span.Days) / 30);
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

        //// GET: api/User/SearchNonMember
        [HttpGet]
        [ActionName("SearchNonMember")]
        public IEnumerable<User> SearchNonMember()
        {
            return (_context.Users.ToArray<User>().Where(u => u.MemberID == null || u.MemberID == "")).OrderBy(x => x.Username);
        }

        //// GET: api/User/SearchExpiringMember
        [HttpGet]
        [ActionName("SearchExpiringMember")]
        public IEnumerable<User> SearchExpiringMember()
        {
            var dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "")).OrderBy(x => x.Username);
            DateTime current = DateTime.Now;
            var months = new List<int>();
            foreach (var user in dbUsers)
            {
                DateTime expiry = (DateTime)user.MemberExpiryDate;
                TimeSpan span = expiry.Subtract(current);
                months.Add((span.Days) / 30);
            }

            //Looks for Members with expiry time left that is <=2 months
            var timeMonths = months.ToArray();
            List<User> users = new List<User>();
            for (int i = 0; i < timeMonths.Length; i++) {
                if (months[i] <= 2)
                {
                    users.Add(dbUsers.ToList().ElementAt(i));
                }
            }

            return users.ToArray<User>();
        }

        //// GET: api/User/SearchExpiringMember
        [HttpGet]
        [ActionName("SearchExpiringMemberTimeLeft")]
        public IEnumerable<int> SearchExpiringMemberTimeLeft()
        {
            var dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "")).OrderBy(x => x.Username);
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

            //Looks for Members with expiry time left that is <=2 months
            var timeMonths = months.ToArray();
            List<User> users = new List<User>();
            var monthsExpiring = new List<int>();
            for (int i = 0; i < timeMonths.Length; i++)
            {
                if (months[i] <= 2)
                {
                    users.Add(dbUsers.ToList().ElementAt(i));

                    int expiryYear = dbUsers.ToList().ElementAt(i).MemberExpiryDate.Value.Year;
                    int yearLeft = expiryYear - current.Year;
                    if (yearLeft == 0)
                    {
                        int monthsLeft = dbUsers.ToList().ElementAt(i).MemberExpiryDate.Value.Month - current.Month;
                        monthsExpiring.Add(monthsLeft);
                    }
                    else
                    {
                        if (dbUsers.ToList().ElementAt(i).MemberExpiryDate.Value.Month > current.Month)
                        {
                            int diff = dbUsers.ToList().ElementAt(i).MemberExpiryDate.Value.Month - current.Month;
                            int monthsLeft = 12 - diff;
                            monthsExpiring.Add(monthsLeft);
                        }
                        else if (current.Month > dbUsers.ToList().ElementAt(i).MemberExpiryDate.Value.Month)
                        {
                            int diff = current.Month - dbUsers.ToList().ElementAt(i).MemberExpiryDate.Value.Month;
                            int monthsLeft = 12 - diff;
                            monthsExpiring.Add(monthsLeft);
                        }
                    }

                }
            }
            return monthsExpiring.ToArray();
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

        //// POST: api/User/RenewMembership
        [HttpPost]
        [ActionName("RenewMembership")]
        public async Task<IActionResult> RenewMembership([FromBody] User user)
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
            dbUser.MemberExpiryDate = dbUser.MemberExpiryDate.Value.AddYears(1); ;
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
