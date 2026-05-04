using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backend.Models
{

    public class PointsTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int? WasteRecordId { get; set; }
        [ForeignKey("WasteRecordId")]
        public virtual WasteRecord? WasteRecord { get; set; }

        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; }

        public int Points { get; set; }

        [Required]
        [MaxLength(255)]
        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
