using Course_microservice.Data;
using Course_microservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Course_microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseDbContext _context;

        public CoursesController(CourseDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            return Ok(_context.Courses.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetCourseById(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        // Admin-only access
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateCourse(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateCourse(int id, Course updated)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            course.Title = updated.Title;
            course.Description = updated.Description;
            course.StartDate = updated.StartDate;
            course.EndDate = updated.EndDate;
            course.Mode = updated.Mode;
            course.Cost = updated.Cost;

            _context.SaveChanges();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
