using SkietbaanBE.Controllers;
using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib
{
    public class LeaderboardFilterData
    {
        public User user { get; set; }
        public RankResults rankResult {get; set;}
        public List<GroupLabel> groups1 { get; set; }
        public List<CompetitionLabel> competitions1 { get; set; }
    }
}
