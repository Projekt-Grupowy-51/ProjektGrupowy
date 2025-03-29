using AutoMapper;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Mapper;

public class SubjectMap : Profile
{
    public SubjectMap()
    {
        CreateMap<Subject, SubjectResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.Id));
    }
}