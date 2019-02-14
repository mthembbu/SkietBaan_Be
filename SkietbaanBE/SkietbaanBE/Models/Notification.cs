using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkietbaanBE.Models {
    public class Notification {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool isRead { get; set; }
        public string NotificationMessage { get; set; }
        public string NotitficationContent { get; set; }
    }
}
