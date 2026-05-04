using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backend.Models
{
    public class WasteBin
    {
        [Key]
        public int WasteBinId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BinCode { get; set; }

        public int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public virtual Faculty? Faculty { get; set; }

        public int WasteTypeId { get; set; }
        [ForeignKey("WasteTypeId")]
        public virtual WasteType? WasteType { get; set; }

        [Required]
        [MaxLength(150)]
        public string LocationName { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? CapacityLiter { get; set; }

        public decimal FillLevelPercent { get; set; } = 0;
        public bool IsCompostUnit { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
