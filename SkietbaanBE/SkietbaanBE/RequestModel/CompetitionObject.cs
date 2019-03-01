using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel
{
    public class CompetitionObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public string BestScoresNumber { get; set; }
    }
}
