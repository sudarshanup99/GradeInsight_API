using GradeInsight.Model;
using GradeInsight.ViewModel;
namespace GradeInsight.SpecificRepositories.Marks
{
    public interface IMarksRepositories
    {
        Task <List<ResultDataVM>> GetResultData();

        Task<List<ResultDataVM>> GetStudentResultData(int id);

        Task<Dictionary<string, string>> GetResultInsight();

        Task<List<Object>> GetCourseAverages();


    }
}
