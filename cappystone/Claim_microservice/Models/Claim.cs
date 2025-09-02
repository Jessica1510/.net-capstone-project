namespace Claim_microservice.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public String WorkerId { get; set; } 
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        
    }
}
