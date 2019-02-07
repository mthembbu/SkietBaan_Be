using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private ModelsContext _context;
        public UserController(ModelsContext db)
        {
            _context = db;
        }

        //public UserController() { }

        // GET: api/User
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            var users = _context.Users.ToArray<User>();
            return users;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<User> Get(int id)
        {
            User user = _context.Users.Find(id);
            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User user)
        {
            //error handling, check if client provided valid data
            if (user == null)
            {
                return new BadRequestObjectResult("user cannot be null");
            }
            else if (Get(user.Id) != null)
            {
                return new OkObjectResult("user already exists");
            }

            //get today's date and save it under user entry date
            DateTime currentDate = new DateTime();
            user.EntryDate = currentDate;

            //Save User
            _context.Add(user);
            _context.SaveChanges();

            return new OkObjectResult("Registration successfull, Membership number will be assigned by Admin");
        }

        // POST: api/login
        [HttpPost]
        public async Task<ActionResult> LoginPost([FromBody]string username, string password, string email)
        {
            foreach (User user in Get().Result) {

                if (username == null || password == null || email == null)
                {
                    return new BadRequestObjectResult("No empty fields allowed");
                }else if (user.Username.Equals(username) && user.Password.Equals(password) && user.Email.Equals(email)){
                    return new OkObjectResult("Successful login");
                }
                else
                {
                    return new BadRequestObjectResult("Unsuccessful login");
                }
            }
            return null;
        }

        private object Get(object id)
        {
            throw new NotImplementedException();
        }

        // PUT: api/User/5
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] User user)
        {
            //error handling, check if client provided valid data
            if (user == null)
            {
                return new BadRequestObjectResult("user cannot be null");
            }
            else if (Get(user.Id) == null)
            {
                return new OkObjectResult("user does not exist");
            }

            _context.Users.Update(user);
            _context.SaveChanges();
            return new OkObjectResult("User update successful");
        }
    }
}