using GradeInsight.Model;
using GradeInsight.ViewModel;
namespace GradeInsight.SpecificRepositories.Marks
{
    public interface IMarksRepositories
    {
        Task <List<ResultDataVM>> GetResultData();

    }
}
