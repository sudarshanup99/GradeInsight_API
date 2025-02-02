namespace GradeInsight.SpecificRepositories.Faculties
{
    public interface IFacultiesRepositories
    {
        public  Task<Dictionary<string, int>> GetFacultyCount();
    }
}
