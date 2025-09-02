using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Course_microservice.Data;
using Course_microservice.Models;
using Course_microservice.Services;

public class SignUpRequest
{
    public string UserId { get; set; }
    public int CourseId { get; set; }
}

namespace Course_microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly CourseDbContext _context;
        private readonly UserService _userService;

        public EnrollmentController(CourseDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }
        
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return BadRequest("Invalid request data.");

            try
            {
                if (!await _userService.IsUserValid(request.UserId))
                    return BadRequest("User does not exist.");

                var existing = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == request.UserId && e.CourseId == request.CourseId);

                if (existing != null)
                    return BadRequest("User is already enrolled in this course.");

                var enrollment = new Enrollment
                {
                    UserId = request.UserId,
                    CourseId = request.CourseId
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return Ok("Enrollment successful.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        [HttpDelete("drop")]
        public async Task<IActionResult> Drop(string userId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Course dropped successfully.");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserEnrollments(string userId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .Select(e => new
                {
                    e.Course.Id,
                    e.Course.Title,
                    e.Course.Description,
                    e.Course.StartDate,
                    e.Course.EndDate,
                    e.Course.Mode,
                    e.Course.Cost,
                    e.EnrolledAt
                })
                .ToListAsync();

            return Ok(enrollments);
        }
    }
}
