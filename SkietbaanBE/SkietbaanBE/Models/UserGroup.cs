using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class UserGroup {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }

      
    }
}
