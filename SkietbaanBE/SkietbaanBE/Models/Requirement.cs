using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class Requirement {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Competition  Competition{ get; set;}
        public string Standard { get; set; }
        public double Accuracy { get; set; }
        public double Total { get; set; }
}
}
