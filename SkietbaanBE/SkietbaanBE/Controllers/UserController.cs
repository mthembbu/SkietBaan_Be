using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private ModelsContext _context;
        public UserController(ModelsContext db) {
            _context = db;
        }

        //public UserController() { }

        // GET: api/User
        [HttpGet]
        public IEnumerable<User> Get() {
            var users = _context.Users.ToArray<User>();
            return users;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public User Get(int id) {
            User user = _context.Users.Find(id);
            return user;
        }

        [HttpGet("test/{ret}", Name = "Test")]
        public string Test(string ret) {
            return $"Returned : {ret}";
        }

        // POST: api/User
        [HttpPost]
        public virtual void Post([FromBody] User user) {
            _context.Add(user);
            _context.SaveChanges();
        }

        // PUT: api/User/5
        [HttpPut]
        public void Put([FromBody] User user) {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public void Delete(User user) {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}