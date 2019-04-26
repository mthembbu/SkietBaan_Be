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
            try
            {  //get the competitions where(Status == true)
                List<Competition> competitionsList = (from Comp in _context.Competitions
                                                      where Comp.Status == true
                                                      select Comp).ToList<Competition>();

                if (competitionsList == null)
                    return new List<Competition>();
                else
                    return competitionsList;
            }
            catch { return new List<Competition>(); }

        }
        // The method that return an array of competition objects whether status is true or false
        // GET: api/Competition/all
        [HttpGet("all")]
        public IEnumerable<Competition> GetAllCompetitions()
       {
            List<Competition> competitionsList = new List<Competition>();
            //List<Competition> competitoinList;
            //get the competitions where(Status == true / false)
            try
            {
                competitionsList = (from C in _context.Competitions
                                                      select C).OrderBy(c => c.Status == false).ThenBy(x => x.Name).ToList<Competition>();

                if (competitionsList == null)
                    return new List<Competition>();
                else
                    return competitionsList;
            }
            catch { return competitionsList; }
        }
        //A method that returns a last competitions ID
        //GET: api/Competition/getCompId
        [HttpGet("getCompID")]
        public int GetLastCompetitionID() {
            try
            {
                int compId = 0;
                compId = _context.Competitions.Last<Competition>().Id;
                return compId;
            }
            catch
            {
                return -1;
            }
            
        }

        // DELETE: api/Groups/5
        [HttpPost("delete")]
        public void DeleteGroup([FromBody] Competition[] Compobj)
        {
            try
            {
                for (int i = 0; i < Compobj.Length; i++)
                {
                    var query = _context.Requirements.Where(m => m.Competition.Id == Compobj.ElementAt(i).Id);
                    var scores = _context.Scores.Where(s => s.Competition.Id == Compobj.ElementAt(i).Id);
                    var userstat = _context.UserCompetitionTotalScores.Where(w => w.Competition.Id == Compobj.ElementAt(i).Id);

                    if (scores != null)
                    {
                        foreach (var item in scores)
                        {
                            _context.Scores.Remove(item);
                        }
                        _context.SaveChanges();
                    }

                    if (userstat != null)
                    {
                        foreach (var item in userstat)
                        {
                            _context.UserCompetitionTotalScores.Remove(item);  
                        }
                        _context.SaveChanges();
                    }

                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            _context.Requirements.Remove(item);
                        }
                    }
                    _context.Competitions.Remove(Compobj.ElementAt(i));
                    _context.SaveChanges();
                }
            }
            catch
            {

            }
    
        }
        //posting the competition to the competition table together with a array of requirements using the Requirements filter
        // POST: api/Competition/filter
        [HttpPost("filter")]
        public IActionResult AddRequirementsFilter([FromBody]RequirementsFilter rFilter)
        { //error handling, check if client provided valid data
            try {
                if (rFilter == null) return new BadRequestObjectResult("Invalid competition object");
                
                Competition dbCompetition = _context.Competitions.FirstOrDefault(c => c.Name == rFilter.Competition.Name);
                if (dbCompetition != null)
                    return new BadRequestObjectResult(rFilter.Competition.Name + " already exists");
                _context.Competitions.Add(rFilter.Competition);
                _context.SaveChanges();
                _notificationMessages.CompetitionNotification(rFilter.Competition);
                dbCompetition = _context.Competitions.FirstOrDefault(x => x.Name == rFilter.Competition.Name);
                for (int i = 0; i < 3; i++){
                    Requirement R = new Requirement{
                        Competition = dbCompetition,
                        Standard = rFilter.GetRequirements.ElementAt(i).Standard
                    };

                    switch (R.Standard) {
                        case "Bronze":
                        case "bronze":
                            if (rFilter.GetRequirements.ElementAt(i).Accuracy == 0) R.Accuracy = 60;
                            else R.Accuracy = rFilter.GetRequirements.ElementAt(i).Accuracy;
                            if (rFilter.GetRequirements.ElementAt(i).Total == 0)
                                R.Total = rFilter.Competition.MaximumScore * 0.6;
                            else R.Accuracy = rFilter.GetRequirements.ElementAt(i).Total;
                            break;
                        case "Silver":
                        case "silver":
                            if (rFilter.GetRequirements.ElementAt(i).Accuracy == 0) R.Accuracy = 70;
                            else R.Accuracy = rFilter.GetRequirements.ElementAt(i).Accuracy;
                            if (rFilter.GetRequirements.ElementAt(i).Total == 0)
                                R.Total = rFilter.Competition.MaximumScore * 0.7;
                            else R.Accuracy = rFilter.GetRequirements.ElementAt(i).Total;
                            break;
                        case "Gold":
                        case "gold":
                            if (rFilter.GetRequirements.ElementAt(i).Accuracy == 0) R.Accuracy = 90;
                            else R.Accuracy = rFilter.GetRequirements.ElementAt(i).Accuracy;
                            if (rFilter.GetRequirements.ElementAt(i).Total == 0)
                                R.Total = rFilter.Competition.MaximumScore * 0.90;
                            else R.Accuracy = rFilter.GetRequirements.ElementAt(i).Total;
                            break;
                    }
                    _context.Requirements.Add(R);
                }
                _context.SaveChanges();
                return Ok("Competition " + rFilter.Competition.Name + " Added!");
                
            }
            catch { return new BadRequestObjectResult("Could not connect to Backend"); ; }
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
                else if (_context.Competitions == null) {
                    _context.Competitions.Add(Comp);
                    _context.SaveChanges();
                    return Ok("Competition " + Comp.Name + " Added!");
                }
                else {
                    Competition dbCompetition = _context.Competitions.FirstOrDefault(c => c.Name == Comp.Name);
                    if (dbCompetition != null)
                        return new BadRequestObjectResult(Comp.Name + " already exists");
                    _notificationMessages.CompetitionNotification(Comp);
                    _context.Competitions.Add(Comp);
                    _context.SaveChanges();
                    dbCompetition = _context.Competitions.Last<Competition>();
                    string[] standards = { "Bronze", "Silver", "Gold" };
                    int[] defaults = { 40, 50, 60 };
                    for (int i = 0; i < 3; i++)
                    {
                        Requirement R = new Requirement {
                            Competition = dbCompetition,
                            Standard = standards[i],
                            Accuracy = defaults[i],
                            Total = defaults[i]
                        };
                        _context.Requirements.Add(R);
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Competition " + Comp.Name + " Added!");
                }
                }  
                else {
                return new BadRequestObjectResult("competition cannot be null");
            }
        }
        //A method that updates the status of the competition
        // POST: api/Competition/5
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCompetitionByStatus(int id, [FromBody]Competition comp)
        {
            if (ModelState.IsValid)
            {
                //error handling, check if client provided valid data
                if (comp == null) {
                    return new BadRequestObjectResult("competition cannot be null");
                }
                else if(_context.Competitions == null){
                    return BadRequest("Database is Empty!!!");
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
                return new BadRequestObjectResult("competition Model cannot be null");
            }
        }
        //A method that updates the competition standards / requirements by {Id}
        // POST: api/Competition/standard/5
        [HttpPost("/Requirements/{CompID}")]
        public async Task<IActionResult> UpdateRequirementsByCompID(int CompID, [FromBody] Requirement [] requirements)
        {
            if (ModelState.IsValid)
            {
                try {

                    //error handling, check if client provided valid data
                    if (requirements == null)
                        return new BadRequestObjectResult("requirements cannot be null");
                    else if (_context.Competitions.Count() == 0) {
                        return BadRequest("Database is currently empty!");
                    }
                    else
                    {
                        //assume the competition does not exist
                        Competition dbCompetition = _context.Competitions.Where(C => C.Id == CompID).FirstOrDefault<Competition>();
                        if (dbCompetition == null)
                        {
                            return BadRequest("Cannot update requirements, competition does not exist!");
                        }
                        List<Requirement> requirementsList = GetRequirementsByID(CompID).ToList<Requirement>();
                        for (int i = 0; i < requirements.Length; i++)
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
                catch {
                    return null;
                }
            }
            else
            {
                return new BadRequestObjectResult("Invalid requirements model!");
            }
        }
        //GET: /R/{CompID}
        /**Returns an array of requirements together with a null competition
         */
        [HttpGet("/R/{CompID}")]
        public IEnumerable<Requirement> GetRequirementsByID(int CompID)
        {
            List<Requirement> requirementList = new List<Requirement>();
            try {
                    requirementList = _context.Requirements.Where(R => R.Competition.Id == CompID).ToList<Requirement>();
                if (requirementList == null)
                    return new List<Requirement>();
                else return requirementList;
            } catch { return requirementList; }
            
        }
//##########################################################################################################
        //GET: api/competition/participants
        [HttpGet("participants")]
        public Dictionary<int, int> getUsersPerCompetition()
        {
            Dictionary<int, int> mapCompToNumUser = new Dictionary<int, int>();
            try {
                var competitionsList = this.GetCompetitions();
                    foreach (var comp in competitionsList)
                        {
                             int count = (from score in _context.Scores
                                            where score.Competition.Id == comp.Id
                                            select score.User.Id).Distinct().ToList().Count();
                             mapCompToNumUser.Add(comp.Id, count);
            }
                if (mapCompToNumUser == null)
                    return new Dictionary<int, int>();
                return mapCompToNumUser;
            } catch {
                return mapCompToNumUser;
            }
            
        }
    }
}
