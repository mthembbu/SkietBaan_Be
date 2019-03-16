using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class TimeSpent {

        [Key]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Key]
        public int CompetitionId { get; set; }
        [ForeignKey("CompetitionId")]
        public virtual Competition Competition { get; set; }
        public int HoursSpent { get; set; }
    }
}
