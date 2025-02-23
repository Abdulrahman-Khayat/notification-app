using AutoMapper;
using NotificationService.Models;
using TemplateService.Dtos;

namespace UserService.Profiles;

public class TemplateProfile: Profile
{
    public TemplateProfile()
    {
        CreateMap<Template, ReadTemplateDto>();
        CreateMap<CreateTemplateDto, Template>();
    }
}