using System.ComponentModel.DataAnnotations;
namespace web_backend.Models
{

    public class WasteType
    {
        [Key]
        public int WasteTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public int BasePoint { get; set; } = 0;

        [MaxLength(20)]
        public string? ColorCode { get; set; }

        [MaxLength(80)]
        public string? IconName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
