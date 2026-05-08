using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace web_backend.Models
{

    public class WasteRecord
    {
        [Key]
        public int WasteRecordId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int WasteBinId { get; set; }
        [ForeignKey("WasteBinId")]
        public virtual WasteBin? WasteBin { get; set; }

        public int WasteTypeId { get; set; }
        [ForeignKey("WasteTypeId")]
        public virtual WasteType? WasteType { get; set; }

        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        public decimal? Amount { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; } = "adet";

        public bool GpsVerified { get; set; } = false;
        public bool PhotoVerified { get; set; } = false;
        public bool SensorVerified { get; set; } = false;

        [MaxLength(30)]
        public string VerificationStatus { get; set; } = "Pending";

        public int EarnedPoints { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // enlem,Boylam bilgisi
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
