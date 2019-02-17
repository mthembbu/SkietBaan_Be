using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[Action]")]
    public class FeaturesController : Controller
    {
        private ModelsContext _context;
        public FeaturesController(ModelsContext db)
        {
            _context = db;
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
                 return new NotFoundObjectResult($"{user.Username} not found");
            if (Security.HashSensitiveData(user.Password) == dbUser.Password)
                return Ok(dbUser);
            else return new BadRequestObjectResult("Invalid Password");
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

                //now updating user details
             dbUser.MemberID = user.MemberID;
             dbUser.EntryDate = user.EntryDate;
             dbUser.MemberExpiry = user.MemberExpiry;
             _context.Users.Update(dbUser);
             await _context.SaveChangesAsync();
             return Ok("User update successful");
        }
    }
}