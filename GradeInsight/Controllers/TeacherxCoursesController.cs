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

namespace GradeInsight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherxCoursesController : ControllerBase
    {
        private readonly GradeInsightContext _context;

        public TeacherxCoursesController(GradeInsightContext context)
        {
            _context = context;
        }

        // GET: api/TeacherxCourses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherxCourse>>> GetTeacherxCourse()
        {
            var teacherxcourses = await _context.TeacherxCourse
                                          .Include(s => s.Teacher)
                                          .Include(s => s.Course)
                                          .ToListAsync();
            return Ok(teacherxcourses);
        }

        // GET: api/TeacherxCourses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherxCourse>> GetTeacherxCourse(int id)
        {
            var teacherxCourse = await _context.TeacherxCourse.FindAsync(id);

            if (teacherxCourse == null)
            {
                return NotFound();
            }

            return teacherxCourse;
        }

        // PUT: api/TeacherxCourses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacherxCourse(int id, TeacherxCourse teacherxCourse)
        {
            if (id != teacherxCourse.TeacherxCourseId)
            {
                return BadRequest();
            }

            
            var existingTeacherxCourse = await _context.TeacherxCourse.FindAsync(id);

            if (existingTeacherxCourse == null)
            {
                return NotFound();
            }

            // Update the properties of the existing teacher with the incoming teacher data, 
            // but keep the DateCreated and Deleted properties unchanged.
            existingTeacherxCourse.TeacherId = teacherxCourse.TeacherId;
            existingTeacherxCourse.CourseId = teacherxCourse.CourseId;


            // Add other properties that need to be updated as necessary

            // Save the changes to the database.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherxCourseExists(id))
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

        // POST: api/TeacherxCourses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeacherxCourse>> PostTeacherxCourse(TeacherxCourse teacherxCourse)
        {
            teacherxCourse.DateCreated = DateTime.Now;
            _context.TeacherxCourse.Add(teacherxCourse);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacherxCourse", new { id = teacherxCourse.TeacherxCourseId }, teacherxCourse);
        }

        // DELETE: api/TeacherxCourses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacherxCourse(int id)
        {
            var teacherxCourse = await _context.TeacherxCourse.FindAsync(id);
            if (teacherxCourse == null)
            {
                return NotFound();
            }

            _context.TeacherxCourse.Remove(teacherxCourse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherxCourseExists(int id)
        {
            return _context.TeacherxCourse.Any(e => e.TeacherxCourseId == id);
        }
    }
}
