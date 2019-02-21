using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class Score {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual User User { get; set; }
        public int UserScore { get; set; }
        public string PictureURL { get; set; }
        public DateTime? UploadDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
