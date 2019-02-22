using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class LeaderInCompetition {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual User User { get; set; }
        public DateTime DateAtTop { get; set; }
    }
}
