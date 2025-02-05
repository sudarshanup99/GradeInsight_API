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
    public class MarksRepositories : IMarksRepositories
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
        public async Task<Dictionary<string, string>> GetResultInsight()
        {
            var marks = await _context.Marks.AsNoTracking().ToListAsync();

            if (!marks.Any())
            {
                return new Dictionary<string, string>
                {
                    { "Average Mark", "N/A" },
                    { "Highest Mark", "N/A" },
                    { "Lowest Mark", "N/A" },
                    { "Total Marks Recorded", "N/A" },
                    { "Pass Rate", "N/A" },
                    { "Fail Rate", "N/A" }
                };
            }

            int highestMark = marks.Max(m => m.Mark);
            int lowestMark = marks.Min(m => m.Mark);
            double averageMark = marks.Average(m => m.Mark);
            int totalMarks = marks.Select(m => m.MarksId).Distinct().Count();
            

            int passCount = marks.Count(m => m.Mark >= 40); // Assuming 40 is the pass mark
            int failCount = marks.Count(m => m.Mark < 40);
            double passRate = totalMarks > 0 ? ((double)passCount / totalMarks) * 100 : 0;
            double failRate = 100 - passRate;

            var insights = new Dictionary<string, string>
            {
                { "Average Mark", $"{averageMark:F1}" },
                { "Highest Mark", highestMark.ToString() },
                { "Lowest Mark", lowestMark.ToString() },
                { "Total Marks Recorded", totalMarks.ToString("N0") }, // Formats with commas
                { "Pass Rate", $"{passRate:F0}%" },
                { "Fail Rate", $"{failRate:F0}%" }
            };

            return insights;
        }

        public async Task<List<object>> GetCourseAverages()
        {
            var result = await (from f in _context.Faculty.AsNoTracking()
                                join s in _context.Semester.AsNoTracking() on f.FacultyId equals s.FacultyId
                                join c in _context.Course.AsNoTracking() on s.SemesterId equals c.SemesterId
                                join m in _context.Marks.AsNoTracking() on c.CourseId equals m.CourseId
                                select new
                                {
                                    facultyName = f.FacultyName,
                                    semesterName = s.SemesterName,
                                    courseName = c.CourseName,
                                    mark = m.Mark
                                })
                            .ToListAsync();

            // Group by Course, Faculty, and Semester to calculate average marks
            var groupedResult = result
                                .GroupBy(x => new { x.courseName, x.facultyName, x.semesterName })
                                .Select(g => new
                                {
                                    course = g.Key.courseName,
                                    avg =Math.Round( g.Average(m => m.mark)),
                                    faculty = g.Key.facultyName,
                                    semester = g.Key.semesterName
                                })
                                .ToList<object>(); // Convert to List<object>

            return groupedResult;
        }

        public async Task<List<ResultDataVM>> GetStudentResultData(int id)
        {
            var result = await (from st in _context.Student.AsNoTracking()
                                join m in _context.Marks.AsNoTracking() on st.StudentId equals m.StudentId
                                join c in _context.Course.AsNoTracking() on m.CourseId equals c.CourseId
                                join s in _context.Semester.AsNoTracking() on c.SemesterId equals s.SemesterId
                                join f in _context.Faculty.AsNoTracking() on s.FacultyId equals f.FacultyId
                                join et in _context.ExamType.AsNoTracking() on m.ExamTypeId equals et.ExamTypeId
                                select new
                                {
                                    StudentId = st.StudentId,
                                    StudentName = st.StudentName,
                                    FacultyId = f.FacultyId,
                                    FacultyName = f.FacultyName,
                                    SemesterId = s.SemesterId,
                                    SemesterName = s.SemesterName,
                                    Marks = new
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
                                    x.StudentId,
                                    x.StudentName,
                                    x.FacultyId,
                                    x.FacultyName,
                                    x.SemesterId,
                                    x.SemesterName
                                })
                                .Select(g => new ResultDataVM
                                {
                                    StudentId = g.Key.StudentId,
                                    StudentName = g.Key.StudentName,
                                    FacultyId = g.Key.FacultyId,
                                    FacultyName = g.Key.FacultyName,
                                    SemesterId = g.Key.SemesterId,
                                    SemesterName = g.Key.SemesterName,
                                    Marks = g.Select(m => new MarksDataVM
                                    {
                                        MarksId = m.Marks.MarksId,
                                        Mark = m.Marks.Mark,
                                        CourseId = m.Marks.CourseId,
                                        CourseName = m.Marks.CourseName,
                                        ExamTypeId = m.Marks.ExamTypeId,
                                        ExamTypeName = m.Marks.ExamTypeName
                                    }).ToList()
                                })
                                .ToList();

            return groupedResult;
        }




    }
}
