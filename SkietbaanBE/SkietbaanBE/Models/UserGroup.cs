using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class UserGroup {
        public int Id { get; set; }

        public int UId { get; set; }
        [ForeignKey("UId")]
        public User User { get; set; }

        public int GId { get; set; }
        [ForeignKey("GId")]
        public Group Group { get; set; }
    }
}
