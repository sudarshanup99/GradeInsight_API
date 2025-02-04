using GradeInsight.Model;
using GradeInsight.ViewModel;
namespace GradeInsight.SpecificRepositories.Marks
{
    public interface IMarksRepositories
    {
        Task <List<ResultDataVM>> GetResultData();
        Task<Dictionary<string, string>> GetResultInsight();

        Task<List<Object>> GetCourseAverages();


    }
}
