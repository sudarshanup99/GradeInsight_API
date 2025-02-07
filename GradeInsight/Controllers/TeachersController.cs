using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeInsight.Data;
using GradeInsight.Model;
using GradeInsight.SpecificRepositories.Teachers;

namespace GradeInsight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly GradeInsightContext _context;
        private readonly ITeachersRepositories _teachersRepositories;

        public TeachersController(GradeInsightContext context, ITeachersRepositories teachersRepositories)
        {
            _context = context;
            _teachersRepositories = teachersRepositories;
        }

        // GET: api/Teachers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeacher()
        {
            return await _context.Teacher.ToListAsync();
        }
        [HttpGet("teacherCount")]
        public async Task<IActionResult> GetTeacherCount()
        {
            var facultyCount = await _teachersRepositories.GetTeacherCount();
            return Ok(facultyCount);
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        // PUT: api/Teachers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return BadRequest();
            }

            var existingTeacher = await _context.Teacher.FindAsync(id);
            if (existingTeacher == null)
            {
                return NotFound();
            }

            // Update the properties of the existing teacher with the incoming teacher data, 
            // but keep the DateCreated and Deleted properties unchanged.
            existingTeacher.TeacherName = teacher.TeacherName;
            existingTeacher.Email = teacher.Email;
            existingTeacher.ContactNo = teacher.ContactNo;
           
            // Add other properties that need to be updated as necessary

            // Save the changes to the database.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
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


        // POST: api/Teachers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
        {
            var existingTeacher = await _context.Teacher
                                .FirstOrDefaultAsync(t => t.TeacherName == teacher.TeacherName
                                && t.ContactNo == teacher.ContactNo
                                && t.Email == teacher.Email);

            if (existingTeacher != null)
            {
                return BadRequest("A teacher with the same Name, Contact Number, and Email already exists.");
            }
            teacher.DateCreated = DateTime.Now;
           
            _context.Teacher.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacher", new { id = teacher.TeacherId }, teacher);
        }

        // DELETE: api/Teachers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.TeacherId == id);
        }
    }
}
