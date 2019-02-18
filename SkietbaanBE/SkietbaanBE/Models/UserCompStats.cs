using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class UserCompStats {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Competition Competition { get; set; }
        public int BestScore { get; set; }
        public int CompScore { get; set; }
        public int Total { get; set; }
        public int Year { get; set; }
    }
}
