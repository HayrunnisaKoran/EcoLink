using System.ComponentModel.DataAnnotations;

namespace web_backend.Models
{

    public class Faculty
    {
        [Key]
        public int FacultyId { get; set; }

        [Required]
        [MaxLength(150)]
        public string FacultyName { get; set; }

        [MaxLength(150)]
        public string? CampusName { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // İlişkiler
        public virtual ICollection<Department>? Departments { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}

