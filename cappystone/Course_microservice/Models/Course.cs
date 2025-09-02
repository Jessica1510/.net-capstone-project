using System.Security.Claims;

namespace Course_microservice.Models
{
    
        public class Course
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Mode { get; set; }
            public decimal Cost { get; set; }

            public virtual ICollection<Enrollment> Enrollments { get; set; }
        }
}
