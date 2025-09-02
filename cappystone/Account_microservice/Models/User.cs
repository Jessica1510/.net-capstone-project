using System.ComponentModel.DataAnnotations;

namespace Account_microservice.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string FullName{ get; set; }
        //public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public DateOnly DOB { get; set; }
        //public string? UserRole { get; set; } 
    }
}
