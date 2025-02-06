using GradeInsight.Data;
using Microsoft.EntityFrameworkCore;

namespace GradeInsight.SpecificRepositories.Students
{
    public class StudentsRepositories : IStudentsRepositories
    {
        private readonly GradeInsightContext _context;
        public StudentsRepositories(GradeInsightContext context)
        {
            _context = context;

        }
        public async Task<Dictionary<string, int>> GetStudentCount()
        {
            var totalStudentinFaculty = await (from st in _context.Student.AsNoTracking()
                                               join ft in _context.Faculty.AsNoTracking()
                                               on st.FacultyId equals ft.FacultyId
                                               group st by ft.FacultyName into facultyGroup
                                               select new
                                               {
                                                   FacultyName = facultyGroup.Key,
                                                   StudentCount = facultyGroup.Count()
                                               })
                                               .ToDictionaryAsync(x => x.FacultyName, x => x.StudentCount);

            return totalStudentinFaculty;
        }


    }
}
