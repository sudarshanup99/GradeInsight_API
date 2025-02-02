using GradeInsight.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeInsight.SpecificRepositories.Courses
{
    public class CoursesRepositories:ICoursesRepositories
    {
        private readonly GradeInsightContext _context;


        public CoursesRepositories(GradeInsightContext context)
        {
            _context = context;

        }
        public async Task<Dictionary<string, int>> GetCourseCount()
        {
            var totalCourse = await _context.Course.CountAsync();

            return new Dictionary<string, int>
            {
                { "totalCourse", totalCourse }
            };
        }
    }
}
