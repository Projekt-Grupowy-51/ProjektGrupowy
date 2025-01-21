using AutoMapper;

namespace ProjektGrupowy.API.Mapper;

public class AssignedLabelMap : Profile
{
    public AssignedLabelMap()
    {
        CreateMap<Models.AssignedLabel, DTOs.AssignedLabel.AssignedLabelResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LabelId, opt => opt.MapFrom(src => src.Label.Id))
            .ForMember(dest => dest.SubjectVideoGroupAssignmentId, opt => opt.MapFrom(src => src.SubjectVideoGroupAssignment.Id))
            .ForMember(dest => dest.LabelerId, opt => opt.MapFrom(src => src.Labeler.Id))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End));
    }
}