namespace Course_microservice.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public string UserId { get; set; } // No navigation to User (microservice separation)

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
