using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class OTP {
        [Key]
        public int UserId { get; set; }
        public DateTime OTPExpiry { get; set; }
        public int OneTimePassword { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
