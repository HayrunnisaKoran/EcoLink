using System.ComponentModel.DataAnnotations;

namespace web_backend.Models
{

    public class Badge
    {
        [Key]
        public int BadgeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string BadgeName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        public string? ImageUrl { get; set; }

        public int? RequiredPoints { get; set; }
        public int? RequiredRecordCount { get; set; }

        [MaxLength(80)]
        public string? IconName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
