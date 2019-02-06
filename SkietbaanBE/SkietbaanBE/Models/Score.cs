using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class Score {
        public int Id { get; set; }
        
        public int CompetitionID { get; set; }
        [ForeignKey("CompetitionID")]
        public Competition Competition { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }

        public int _Score { get; set; }
        public string PictureURL { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
