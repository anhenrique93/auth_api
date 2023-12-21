using AutoMapper;
using TaskCircle.AuthentcationApi.Models;

namespace TaskCircle.AuthentcationApi.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile() 
    { 
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<User, WhoAmIDTO>().ReverseMap();
        CreateMap<User, UpdateUserDTO>().ReverseMap();
        CreateMap<User, ChangePasswordDTO>().ReverseMap();
    }
   
}
