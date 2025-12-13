using AutoMapper;
using VidMark.API.DTOs.VideoGroup;
using VidMark.Domain.Models;

namespace VidMark.API.Mapper;

public class VideoGroupMap : Profile
{
    public VideoGroupMap()
    {
        CreateMap<VideoGroup, VideoGroupResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.VideosAtPositions, opt => opt.MapFrom(src => src.Videos!.GroupBy(v => v.PositionInQueue).ToDictionary(g => g.Key, g => g.Count())));
    }
}