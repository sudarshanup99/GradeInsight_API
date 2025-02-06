using GradeInsight.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeInsight.SpecificRepositories.Faculties
{
    public class FacultiesRepositories : IFacultiesRepositories
    {
        private readonly GradeInsightContext _context;


        public FacultiesRepositories(GradeInsightContext context)
        {
            _context = context;

        }
        public async Task<Dictionary<string, int>> GetFacultyCount()
        {
            var totalFaculty = await _context.Faculty.CountAsync();

            return new Dictionary<string, int>
            {
                { "totalFaculty", totalFaculty }
            };
        }
    }
}
