using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;

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
        /** The method to return an array of competition objects that hold Status == true*/
        // GET: api/Competition
        [HttpGet]
        public IEnumerable<Competition> GetCompetitions()    
        {
            //get the competitions where(Status == true)
            List<Competition> competitionsList = (from Comp in _context.Competitions
                                                  where Comp.Status == true
                                                  select Comp).ToList<Competition>();     
            return competitionsList;
        }
        // The method that return an array of competition objects whether status is true or false
        // GET: api/Competition/all
        [HttpGet("all")]
        public IEnumerable<Competition> GetAllCompetitions()
       {
            //get the competitions where(Status == true / false)
            List<Competition> CompList = (from C in _context.Competitions
                                         select C).OrderBy(c => c.Status == false).ThenBy(x => x.Name).ToList<Competition>();
            //get the list of Requirements for Each Competition
            foreach (Competition C in CompList){
                List<Requirement> ReqList = (from R in _context.Requirements
                                            where R.CompID == C.Id
                                            select R).ToList<Requirement>();
                C.RequirementsList = ReqList;
            }
            return CompList;
        }
        //Getting a competition by ID
        // GET: api/Competition/id
        [HttpGet("{id}")]
        public async Task<Competition> CompetitionGetById(int id)
        {
            return await _context.Competitions.FindAsync(id);
        }
        //posting the competition to the competition table
        // POST: api/Competition
        [HttpPost]
        public async Task<IActionResult> AddCompetition([FromBody]Competition comp)
        {
            if (ModelState.IsValid)
            {
                Competition dbComp = _context.Competitions.FirstOrDefault(c => c.Name == comp.Name);
                if (dbComp != null)
                    return new BadRequestObjectResult(comp.Name + " already exists");
                _notificationMessages.CompetitionNotification(_context, comp);
                await _context.AddAsync(comp);
                await _context.SaveChangesAsync();
                return Ok("Competition Added!");
            }
            else {
                return new BadRequestObjectResult("competition cannot be null");
            }
        }
        //A method that updates the status of the competition
        // PUT: api/Competition/5
        [HttpPost("{id}")]
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
                                         .Where(u => u.Name == comp.Name && u.Id != comp.Id)
                                         .FirstOrDefault<Competition>();
                        if (dbComp == null) {
                            return BadRequest("Cannot update competition, no such competition!");
                        }
                        //now updating status to either true / false
                         dbComp.Status = comp.Status;
                        _context.Competitions.Update(dbComp);
                        await _context.SaveChangesAsync();
                        return Ok("Status update successful");
                    }
                }
            }
            else{
                return new BadRequestObjectResult("competition cannot be null");
            }
        }
        //A method that updates the competition standards / requirements by {Id}
        // POST: api/Competition/standard/5
        [HttpPost("/standard/{id}")]
        public async Task<IActionResult> UpdateCompetitionByRequirements(int compID, [FromBody] Competition Comp)
        { 
            if (ModelState.IsValid){
                //error handling, check if client provided valid data
                if (Comp == null){
                    return new BadRequestObjectResult("competiton cannot be null");
                }
                else {
                    Competition dbComp;//assume the competition does not exist
                    using (_context) {
                        dbComp = _context.Competitions.Where(C => C.Id == Comp.Id).FirstOrDefault<Competition>();
                        if (dbComp == null) {
                            return BadRequest("Cannot update competition, no such competition!");
                        }
                        foreach (Requirement R in Comp.RequirementsList){
                            Requirement Temp = new Requirement {
                                CompID = R.CompID,
                                Standard = R.Standard,
                                Accuracy = R.Accuracy,
                                Total = R.Total
                            };
                            await _context.AddAsync(Temp);
                        }
                        await _context.SaveChangesAsync();
                        return Ok("All Requirements for the "+Comp.Name+" have been added");
                    }
                }
            }
            else {
                return new BadRequestObjectResult("Invalid competition model!");
            }
        }
        public IEnumerable<Requirement> GetAllRequirementsByCompID(int CompID)
        {
            return _context.Requirements.Where(R => R.CompID == CompID).ToList<Requirement>();
        }
        //GET: api/competition/participants
        [HttpGet("participants")]
        public Dictionary<int, int> getUsersPerCompetition()
        {
            Dictionary<int, int> mapCompToNumUser = new Dictionary<int, int>();
            var competitionsList = this.GetCompetitions();
            foreach (var comp in competitionsList)
            {
                int count = (from score in _context.Scores
                             where score.Competition.Id == comp.Id
                             select score.User.Id).Distinct().ToList().Count();
                mapCompToNumUser.Add(comp.Id, count);
            }
            return mapCompToNumUser;
        }
    }
}
