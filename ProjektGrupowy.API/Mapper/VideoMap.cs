using AutoMapper;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Mapper;

public class VideoMap : Profile
{
    public VideoMap()
    {
        CreateMap<Video, VideoResponse>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path));

        CreateMap<VideoRequest, Video>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.File.FileName));

    }
}