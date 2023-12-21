using AutoMapper;
using TaskCircle.AuthentcationApi.DTOs;
using TaskCircle.AuthentcationApi.Infrastructure.Repositories.Iterfaces;
using TaskCircle.AuthentcationApi.Infrastructure.Services.Interfaces;
using TaskCircle.AuthentcationApi.Models;

namespace TaskCircle.AuthentcationApi.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository? _userRepository;
    private readonly IMapper? _mapper;

    public UserService(IUserRepository? userRepository, IMapper? mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task Register(UserDTO userDto)
    {
        var userEntity = _mapper?.Map<User>(userDto);
        await _userRepository.Register(userEntity);
    }

    public async Task DeleteUser(int id)
    {
        User user = await _userRepository?.GetUserById(id);

        await _userRepository?.Delete(user.IdUser);
    }

    public async Task<UserDTO> GetUserByEmail(string email)
    {
        var userEntity = await _userRepository.GetUserByEmail(email);
        return _mapper.Map<UserDTO>(userEntity);
    }

    public async Task RefreshToken(UserDTO userDto)
    {
        var updatedUser = _mapper?.Map<User>(userDto);

        await _userRepository.RefreshToken(updatedUser);
    }

    public async Task<UserDTO>? GetUserById(int id)
    {
        var userEntity = await _userRepository.GetUserById(id);
        return _mapper?.Map<UserDTO>(userEntity);
    }

    public async Task<WhoAmIDTO> WhoAmI(int id)
    {
        var userEntity = await _userRepository.GetUserById(id);
        return _mapper?.Map<WhoAmIDTO>(userEntity);
    }

    public async Task Update(UpdateUserDTO updateUserDTO)
    {
        var userEntity = _mapper?.Map<User>(updateUserDTO);
        await _userRepository.Update(userEntity);
    }

    public async Task ChangePassword(UserDTO userDto)
    {
        var userEntity = _mapper?.Map<User>(userDto);
        await _userRepository.UpdatePassword(userEntity);
    }

    public async Task RemoveRefreshToken(WhoAmIDTO userDto)
    {
        var userEntity = _mapper?.Map<User>(userDto);
        await _userRepository.RemoveRefreshToken(userEntity);
    }
}