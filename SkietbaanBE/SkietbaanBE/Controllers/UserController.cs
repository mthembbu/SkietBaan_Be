
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private ModelsContext _context;
        public UserController(ModelsContext db)
        {
            _context = db;
        }
        // GET: api/User
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToArray<User>();
        }
        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<User> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        //POST: api/User
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                User dbUser = null; //assume user does not exist

                dbUser = _context.Users.FirstOrDefault(x => x.Username == user.Username);

                //if user aready exist return
                if (dbUser != null)
                {
                    return Ok("User already exists");
                }
                //get today's date and save it under user entry date
                user.EntryDate = DateTime.Now;
                //encrypt password
                user.Password = Security.HashSensitiveData(user.Password);
                //Save User
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok("User saved successfully");
            }
            else
            {
                return new BadRequestObjectResult("user cannot be null");
            }
        }
    }
}



