
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backend.Models
{

    public class UserBadge
    {
        [Key]
        public int UserBadgeId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int BadgeId { get; set; }
        [ForeignKey("BadgeId")]
        public virtual Badge? Badge { get; set; }

        public DateTime EarnedAt { get; set; } = DateTime.Now;
    }
}
