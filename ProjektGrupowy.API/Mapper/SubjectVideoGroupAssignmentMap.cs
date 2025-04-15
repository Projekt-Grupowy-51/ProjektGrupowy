using AutoMapper;

namespace ProjektGrupowy.API.Mapper;

public class SubjectVideoGroupAssignmentMap : Profile
{
    public SubjectVideoGroupAssignmentMap()
    {
        CreateMap<Models.SubjectVideoGroupAssignment, DTOs.SubjectVideoGroupAssignment.SubjectVideoGroupAssignmentResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.Id))
            .ForMember(dest => dest.VideoGroupId, opt => opt.MapFrom(src => src.VideoGroup.Id))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.ModificationDate, opt => opt.MapFrom(src => src.ModificationDate))
            .ForMember(dest => dest.Labelers, opt => opt.MapFrom(src => src.Labelers))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.VideoGroup.Project.Id))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.VideoGroup.Project.Name))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
            .ForMember(dest => dest.VideoGroupName, opt => opt.MapFrom(src => src.VideoGroup.Name));
    }
}