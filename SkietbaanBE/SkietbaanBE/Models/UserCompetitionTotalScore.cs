using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class UserCompetitionTotalScore {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Competition Competition { get; set; }
        public int Total { get; set; }
    }
}
