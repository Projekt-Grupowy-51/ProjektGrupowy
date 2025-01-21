using AutoMapper;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Mapper;

public class VideoGroupMap : Profile
{
    public VideoGroupMap()
    {
        CreateMap<VideoGroup, VideoGroupResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.Id));
    }
}