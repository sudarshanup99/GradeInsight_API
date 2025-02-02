using GradeInsight.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeInsight.SpecificRepositories.Teachers
{
    public class TeachersRepositories:ITeachersRepositories
    {
        private readonly GradeInsightContext _context;


        public TeachersRepositories(GradeInsightContext context)
        {
            _context = context;

        }
        public async Task<Dictionary<string, int>> GetTeacherCount()
        {
            var totalTeacher = await _context.Teacher.CountAsync();

            return new Dictionary<string, int>
            {
                { "totalTeacher", totalTeacher }
            };
        }
    }
}
