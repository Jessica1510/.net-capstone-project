using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Worker_microservice.Models
{
    public class Worker
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        [Required]
        public string? FullName { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? SkillLevel { get; set; }
        public bool? IsActive { get; set; }

        //public virtual ICollection<Certification>? Certifications { get; set; }
        //public virtual ICollection<Claim>? Claims { get; set; }
        //public virtual  ICollection<Enrollment>? Enrollments { get; set; }
    }
}
