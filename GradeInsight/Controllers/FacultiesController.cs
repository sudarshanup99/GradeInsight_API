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
    public class FacultiesController : ControllerBase
    {
        private readonly GradeInsightContext _context;

        public FacultiesController(GradeInsightContext context)
        {
            _context = context;
        }

        // GET: api/Faculties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Faculty>>> GetFaculty()
        {
            return await _context.Faculty.ToListAsync();
        }

        // GET: api/Faculties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Faculty>> GetFaculty(int id)
        {
            var faculty = await _context.Faculty.FindAsync(id);

            if (faculty == null)
            {
                return NotFound();
            }

            return faculty;
        }

        // PUT: api/Faculties/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFaculty(int id, Faculty faculty)
        {
            if (id != faculty.FacultyId)
            {
                return BadRequest();
            }

            _context.Entry(faculty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacultyExists(id))
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

        // POST: api/Faculties
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Faculty>> PostFaculty(Faculty faculty)
        {
            _context.Faculty.Add(faculty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFaculty", new { id = faculty.FacultyId }, faculty);
        }

        // DELETE: api/Faculties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            var faculty = await _context.Faculty.FindAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }

            _context.Faculty.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacultyExists(int id)
        {
            return _context.Faculty.Any(e => e.FacultyId == id);
        }
    }
}
