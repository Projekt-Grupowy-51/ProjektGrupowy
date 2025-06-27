using AutoMapper;
using ProjektGrupowy.Application.DTOs.Project;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Mapper;

public class ProjectMap : Profile
{
    public ProjectMap()
    {
        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ScientistId, opt => opt.MapFrom(src => src.CreatedBy.Id))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.ModificationDate, opt => opt.MapFrom(src => src.ModificationDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Finished, opt => opt.MapFrom(src => src.EndDate.HasValue));
    }
}