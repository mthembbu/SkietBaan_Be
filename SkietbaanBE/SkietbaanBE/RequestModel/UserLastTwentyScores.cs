using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel {
    public class UserLastTwentyScores {
        public string CompetitionName { get; set; }
        public List<double> Scores { get; set; }
    }
}
