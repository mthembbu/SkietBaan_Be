using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel
{
    public class CreateGroup
    {
        public string name { get; set; }
        public User[] users { get; set; }
    }
}
