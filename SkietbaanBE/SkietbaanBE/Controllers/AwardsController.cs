using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;
using System.Collections.Generic;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Awards")]
    public class AwardsController : Controller
    {
        private ModelsContext context;
        public AwardsController(ModelsContext context) {
            this.context = context;
        }

        [HttpGet("{username}")]
        public IEnumerable<Award> GetAward(string username) {
            var awards = StatsCalculator.GetAward(context, username);

            return awards;
        }

    }
}