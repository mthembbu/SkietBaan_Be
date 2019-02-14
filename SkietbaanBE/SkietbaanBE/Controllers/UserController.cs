
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private ModelsContext _context;
        private IConfiguration _config;
        public UserController(ModelsContext db, IConfiguration config)
        {
            _context = db;
            _config = config;
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
        public async Task<ActionResult> AddUser([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                User dbUser = null; //assume user does not exist

                dbUser = _context.Users.FirstOrDefault(x => x.Username == user.Username);

                //if user aready exist return
                if (dbUser != null)
                {
                    return new BadRequestObjectResult("User already exists");
                }
                //get today's date and save it under user entry date
                user.EntryDate = DateTime.Now;
                //encrypt password
                user.Password = Security.HashSensitiveData(user.Password);
                //generate token
                string tokenString = BuildToken();
                user.Token = tokenString;
                //Save User
                await _context.AddAsync(user);
                await  _context.SaveChangesAsync();

                new OkObjectResult("User saved successfully");
            }
            else
            {
                 return new BadRequestObjectResult("user cannot be null");
            }
            return Ok(user);
        }

        private string BuildToken()
        {
            string guid = Guid.NewGuid().ToString();
            int index = guid.LastIndexOf("-");
            return guid.Substring(index + 1);
        }

        // PUT: api/User/
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
                    User dbUser = null; //assume user does not exist
                    using (_context)
                    {
                        dbUser = _context.Users
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
}



