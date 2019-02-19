using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Statistic")]
    public class StatisticController : Controller
    {
        private ModelsContext context;
        public StatisticController(ModelsContext context) {
            this.context = context;
        }

        [HttpGet("{username}")]
        public int GetTotalScore(string username) {
            return StatsCalculator.TotalScore(context, username);
        }

        [HttpGet("accuracy/{username}")]
        public double GetAccuracy(string username) {
            return StatsCalculator.Accuracy(context, username);
        }
        
    }
}