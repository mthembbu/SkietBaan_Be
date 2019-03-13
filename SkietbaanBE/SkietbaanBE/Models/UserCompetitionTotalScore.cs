using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class UserCompetitionTotalScore {

        [Key]
        public int CompetitionId { get; set; }
        [Key]
        public int UserId { get; set; }
        [ForeignKey("CompetitionId")]
        public virtual Competition Competition { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public double Best { get; set; }
        public double Total { get; set; }
        public double Average { get; set; }
        public int Year { get; set; }
    }
}
