namespace GradeInsight.SpecificRepositories.Teachers
{
    public interface ITeachersRepositories
    {
        public Task<Dictionary<string, int>> GetTeacherCount();
    }
}
