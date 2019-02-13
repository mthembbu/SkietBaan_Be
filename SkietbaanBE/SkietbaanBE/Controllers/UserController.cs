
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

        // POST: api/User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                //error handling, check if client provided valid data
                if (user == null)
                {
                    return new BadRequestObjectResult("user cannot be null");
                }
                else
                {
                    using (_context)
                    {
                        var dbUser = _context.Users
                        .Where(u => u.Username == user.Username && u.Id != user.Id) //check if a different user with the new username already exists
                        .FirstOrDefault<User>();
                        if (dbUser != null)
                        {
                            return BadRequest("Cannot update user, Username already exists");
                        }
                        dbUser = _context.Users
                        .Where(u => u.Id == user.Id)
                        .FirstOrDefault<User>();

                        //now updating user details
                        dbUser.Username = user.Username;
                        _context.Users.Update(dbUser);
                        await _context.SaveChangesAsync();
                        return Ok("User update successful");
                    }

                }
            }
            else
            {
                return new BadRequestObjectResult("user cannot be null");
            }
        }
    }
        // PUT: api/User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
                //error handling, check if client provided valid data
                if (user == null)
                {
                    return new BadRequestObjectResult("user cannot be null");
                }
                else
                {
                    User dbUser = _context.Users.Where(u => u.Username == user.Username && u.Id != user.Id) //check if a different user with the new username already exists
                        .FirstOrDefault<User>();
                    if (dbUser != null)
                    {
                        return BadRequest("Cannot update user, Username already exists");
                    }
                    dbUser = _context.Users.Where(u => u.Id == user.Id)
                        .FirstOrDefault<User>();

                    //now updating user details
                    dbUser.MemberID = user.MemberID;
                    dbUser.EntryDate = user.EntryDate;
                    dbUser.MemberExpiry = user.MemberExpiry;
                    _context.Users.Update(dbUser);
                    await _context.SaveChangesAsync();
                    return Ok("User update successful");
                }
            
        }
        // POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> LoginPost([FromBody]User user)
        {
            if (user.Username == null || user.Password == null || user.Email == null)
            {
                return new BadRequestObjectResult("No empty fields allowed");
            }
            foreach (User dbUser in GetUsers())
            {
                if (dbUser.Username.Equals(user.Username))
                {
                    if (Security.CompareHashedData(dbUser.Password,user.Password))
                        return new OkObjectResult("Successful login");
                    else
                        return new BadRequestObjectResult("Incorrect Password or Username");
                }
            }
            return new BadRequestObjectResult("User not found");
        }
    }
}

