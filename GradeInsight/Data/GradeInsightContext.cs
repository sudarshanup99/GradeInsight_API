using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GradeInsight.Model;

namespace GradeInsight.Data
{
    public class GradeInsightContext : DbContext
    {
        public GradeInsightContext (DbContextOptions<GradeInsightContext> options)
            : base(options)
        {
        }

        public DbSet<GradeInsight.Model.Course> Course { get; set; } = default!;
        public DbSet<GradeInsight.Model.Faculty> Faculty { get; set; } = default!;
        public DbSet<GradeInsight.Model.Marks> Marks { get; set; } = default!;
        public DbSet<GradeInsight.Model.Semester> Semester { get; set; } = default!;
        public DbSet<GradeInsight.Model.Student> Student { get; set; } = default!;
        public DbSet<GradeInsight.Model.Teacher> Teacher { get; set; } = default!;
        public DbSet<GradeInsight.Model.TeacherxCourse> TeacherxCourse { get; set; } = default!;
        public DbSet<GradeInsight.Model.ExamType> ExamType { get; set; } = default!;
        public DbSet<GradeInsight.Model.UserType> UserType { get; set; } = default!;
        public DbSet<GradeInsight.Model.User> User { get; set; } = default!;
    }
}
