using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class Competition {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<UserCompStats> UserCompStats { get; set; }
        public Competition() {
            Scores = new HashSet<Score>();
            UserCompStats = new HashSet<UserCompStats>();
        }
    }
}

