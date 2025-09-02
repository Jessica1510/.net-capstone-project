using System.ComponentModel.DataAnnotations;

namespace UI_MVC_Worker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? SkillLevel { get; set; }
        public bool? IsActive { get; set; }

    }
}
