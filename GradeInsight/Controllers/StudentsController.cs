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
using GradeInsight.SpecificRepositories.Students;

namespace GradeInsight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly GradeInsightContext _context;
        private readonly IStudentsRepositories _studentsRepositories;

        public StudentsController(GradeInsightContext context,IStudentsRepositories studentsRepositories)
        {
            _context = context;
            _studentsRepositories = studentsRepositories;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent()
        {
            var student= await _context.Student.Include(f=>f.Faculty).ToListAsync();
            return Ok(student);
        }
        [HttpGet("studentCount")]
        public async Task<IActionResult> GetStudentCount()
        {
            var totalStudentinFaculty = await _studentsRepositories.GetStudentCount();
            return Ok(totalStudentinFaculty);
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

            var existingStudent = await _context.Student.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            // Update the properties of the existing teacher with the incoming teacher data, 
            // but keep the DateCreated and Deleted properties unchanged.
            existingStudent.StudentName = student.StudentName;
                existingStudent.FacultyId = student.FacultyId;
            existingStudent.Address = student.Address;
            existingStudent.ContactNo = student.ContactNo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            student.DateCreated = DateTime.Now;
            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.StudentId == id);
        }
    }
}
