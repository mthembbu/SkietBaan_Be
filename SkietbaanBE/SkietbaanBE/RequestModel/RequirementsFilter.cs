using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel
{
    public class RequirementsFilter
    {
        public int compID { get; set; }
        public Requirement [] requirements { get; set; }
    }
}
