using GradeInsight.Data;
using GradeInsight.Model;
using GradeInsight.ViewModel;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
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
                            join s in _context.Semester.AsNoTracking() on f.FacultyId equals s.FacultyId
                            join c in _context.Course.AsNoTracking() on s.SemesterId equals c.SemesterId
                            join m in _context.Marks.AsNoTracking() on c.CourseId equals m.CourseId
                            join st in _context.Student.AsNoTracking() on m.StudentId equals st.StudentId
                            join et in _context.ExamType.AsNoTracking() on m.ExamTypeId equals et.ExamTypeId
                            
                            select new
                            {
                                studentId = st.StudentId,
                                studentName = st.StudentName,
                                facultyId = f.FacultyId,
                                facultyName = f.FacultyName,
                                semesterId = s.SemesterId,
                                semesterName = s.SemesterName,
                                Marks = new MarksDataVM
                                {
                                    MarksId = m.MarksId,
                                    Mark = m.Mark,
                                    CourseId = c.CourseId,
                                    CourseName = c.CourseName,
                                    ExamTypeId = et.ExamTypeId,
                                    ExamTypeName = et.ExamTypeName
                                }
                            })
                        .ToListAsync();

            var groupedResult = result
                                .GroupBy(x => new
                                {
                                    x.studentId,
                                    x.studentName,
                                    x.facultyId,
                                    x.facultyName,
                                    x.semesterId,
                                    x.semesterName
                                })
                                .Select(g => new ResultDataVM
                                {
                                    StudentId = g.Key.studentId,
                                    StudentName = g.Key.studentName,
                                    FacultyId = g.Key.facultyId,
                                    FacultyName = g.Key.facultyName,
                                    SemesterId = g.Key.semesterId,
                                    SemesterName = g.Key.semesterName,
                                    Marks = g.Select(m => m.Marks).ToList()
                                })
                                .ToList();

            return groupedResult;
        }

    }
}
