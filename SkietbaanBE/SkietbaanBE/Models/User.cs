using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class User {

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MemberID { get; set; }
        public string Email { get; set; }
        public DateTime MemberExpiry { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Admin { get; set; }

        public static implicit operator Task<object>(User v) {
            throw new NotImplementedException();
        }
    }
}
