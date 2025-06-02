using System.Collections.Generic;
using BusinessEntities;

namespace Core.Services.Users
{
    public interface IUpdateUserService
    {
        void Update(User user, string name, string email, UserTypes type, int age, decimal? annualSalary, IEnumerable<string> tags);
    }
}