using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib
{
    public class LeaderboardResults
    {
        public List<RankResults> RankResults {get;set;}
        public RankResults UserResults { get; set; }
    }
}
