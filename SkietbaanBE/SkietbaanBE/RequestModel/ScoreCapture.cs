﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel
{
    public class ScoreCapture
    {
        public string CompetitionName { get; set; }
        public string Username { get; set; }
        public int UserScore { get; set; }
        public string PictureURL { get; set; }
    }
}
