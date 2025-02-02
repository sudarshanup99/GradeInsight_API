namespace GradeInsight.SpecificRepositories.Students
{
    public interface IStudentsRepositories
    {
        public Task<Dictionary<string, int>> GetStudentCount();
    }
}
