using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        // GET: api/Test
        [HttpGet]
        public string Get()
        {
            return "Connected to SkietBaan Web Api";
        }
    }
}
