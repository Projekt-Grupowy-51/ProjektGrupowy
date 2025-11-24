using AutoMapper;
using VidMark.API.DTOs.AccessCode;
using VidMark.Domain.Models;

namespace VidMark.API.Mapper;

public class AccessCodeMap : Profile
{
    public AccessCodeMap()
    {
        CreateMap<ProjectAccessCode, AccessCodeResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => src.CreatedAtUtc))
            .ForMember(dest => dest.ExpiresAtUtc, opt => opt.MapFrom(src => src.ExpiresAtUtc))
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid));
    }
}