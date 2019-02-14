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

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                User dbUser = null; //assume user does not exist
             
                dbUser = _context.Users
                                      .Where(u => u.Username == user.Username)
                                      .FirstOrDefault<User>();
                
                //if user aready exist return
                if(dbUser != null)
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

        // POST: api/user/login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult CreateToken([FromBody]User user)
        {
            var response = Unauthorized();
            var _user = Authenticate(user);
            
            if (_user != null)
            {
                var tokenString = BuildToken(_user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private User Authenticate(User user)
        {
            if (user.Username == null || user.Password == null || user.Email == null)
            {
                new BadRequestObjectResult("No empty fields allowed");
            }
            
            foreach (User dbUser in GetUsers())
            {
                if (dbUser.Username.Equals(user.Username))
                {
                    if (Security.CompareHashedData(dbUser.Password, user.Password))
                        return new User { Username = user.Username, Password = user.Password, Email = user.Email, EntryDate = user.EntryDate, MemberID = user.MemberID, MemberExpiry = user.MemberExpiry, Admin = user.Admin };
                    else
                    {
                        new BadRequestObjectResult("Incorrect Password or Username");
                    }
                }
            }
            return user;
        }

        private string BuildToken(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Acr, GetCorrelationId())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
             _config["Jwt:Issuer"],
             expires: DateTime.Now.AddDays(365),
             signingCredentials: creds);

             return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetCorrelationId()
        {
            string guid = Guid.NewGuid().ToString();
            int index = guid.LastIndexOf("-");
            return guid.Substring(index + 1);
        }
    }
}

