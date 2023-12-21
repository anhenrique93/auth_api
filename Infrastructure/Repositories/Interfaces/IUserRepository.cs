using TaskCircle.AuthentcationApi.DTOs;
using TaskCircle.AuthentcationApi.Models;

namespace TaskCircle.AuthentcationApi.Infrastructure.Repositories.Iterfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserByEmail(string email);

    Task<User> RefreshToken(User user);
    Task<User> UpdatePassword(User Entity);
    Task RemoveRefreshToken(User Entity);
}