
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_backend.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        public int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public virtual Faculty? Faculty { get; set; }

        [Required]
        [MaxLength(150)]
        public string DepartmentName { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}