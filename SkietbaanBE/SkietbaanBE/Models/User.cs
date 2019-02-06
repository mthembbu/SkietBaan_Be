using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MemberID { get; set; }
        public string Email { get; set; }
        public DateTime MemberExpiry { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Admin { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<UserCompStats> UserCompStats { get; set; }

        public User() {
            UserGroups = new HashSet<UserGroup>();
            Scores = new HashSet<Score>();
            UserCompStats = new HashSet<UserCompStats>();
        }

        public static implicit operator Task<object>(User v) {
            throw new NotImplementedException();
        }
    }
}
