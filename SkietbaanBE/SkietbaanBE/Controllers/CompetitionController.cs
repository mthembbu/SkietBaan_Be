﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Competition")]

    public class CompetitionController : Controller
    {
        private ModelsContext _context;
        private NotificationMessages _notificationMessages;
        public CompetitionController(ModelsContext db , NotificationMessages notificationMessages)
        {
            _context = db;
            _notificationMessages = notificationMessages;
        }
        /** The method to return an array of competition objects that hold the true Status*/
        // GET: api/Competition
        [HttpGet]
        public IEnumerable<Competition> Get()
        {
            //get the competitions where(Status == true)
            var competitionIDsQuery = from Comp in _context.Competitions
                                      where Comp.Status == true
                                      select Comp;
            List<Competition> competitionsList = competitionIDsQuery.ToList<Competition>();
            // return _context.Competitions.ToArray<Competition>();
            return competitionsList;
        }
        //Getting the competition by ID
        // GET: api/Competition/5
        [HttpGet("{id}")]
        public async Task<Competition> CompetitionGet(int id)
        {
            return await _context.Competitions.FindAsync(id);
        }
        //Getting the competition by ID
        // GET: api/Competition/5
        [HttpGet("{Name}")]
        public async Task<Competition> CompetitionGet(string Name)
        {
            return await _context.Competitions.FindAsync(Name);
        }
        //posting the competition to the competition table
        // POST: api/Competition
        [HttpPost]
        public async Task<IActionResult> addCompetition([FromBody]Competition comp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
 
            await _context.AddAsync(comp);
            await _context.SaveChangesAsync();
            _notificationMessages.CompetitionNotification(_context, comp);
            return Ok("Competition Added!!!!!!!");
        }
        //A method that updates the status of the competition
        // PUT: api/Competition/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompetition(int id, [FromBody]Competition comp)
        {
            if (ModelState.IsValid)
            {
                //error handling, check if client provided valid data
                if (comp == null)
                {
                    return new BadRequestObjectResult("competiton cannot be null");
                }
                else
                {
                    Competition dbComp = null; //assume competition does not exist
                    using (_context)
                    {
                        dbComp = _context.Competitions
                                         .Where(u => u.Name == comp.Name && u.Id != comp.Id) //check if a different user with the new username already exists
                                         .FirstOrDefault<Competition>();
                        if (dbComp != null)
                        {
                            return BadRequest("Cannot update competition, already exists");
                        }
                        dbComp = _context.Competitions
                                         .Where(u => u.Id == comp.Id)
                                         .FirstOrDefault<Competition>();

                        //now updating user details
                        dbComp.Name = comp.Name;
                        _context.Competitions.Update(dbComp);
                        await _context.SaveChangesAsync();
                        return Ok("Status update successful");
                    }
                }
            }
            else
            {
                return new BadRequestObjectResult("competition cannot be null");
            }
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

}
