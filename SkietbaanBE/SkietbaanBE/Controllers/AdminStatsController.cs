//using SkietbaanBE.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using SkietbaanBE.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;
namespace SkietbaanBE.Controllers
{
    public class AdminStatsController:Controller
    {
        private readonly ModelsContext _context;
        public AdminStatsController(ModelsContext db) {
            _context = db;
        }

        [HttpGet]
        [Route("adminstat")]
        public Dictionary<String,int> getStatDetails()
        {
            Dictionary<String, int> statDetails = new Dictionary<String, int>();
                var query = _context.Users.Where(u=>u.MemberID!=null);
            int count = 0;
            if (query != null)
            {
                count = (query.ToList()).Count();
            }
            statDetails.Add("new user", 500);
            statDetails.Add("new member", 500);
            statDetails.Add("total users", (this.SearchNonMember()).Count());
            statDetails.Add("total members", count);
            statDetails.Add("expiring members", (this.SearchExpiringMember()).Count());
            return statDetails;
        }
        public IEnumerable<User> SearchNonMember()
        {
            return (_context.Users.ToArray<User>().Where(u => u.MemberID == null || u.MemberID == "")).OrderBy(x => x.Username);
        }

        public IEnumerable<User> SearchExpiringMember()
        {
            var dbUsers = (_context.Users.Where(u => u.MemberID != null && u.MemberID != "" && u.MemberExpiryDate != null && u.MemberStartDate != null)).OrderBy(x => x.Username);
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
            for (int i = 0; i < timeMonths.Length; i++)
            {
                if (months[i] <= 2)
                {
                    users.Add(dbUsers.ToList().ElementAt(i));
                }
            }
            return users.ToArray<User>();
        }
    }
}
