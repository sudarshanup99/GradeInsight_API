namespace GradeInsight.SpecificRepositories.Courses
{
    public interface ICoursesRepositories
    {
        public Task<Dictionary<string, int>> GetCourseCount();
    }
}
