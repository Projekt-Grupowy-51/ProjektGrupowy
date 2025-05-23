﻿using AutoMapper;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Mapper;

public class VideoMap : Profile
{
    public VideoMap()
    {
        CreateMap<Video, VideoResponse>()
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.VideoGroupId, opt => opt.MapFrom(src => src.VideoGroup.Id))
            .ForMember(dest => dest.PositionInQueue, opt => opt.MapFrom(src => src.PositionInQueue));
    }
}