using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SkietbaanBE.Models {
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MemberID { get; set; }
        public string Email { get; set; }
        public DateTime MemberExpiry { get; set; }
        public DateTime EntryDate { get; set; }
        public bool Admin { get; set; }
        public string Token { get; set; }
    }
}

