using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.RequestModel
{
    public class CSVFilter
    {
       public string[] getfilterName { get; set; }
       public string getAdminToken { get; set; }
    }
}
