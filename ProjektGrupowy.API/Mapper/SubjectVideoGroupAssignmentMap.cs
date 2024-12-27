using AutoMapper;

namespace ProjektGrupowy.API.Mapper;

public class SubjectVideoGroupAssignmentMap : Profile
{
    public SubjectVideoGroupAssignmentMap()
    {
        CreateMap<Models.SubjectVideoGroupAssignment, DTOs.SubjectVideoGroupAssignment.SubjectVideoGroupAssignmentResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.Id))
            .ForMember(dest => dest.VideoGroupId, opt => opt.MapFrom(src => src.VideoGroup.Id));
    }
}