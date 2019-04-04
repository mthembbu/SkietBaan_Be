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
            List<Competition> competitionsList = (from C in _context.Competitions
                                                  select C).OrderBy(c => c.Status == false).ThenBy(x => x.Name).ToList<Competition>();
            return competitionsList;
        }
        //A method that returns a last competitions ID
        //GET: api/Competition/getCompId
        [HttpGet("getCompID")]
        public int GetLastCompetitionID() { 

                return _context.Competitions.Last<Competition>().Id;
                
                }
        //posting the competition to the competition table
        // POST: api/Competition
        [HttpPost]
        public async Task<IActionResult> AddCompetition([FromBody]Competition Comp)
        {
            if (ModelState.IsValid)
            {
                //error handling, check if client provided valid data
                if (Comp == null)
                {
                    return new BadRequestObjectResult("competition cannot be null");
                }
                else{
                        Competition dbCompetition = _context.Competitions.FirstOrDefault(c => c.Name == Comp.Name);
                        if (dbCompetition != null)
                            return new BadRequestObjectResult(Comp.Name + " already exists");
                    _notificationMessages.CompetitionNotification(Comp);
                     _context.Competitions.Add(Comp);
                    _context.SaveChanges();
                    dbCompetition = _context.Competitions.Last <Competition>();
                   for (int i =0; i < 3;i++)
                    {
                        Requirement R = new Requirement {
                            Competition = dbCompetition  
                        };
                        _context.Requirements.Add(R);
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Competition"+Comp.Name+" Added!");
                    }
                }  
                else {
                return new BadRequestObjectResult("competition cannot be null");
            }
        }
        //A method that updates the status of the competition
        // PUT: api/Competition/5
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCompetitionByStatus(int id, [FromBody]Competition comp)
        {
            if (ModelState.IsValid)
            {
                //error handling, check if client provided valid data
                if (comp == null) {
                    return new BadRequestObjectResult("competition cannot be null");
                }
                else
                {
                    Competition dbCompetition = null; //assume competition does not exist
                   
                        dbCompetition = _context.Competitions
                                         .Where(C => C.Name == comp.Name && C.Id == comp.Id)
                                         .FirstOrDefault<Competition>();
                        if (dbCompetition == null) {
                            return BadRequest("Cannot update competition, no such competition!");
                        }
                        //now updating status to either true / false
                        dbCompetition.Status = comp.Status;
                        _context.Competitions.Update(dbCompetition);
                        await _context.SaveChangesAsync();
                        return Ok("Status update successful");   
                }
            }
            else{
                return new BadRequestObjectResult("competition cannot be null");
            }
        }
        //A method that updates the competition standards / requirements by {Id}
        // POST: api/Competition/standard/5
        [HttpPost("/Requirements/{CompID}")]
        public async Task<IActionResult> UpdateRequirementsByCompID(int CompID, [FromBody] Requirement [] requirements)
        {
            if (ModelState.IsValid)
            {
                //error handling, check if client provided valid data
                if (requirements == null)
                    return new BadRequestObjectResult("requirements cannot be null");
                else
                {
                    //assume the competition does not exist
                    Competition dbCompetition = _context.Competitions.Where(C => C.Id == CompID).FirstOrDefault<Competition>();
                    if (dbCompetition == null)
                    {
                        return BadRequest("Cannot update requirements, competition does not exist!");
                    }
                    List<Requirement> requirementsList = GetRequirementsByID(CompID).ToList<Requirement>();
                    for (int i =0; i < requirements.Length; i++)
                    {
                        Requirement R = requirementsList.ElementAt(i);
                        R.Standard = requirements[i].Standard;
                        R.Accuracy = requirements[i].Accuracy;
                        R.Total = requirements[i].Total;
                        _context.Requirements.Update(R);   
                    }
                    await _context.SaveChangesAsync();
                    return Ok("All Requirements for the " + dbCompetition.Name + " have been updated");
                }
            }
            else
            {
                return new BadRequestObjectResult("Invalid requirements model!");
            }
        }
 //##########################################################################################################
        //GET: api/Competition/Requirements/{id}
        /**returning an array of requirements for a given competition Id
         * The newly selected object contains only the Competition.ID and 
         * a Requirement object which will be converted in a front-end.
        */
        [HttpGet("/Requirements/{CompID}")]
        public IEnumerable<Object> GetRequirementsByCompID(int CompID)
        {
            var comp = from R in _context.Requirements
                        where (R.Competition.Id == CompID)
                        select new{
                            R.Competition.Id, R 
                        };
            List<Object> RList = comp.ToList<Object>();
            return RList;
        }
        //GET: /R/{CompID}
        /**Returns an array of requirements together with a null competition
         */
        [HttpGet("/R/{CompID}")]
        public IEnumerable<Requirement> GetRequirementsByID(int CompID)
        {
            return _context.Requirements.Where(R => R.Competition.Id == CompID).ToList<Requirement>();
        }
//##########################################################################################################
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
