using GradeInsight.ViewModel;
using Microsoft.AspNetCore.Mvc;
namespace GradeInsight.SpecificRepositories.Prediction
{
    public interface IPredictionRepositories
    {
        Task <object> PredictMarks(PredictionInitialDataViewModel inputData);
      
        Task TrainModel();
    }
}
