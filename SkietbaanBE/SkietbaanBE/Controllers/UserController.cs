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

        //public UserController() { }

        // GET: api/User
        [HttpGet]
        public IEnumerable<User> Get()
        {
            var users = _context.Users.ToArray<User>();
            return users;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<User> Get(int id)
        {
            User user = await _context.Users.FindAsync(id);
            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] User user)
        {
            
            if (ModelState.IsValid)
            {
                //get today's date and save it under user entry date
                DateTime currentDate = new DateTime();
                user.EntryDate = currentDate;

                //Save User
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();


                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
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
            await _context.SaveChangesAsync();
            return new OkObjectResult("User update successful");
        }
    }
}