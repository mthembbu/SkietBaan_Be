using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class Member {
        public int membershipID { get; set; }
        public string username { get; set; }
        public string emailAddress {get; set;} 

        public static implicit operator Task<object>(User v) {
            throw new NotImplementedException();
        }
    }
}
