using AutoMapper;
using ProjektGrupowy.API.DTOs.ProjectReport;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Mapper;

public class ReportMap : Profile
{
    public ReportMap()
    {
        CreateMap<GeneratedReport, GeneratedReportResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => src.CreatedAtUtc));
    }
}