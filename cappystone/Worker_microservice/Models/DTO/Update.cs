using System.ComponentModel.DataAnnotations;

namespace Worker_microservice.Models.DTO
{
    public class Update
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? SkillLevel { get; set; }
        public bool? isActive { get; set; }

        public string UserId { get; set; } = default!;
    }
}
