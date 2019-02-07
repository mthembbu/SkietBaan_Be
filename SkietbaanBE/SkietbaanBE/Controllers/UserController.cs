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
    [Route("api/User")]
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
            var users = _context.Users.ToArray<User>();
            return users;
        }
        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<User> GetUser(int id)
        {
            User user = await _context.Users.FindAsync(id);
            return user;
        }
        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> AddUser(int id,[FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                User dbUser = await _context.Users.FindAsync(id);
                //user not found
                if(user == null)
                {
                    return NotFound("User does not exist");
                }
                //get today's date and save it under user entry date
                user.EntryDate = DateTime.Now;
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
        // PUT: api/User/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] User user)
        {
            //error handling, check if client provided valid data
            if (user == null)
            {
                return new BadRequestObjectResult("user cannot be null");
            }
            else if (GetUser(user.Id) == null)
            {
                return NotFound("user does not exist");
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok("User update successful");
        }
    }
}
