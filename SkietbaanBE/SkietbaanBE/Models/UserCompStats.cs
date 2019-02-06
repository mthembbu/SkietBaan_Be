using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class UserCompStats {

        public int Id { get; set; }

        public int _UserID { get; set; }
        [ForeignKey("_UserID")]
        public User User { get; set; }

        public int _CompID { get; set; }
        [ForeignKey("_CompID")]
        public Competition Competition { get; set; }

        public int BestScore { get; set; }
        public int CompScore { get; set; }
        public DateTime Year { get; set; }

    }
}
