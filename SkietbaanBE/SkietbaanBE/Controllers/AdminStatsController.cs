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
        public Dictionary<String, double> getStatDetails()
        {
            try
            {
                Dictionary<String, double> statDetails = new Dictionary<String, double>();
                var query = _context.Users.Where(u => u.MemberID != null);
                var users = _context.Users;
                int count = 0;
                DateTime date = DateTime.Now;
                int month = date.Month;
                int year = date.Year;
                if (query != null)
                {
                    count = (query.ToList()).Count();
                }
                var nonMemberUsers = (users.Where(u => u.MemberID == null)).Count();
                var newUsers = (double)((users.Where(x => x.EntryDate.Year == year && x.EntryDate.Month == month).ToList()).ToList()).Count;
                var newMembers = (double)(users.Where(y => y.MemberStartDate.Value.Year == year && y.MemberStartDate.Value.Month == month)).Count();
                var new_user_percent = Math.Round(((newUsers / nonMemberUsers) * 100), 1);
                var new_member_percent = Math.Round(((newMembers / query.Count()) * 100), 1);
                statDetails.Add("new user", newUsers);
                statDetails.Add("new member", newMembers);
                statDetails.Add("total users", (this.SearchNonMember()).Count());
                statDetails.Add("total members", count);
                statDetails.Add("expiring members", (this.SearchExpiringMember()).Count());
                statDetails.Add("new user percentage", new_user_percent);
                statDetails.Add("new member percentage", new_member_percent);
                return statDetails;
            }catch(Exception e)
            {
                return null;
            }
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
