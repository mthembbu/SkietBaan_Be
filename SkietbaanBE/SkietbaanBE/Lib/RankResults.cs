using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib
{
    public class RankResults
    {
        public int Rank { get; set; }
        public string Username { get; set; }
        public double Best { get; set; }
        public double Total { get; set; }
        public double Average { get; set; }
        public bool isMember { get; set; }
        public bool isCompetitiveShooter { get; set; }
        public bool IsIncreasing { get; set; }
    }
}
