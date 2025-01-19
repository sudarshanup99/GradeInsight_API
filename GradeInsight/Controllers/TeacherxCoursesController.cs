using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeInsight.Data;
using GradeInsight.Model;

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
            return await _context.TeacherxCourse.ToListAsync();
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
            if (id != teacherxCourse.TeacherXcourseId)
            {
                return BadRequest();
            }

            _context.Entry(teacherxCourse).State = EntityState.Modified;

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
            _context.TeacherxCourse.Add(teacherxCourse);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacherxCourse", new { id = teacherxCourse.TeacherXcourseId }, teacherxCourse);
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
            return _context.TeacherxCourse.Any(e => e.TeacherXcourseId == id);
        }
    }
}
