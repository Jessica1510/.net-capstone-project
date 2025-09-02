using System.ComponentModel.DataAnnotations;

namespace Worker_microservice.Models.DTO
{
    public class Create
    {
        [Required]
        public string? FullName { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? SkillLevel { get; set; }
        public bool? IsActive { get; set; }

        public string UserId { get; set; } = default!;


    }
}
