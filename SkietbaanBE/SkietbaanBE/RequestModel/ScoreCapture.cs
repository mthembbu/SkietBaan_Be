using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel
{
    public class ScoreCapture
    {
        public string CompetitionName { get; set; }
        public string Token { get; set; }
        public int UserScore { get; set; }
        public string PictureURL { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
