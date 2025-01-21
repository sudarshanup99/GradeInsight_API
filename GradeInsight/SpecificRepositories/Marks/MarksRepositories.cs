using GradeInsight.Data;
using GradeInsight.Model;
using GradeInsight.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace GradeInsight.SpecificRepositories.Marks
{
    public class MarksRepositories:IMarksRepositories
    {
        private readonly GradeInsightContext _context;


        public MarksRepositories(GradeInsightContext context)
        {
            _context = context;
            
        }
        public async Task<List<ResultDataVM>> GetResultData()
        {
            var result = await (from f in _context.Faculty.AsNoTracking()
                                join s in _context.Semester.AsNoTracking()
                                on f.FacultyId equals s.FacultyId
                                join c in _context.Course.AsNoTracking()
                                on s.SemesterId equals c.SemesterId
                                join m in _context.Marks.AsNoTracking()
                                on c.CourseId equals m.CourseId
                                join st in _context.Student.AsNoTracking()
                                on m.StudentId equals st.StudentId
                                join et in _context.ExamType.AsNoTracking()
                                on m.ExamTypeId equals et.ExamTypeId
                                select new ResultDataVM
                                {
                                    StudentId = st.StudentId,
                                    StudentName = st.StudentName,
                                    FacultyId = f.FacultyId,
                                    FacultyName = f.FacultyName,
                                    SemesterId = s.SemesterId,
                                    SemesterName = s.SemesterName,
                                   
                                    Marks = new MarksDataVM
                                    {
                                        Mark = m.Mark,
                                        CourseId = c.CourseId, 
                                        CourseName=c.CourseName,
                                        ExamTypeId = et.ExamTypeId,
                                        ExamTypeName = et.ExamTypeName
                                    }
                                }).ToListAsync();

            return result;
        }

    }
}
