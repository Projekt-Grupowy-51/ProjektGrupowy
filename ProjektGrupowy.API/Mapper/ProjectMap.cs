using AutoMapper;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Mapper;

public class ProjectMap : Profile
{
    public ProjectMap()
    {
        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<ProjectRequest, Project>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}