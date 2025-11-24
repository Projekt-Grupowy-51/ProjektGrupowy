using AutoMapper;
using VidMark.API.DTOs.ProjectReport;
using VidMark.Domain.Models;

namespace VidMark.API.Mapper;

public class ReportMap : Profile
{
    public ReportMap()
    {
        CreateMap<GeneratedReport, GeneratedReportResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => src.CreatedAtUtc));
    }
}