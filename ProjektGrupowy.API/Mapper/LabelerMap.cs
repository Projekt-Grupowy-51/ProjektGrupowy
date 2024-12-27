using AutoMapper;

namespace ProjektGrupowy.API.Mapper;

public class LabelerMap : Profile
{
    public LabelerMap()
    {
        CreateMap<Models.Labeler, DTOs.Label.LabelResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}