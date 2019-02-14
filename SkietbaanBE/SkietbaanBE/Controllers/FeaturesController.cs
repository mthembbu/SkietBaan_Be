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
    }
}
