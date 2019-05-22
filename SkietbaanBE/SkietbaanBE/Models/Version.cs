
using System.ComponentModel.DataAnnotations;

namespace SkietbaanBE.Models {
    public class Version {
        [Key]
        public int Id { get; set; }
        public string VersionNumber { get; set; }
    }
}
