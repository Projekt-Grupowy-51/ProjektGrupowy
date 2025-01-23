using AutoMapper;

namespace ProjektGrupowy.API.Mapper;

public class LabelMap : Profile
{
    public LabelMap()
    {
        CreateMap<Models.Label, DTOs.Label.LabelResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.Id))
            .ForMember(dest => dest.ColorHex, opt => opt.MapFrom(src => src.ColorHex))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Shortcut, opt => opt.MapFrom(src => src.Shortcut));
    }
}