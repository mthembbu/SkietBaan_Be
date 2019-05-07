using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;
using System.Data;
using System.Net.Mail;
using System.Text;

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
            try
            {
                User user = _context.Users.FirstOrDefault(x => x.Token == token);
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string generateCSV(string filter,string adminToken)
        {
            var dbUsers = (_context.Users.ToArray());
            var Adminuser = _context.Users.FirstOrDefault(x => x.Token == adminToken);
            if (filter == "members")
            {
                 dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "")).ToArray();
            }
            else if(filter == "users")
            {
                 dbUsers = (_context.Users.Where(u => u.MemberID == null || u.MemberID == "")).ToArray();

            }
            else if(filter == "expiring")
            {
                var expdbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "" && u.MemberExpiryDate != null && u.MemberStartDate != null)).OrderBy(x => x.Username);
                DateTime current = DateTime.Now;
                var months = new List<int>();
                foreach (var user in expdbUsers)
                {
                    DateTime expiry = (DateTime)user.MemberExpiryDate;
                    TimeSpan span = expiry.Subtract(current);
                    months.Add((span.Days) / 30);
                }

                //Looks for Members with expiry time left that is <=2 months
                var timeMonths = months.ToArray();
                List<User> users = new List<User>();
                for (int i = 0; i < timeMonths.Length; i++)
                {
                    if (months[i] <= 2)
                    {
                        users.Add(dbUsers.ToList().ElementAt(i));
                        _notificationMessage.ExpiryNotification(users);
                    }
                }
                dbUsers = users.ToArray<User>();
            }

            StringBuilder mydata = new StringBuilder();

            mydata.Append("Username" + "," + "Cellphone Number" + "," + "Email Address" + "," + "Name and Surname");
            mydata.Append(System.Environment.NewLine);            

            foreach (User user in dbUsers)
            {
                var number = "";

                if (user.PhoneNumber == null)
                {
                    number = " ";                    
                }
                else {
                    number += "+27";
                    number += user.PhoneNumber.Substring(1, user.PhoneNumber.Length-1);
                }
                mydata.Append(user.Username+","+ number + "," + user.Email+"," + user.Name+" "+user.Surname);
                mydata.Append(System.Environment.NewLine);   
            }

            byte[] data = Encoding.ASCII.GetBytes(mydata.ToString());

            MemoryStream ms = new MemoryStream(data);           

            Attachment attachment = new Attachment(ms, $"{filter}.csv", "text/plain");

            if(Adminuser != null)
            {
                if (sendMail.SendEmail(Adminuser.Email.Trim(), "csv", attachment))
                    return (filter + ".csv" + " sent to " + Adminuser.Email);
                else
                    return "Could not send email.\nAuthentication Error";
            }
            else
            {
                return "Something went wrong";
            }
        }

        [HttpPost]
        public string ForgotPassword(string user)
        {
            try
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

                    return ("Email Sent To: " + email.Email);
                }

                sendMail.SendPasswordEmail(username.Email, "reset Password", username.Token);

                return ("Email Sent To: " + username.Email);
            }
            catch(Exception e)
            {
                return ("something went wrong");
            }
            
        }    
        
        [HttpPost]

        public string ResetPassword(string token, string password)
        {
            try{
                var user = _context.Users.FirstOrDefault(x => x.Token == token);

                if (user == null)
                {
                    return ("forgot password link has already been used");
                }

                user.Password = Security.HashSensitiveData(password);
                string guid = Guid.NewGuid().ToString();
                int index = guid.LastIndexOf("-");
                string tokenString = guid.Substring(index + 1);
                user.Token = tokenString;
                _context.Update(user);
                _context.SaveChanges();
                return ("changed");
            }
            catch (System.Data.SqlClient.SqlException)
            {
                return ("forgot password link has already been used");
            }

           

        }

        [HttpPost]
        public string UpdateDetails([FromBody]User user)
        {
            try
            {
                var tempUser = _context.Users.FirstOrDefault(x => x.Token == user.Token);
                tempUser.Email = user.Email;

                if (user.PhoneNumber == "")
                {
                    user.PhoneNumber = null;
                }
                tempUser.PhoneNumber = user.PhoneNumber;
                if (user.Name == "")
                {
                    user.Name = null;
                }
                tempUser.Name = user.Name;
                if (user.Surname == "")
                {
                    user.Surname = null;
                }
                tempUser.Surname = user.Surname;
                _context.Update(tempUser);
                _context.SaveChanges();
                return ("updated");
            }
            catch(Exception e)
            {
                return "Update failed: "+e.Message;
            }
        }


        [HttpPost]
        public ActionResult Login ([FromBody]User user)
        {
            User dbUser = null;
            try
            {
                if (!String.IsNullOrEmpty(user.Username))
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
            }catch(Exception e)
            {
                return new BadRequestObjectResult(e.Message);
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
                   if((((user.MemberExpiryDate.Value.Year - current.Year) * 12) + (user.MemberExpiryDate.Value.Month - current.Month))>0){
                            months.Add(((user.MemberExpiryDate.Value.Year - current.Year) * 12) + user.MemberExpiryDate.Value.Month - current.Month);
                    }
                   else if((((user.MemberExpiryDate.Value.Year - current.Year) * 12) + (user.MemberExpiryDate.Value.Month - current.Month)) == 0)
                    {
                        if (user.MemberExpiryDate.Value.Day - current.Day > 0)
                        {
                            months.Add(((user.MemberExpiryDate.Value.Year - current.Year) * 12) + user.MemberExpiryDate.Value.Month - current.Month);
                        }
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
            try
            {
                var dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "")).OrderBy(x => x.Username);
                DateTime current = DateTime.Now;
                List<User> users = new List<User>();
                foreach (var user in dbUsers)
                {
                    if ((((user.MemberExpiryDate.Value.Year - current.Year) * 12) + (user.MemberExpiryDate.Value.Month - current.Month)) > 0)
                    {
                        users.Add(user);
                    }
                    else if ((((user.MemberExpiryDate.Value.Year - current.Year) * 12) + (user.MemberExpiryDate.Value.Month - current.Month)) == 0)
                    {
                        if (user.MemberExpiryDate.Value.Day - current.Day > 0)
                        {
                            users.Add(user);
                        }
                    }
                }
                return users.ToArray();
            }
            catch
            {
                return null;
            }
            
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
            var dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != ""&& u.MemberExpiryDate!=null&& u.MemberStartDate!=null)).OrderBy(x => x.Username);
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
            if (users.Count != 0)
            {
                _notificationMessage.ExpiryNotification(users);
            }
            return users.ToArray<User>();
        }

        //// GET: api/User/SearchExpiringMember
        [HttpGet]
        [ActionName("SearchExpiringMemberTimeLeft")]
        public IEnumerable<int> SearchExpiringMemberTimeLeft()
        {
            var dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "" && u.MemberExpiryDate != null && u.MemberStartDate != null)).OrderBy(x => x.Username);
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
            int year = ((user.MemberExpiryDate).Value).Year;
            if (dbUser == null)
                {
                    return BadRequest("User is null");
                }
             dbUser.MemberID = user.MemberID;
             dbUser.MemberStartDate = user.MemberStartDate;
            
             if((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0)){
                dbUser.MemberExpiryDate = user.MemberExpiryDate.Value.AddYears(+1);
                dbUser.MemberExpiryDate = dbUser.MemberExpiryDate.Value.AddDays(+1);
            }
            else
            {
                var monthNum = DiffMonths(user.MemberExpiryDate.Value, DateTime.Now);
                if (monthNum <= 12)
                {
                    dbUser.MemberExpiryDate = user.MemberExpiryDate.Value.AddMonths(+12);
                }
                else
                {
                    return Ok("the user has alredy expired");
                }
            }
             _context.Users.Update(dbUser);
             await _context.SaveChangesAsync();
            _notificationMessage.ConfirmationNotification(dbUser);
             return Ok("User update successful");
        }
        public int DiffMonths( DateTime start, DateTime end)
        {
            int months = 0;
            DateTime tmp = start;

            while (tmp < end)
            {
                months++;
                tmp = tmp.AddMonths(1);
            }
            return months;
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

            if (user.EntryDate == user.MemberExpiryDate)
            {
                dbUser.MemberExpiryDate = dbUser.MemberExpiryDate.Value.AddYears(1);
                _context.Users.Update(dbUser);
            }
            else
            {
                dbUser.AdvanceExpiryDate = user.MemberExpiryDate;
                _context.Users.Update(dbUser);
                _context.SaveChanges();
                ScheduleJob.ReNewUserMemberShip(dbUser);
            }
            await _context.SaveChangesAsync();
            _notificationMessage.RenewalNotification(dbUser);
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
