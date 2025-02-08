using GradeInsight.Model;

namespace GradeInsight.SpecificRepositories.UserRepositories.cs
{
    public interface IUserRepositories
    {
        Task<User> GetUserDetailFromUserEmail(string email);
    }
}
