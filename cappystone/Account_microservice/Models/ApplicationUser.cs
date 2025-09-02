using Microsoft.AspNetCore.Identity;

namespace Account_microservice.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? FullName {  get; set; }
        public string? SkillLevel { get; set; }
        public string? Address { get; set; }


    }
}
