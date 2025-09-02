namespace Certification_microservice.Models
{
    public class Certification
    {
        public int Id { get; set; }
        public string WorkerId { get; set; }
        public int CourseId { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ValidTill { get; set; }
        public string CredentialNumber { get; set; }

        //public virtual Worker? Worker { get; set; }
        //public virtual Course? Course { get; set; }
    }
}
