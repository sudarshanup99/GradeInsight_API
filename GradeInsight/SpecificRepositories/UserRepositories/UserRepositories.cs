using GradeInsight.Data;
using GradeInsight.Model;
using Microsoft.EntityFrameworkCore;

namespace GradeInsight.SpecificRepositories.UserRepositories.cs
{
    public class UserRepositories : IUserRepositories
    {
        private readonly GradeInsightContext _context;


        public UserRepositories(GradeInsightContext context)
        {
            _context = context;

        }
        public async Task<User> GetUserDetailFromUserEmail(string email)
        {
            return await _context.User
                .Where(user => user.UserEmail == email)
                .FirstOrDefaultAsync();
        }
    }
}