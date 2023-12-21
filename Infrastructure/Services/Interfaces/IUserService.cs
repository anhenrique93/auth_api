using TaskCircle.AuthentcationApi.DTOs;

namespace TaskCircle.AuthentcationApi.Infrastructure.Services.Interfaces
{
    public interface IUserService
    {
        Task Register(UserDTO userDto);
        Task DeleteUser(int id);
        Task Update(UpdateUserDTO userDto);
        Task ChangePassword(UserDTO userDto);
        Task<UserDTO> GetUserByEmail(string email);
        Task<UserDTO> GetUserById(int id);
        Task<WhoAmIDTO> WhoAmI(int id);
        Task RefreshToken(UserDTO userDto);

        Task RemoveRefreshToken(WhoAmIDTO userDto);
    }
}