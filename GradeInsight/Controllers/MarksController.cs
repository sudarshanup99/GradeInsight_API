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
using GradeInsight.SpecificRepositories.Marks;
using GradeInsight.ViewModel;
using GradeInsight.Utilities;

namespace GradeInsight.Controllers
{
    [ApiKeyAuth]
    [Route("api/[controller]")]
    [ApiController]
    public class MarksController : ControllerBase
    {
        private readonly GradeInsightContext _context;
        private readonly IMarksRepositories _marksRepositories;

        public MarksController(GradeInsightContext context, IMarksRepositories  marksRepositories)
        {
            _context = context;
            _marksRepositories = marksRepositories;
        }

        // GET: api/Marks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marks>>> GetMarks()
        {
            return await _context.Marks.ToListAsync();
        }
        [HttpGet("result")]
        public async Task<ActionResult<List<ResultDataVM>>> GetMarkFromStudent()
        {
            return await _marksRepositories.GetResultData();
            

        }
        [HttpGet("resultInsight")]
        public async Task<IActionResult> GetResultInsight()
        {
            var resultInsight = await _marksRepositories.GetResultInsight();

            if (resultInsight == null || !resultInsight.Any())
            {
                return NotFound(new { message = "No marks data available." });
            }

            return Ok(resultInsight);
        }
        [HttpGet("courseAverages")]
        public async Task<IActionResult> GetCourseAverages()
        {
            var courseAverage = await _marksRepositories.GetCourseAverages();

            if (courseAverage == null || !courseAverage.Any())
            {
                return NotFound(new { message = "No marks data available." });
            }

            return Ok(courseAverage);
        }
        // GET: api/Marks/student/{id}
        [HttpGet("student/{id}")]
        public async Task<ActionResult<List<ResultDataVM>>> GetStudentResultData(int id)
        {
            var studentResult = await _marksRepositories.GetStudentResultData(id);

            if (studentResult == null || !studentResult.Any())
            {
                return NotFound(new { message = "No results found for this student." });
            }

            return studentResult;
        }

        // GET: api/Marks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Marks>> GetMarks(int id)
        {
            var marks = await _context.Marks.FindAsync(id);

            if (marks == null)
            {
                return NotFound();
            }

            return marks;
        }

        // PUT: api/Marks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarks(int id, Marks marks)
        {
            if (id != marks.MarksId)
            {
                return BadRequest();
            }

            var existingMarks = await _context.Marks.FindAsync(id);
            if (existingMarks == null)
            {
                return NotFound();
            }

            // Update the properties of the existing teacher with the incoming teacher data, 
            // but keep the DateCreated and Deleted properties unchanged.
            existingMarks.Mark = marks.Mark;
            existingMarks.CourseId= marks.CourseId;
            existingMarks.StudentId = marks.StudentId;
            existingMarks.ExamTypeId = marks.ExamTypeId;



            // Add other properties that need to be updated as necessary

            // Save the changes to the database.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarksExists(id))
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

        // POST: api/Marks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Marks>> PostMarks(Marks marks)
        {
            var existingMark = await _context.Marks
        .FirstOrDefaultAsync(m => m.StudentId == marks.StudentId
                               && m.CourseId == marks.CourseId
                               && m.ExamTypeId == marks.ExamTypeId);

            if (existingMark != null)
            {
                return BadRequest("Marks for this student in the selected course and exam type already exist.");
            }

            marks.DateCreated = DateTime.Now;
            _context.Marks.Add(marks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMarks", new { id = marks.MarksId }, marks);
        }

        // DELETE: api/Marks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarks(int id)
        {
            var marks = await _context.Marks.FindAsync(id);
            if (marks == null)
            {
                return NotFound();
            }

            _context.Marks.Remove(marks);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MarksExists(int id)
        {
            return _context.Marks.Any(e => e.MarksId == id);
        }
    }
}
