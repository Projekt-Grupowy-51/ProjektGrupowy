using AutoMapper;
using VidMark.API.DTOs.Project;
using VidMark.Domain.Models;

namespace VidMark.API.Mapper;

public class ProjectMap : Profile
{
    public ProjectMap()
    {
        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //.ForMember(dest => dest.ScientistId, opt => opt.MapFrom(src => src.CreatedBy.Id))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.ModificationDate, opt => opt.MapFrom(src => src.ModificationDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Finished, opt => opt.MapFrom(src => src.EndDate.HasValue));

        CreateMap<Project, ProjectStatsResponse>()
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.Subjects.Count))
            .ForMember(dest => dest.Videos, opt => opt.MapFrom(src => src.VideoGroups.Count))
            .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Subjects.SelectMany(s => s.SubjectVideoGroupAssignments).DistinctBy(x => x.Id).Count()))
            .ForMember(dest => dest.Labelers, opt => opt.MapFrom(src => src.ProjectLabelers.Count))
            .ForMember(dest => dest.AccessCodes, opt => opt.MapFrom(src => src.AccessCodes.Count));
    }
}