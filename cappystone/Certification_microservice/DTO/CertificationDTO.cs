namespace Certification_microservice.DTO
{
    public class CertificationDTO
    {
        public int Id { get; set; }
        public string? WorkerId { get; set; }
        public int CourseId { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ValidTill { get; set; }
        public string CredentialNumber { get; set; }
    }
}
