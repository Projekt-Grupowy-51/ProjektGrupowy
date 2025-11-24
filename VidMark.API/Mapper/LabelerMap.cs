using AutoMapper;
using VidMark.API.DTOs.Labeler;

namespace VidMark.API.Mapper;

public class LabelerMap : Profile
{
    public LabelerMap()
    {
        CreateMap<Domain.Models.User, LabelerResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName));
    }
}