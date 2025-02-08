using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeInsight.Data;
using GradeInsight.Model;
using NuGet.Protocol.Plugins;
using GradeInsight.SpecificRepositories.Courses;
using GradeInsight.SpecificRepositories.Faculties;
using GradeInsight.Utilities;

namespace GradeInsight.Controllers
{
    [ApiKeyAuth]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly GradeInsightContext _context;
        private readonly ICoursesRepositories _coursesRepositories;

        public CoursesController(GradeInsightContext context, ICoursesRepositories coursesRepositories)
        {
            _context = context;
            _coursesRepositories = coursesRepositories;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            var course = await _context.Course
                                        .Include(s => s.Semester)
                                        .ToListAsync();
            return Ok(course);
        }
        [HttpGet("courseCount")]
        public async Task<IActionResult> GetFacultyCount()
        {
            var courseCount = await _coursesRepositories.GetCourseCount();
            return Ok(courseCount);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            var existingCourse = await _context.Course.FindAsync(id);
            if (existingCourse == null)
            {
                return NotFound();
            }

            // Update the properties of the existing teacher with the incoming teacher data, 
            // but keep the DateCreated and Deleted properties unchanged.
            existingCourse.CourseName = course.CourseName;
            existingCourse.SemesterId=course.SemesterId;
            


            // Add other properties that need to be updated as necessary

            // Save the changes to the database.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

      
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            // Check if a course with the same CourseName and SemesterId already exists
            var existingCourse = await _context.Course
                                .FirstOrDefaultAsync(c => c.CourseName == course.CourseName && c.SemesterId == course.SemesterId);

            if (existingCourse != null)
            {
                return BadRequest("A course with the same name already exists in this semester.");
            }
            course.DateCreated = DateTime.Now;
            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
