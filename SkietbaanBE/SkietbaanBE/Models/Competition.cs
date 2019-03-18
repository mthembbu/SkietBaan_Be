using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SkietbaanBE.Models {
    public class Competition {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public int BestScoresNumber { get; set; }
        public int Hours { get; set; }
    }
}

