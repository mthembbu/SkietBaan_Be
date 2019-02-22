﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib
{
    public class RankResults
    {
        public int Rank { get; set; }
        public string Username { get; set; }
        public int BestScore { get; set; }
        public int Total { get; set; }
        public int Average { get; set; }
        public bool ShowMore { get; set; }//extra for front-End UI collapse feature

    }
}
