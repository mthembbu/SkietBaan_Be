using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models
{
    public class UserGroup
    {
        [Key]
        public int GroupId { get; set; }
        [Key]
        public int UserId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
