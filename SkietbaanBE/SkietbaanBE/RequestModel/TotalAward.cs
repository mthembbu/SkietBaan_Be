using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel {
    public class TotalAward {
        public bool Gold { get; set; }
        public bool Silver { get; set; }
        public bool Bronze { get; set; }
        public string GoldRequirementStatus { get; set; }
        public string SilverRequirementStatus { get; set; }
        public string BronzeRequirementStatus { get; set; }
    }
}
