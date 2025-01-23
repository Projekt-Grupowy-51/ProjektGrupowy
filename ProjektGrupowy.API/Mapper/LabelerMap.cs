using AutoMapper;
using ProjektGrupowy.API.DTOs.Labeler;

namespace ProjektGrupowy.API.Mapper;

public class LabelerMap : Profile
{
    public LabelerMap()
    {
        CreateMap<Models.Labeler, LabelerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}