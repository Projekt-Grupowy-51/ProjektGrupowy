using AutoMapper;
using ProjektGrupowy.Application.DTOs.Labeler;

namespace ProjektGrupowy.Application.Mapper;

public class LabelerMap : Profile
{
    public LabelerMap()
    {
        CreateMap<ProjektGrupowy.Domain.Models.User, LabelerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName));
    }
}