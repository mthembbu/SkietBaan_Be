using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class Group {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }

        public Group() {
            UserGroups = new HashSet<UserGroup>();
        }
    }
}
