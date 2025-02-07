using GradeInsight.ViewModel;
namespace GradeInsight.SpecificRepositories.Prediction
{
    public interface IPredictionRepositories
    {
        Task <double> PredictMarks(PredictionInitialDataViewModel inputData);
        Task<object> TrainModels();
        Task TrainModel();
    }
}
