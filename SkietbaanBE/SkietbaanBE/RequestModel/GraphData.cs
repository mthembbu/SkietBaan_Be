using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel {
    public class GraphData {
        public string CompetitionName { get; set; }
        public double CompetitionMaximum { get; set; }
        public List<Data> Data { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Average { get; set; }
        public bool IsParticipating { get; set; }
    }
}
