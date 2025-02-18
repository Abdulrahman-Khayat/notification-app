using AutoMapper;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Profiles;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, ReadUserDto>();
        CreateMap<CreateUserDto, User>();
    }
}