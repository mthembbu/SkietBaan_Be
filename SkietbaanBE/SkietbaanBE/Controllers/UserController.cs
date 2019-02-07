﻿using System;
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
        public async Task<IActionResult> AddUser(int id,[FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                //get user with the specified ID from database
                User dbUser = await _context.Users.FindAsync(id);
                //user not found
                if(dbUser == null)
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


        // POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> LoginPost([FromBody]User user)
        {
            if (user.Username == null || user.Password == null || user.Email == null)
            {
                return new BadRequestObjectResult("No empty fields allowed");
            }

            foreach (User dbUser in Get())
            {
                if (dbUser.Username.Equals(user.Username))
                {
                    if (dbUser.Password.Equals(user.Password))
                        return new OkObjectResult("Successful login");
                    else
                        return new BadRequestObjectResult("Incorrect Password or Username");
                }

            }
            return new BadRequestObjectResult("User not found");
        }

    }
}