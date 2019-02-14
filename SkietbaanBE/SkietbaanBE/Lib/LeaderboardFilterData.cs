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
        public List<Group> groups { get; set; }
        public List<Competition> competitions { get; set; }
        public List<Group1> groups1 { get; set; }
        public List<Competition1> competitions1 { get; set; }
    }
}
