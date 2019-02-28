using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkietbaanBE.Models;

namespace SkietbaanBE.RequestModel
{
    public class Filter
    {
       
        public int GroupIds { get; set; }
        public User[] users { get; set; }
       
    }
}
