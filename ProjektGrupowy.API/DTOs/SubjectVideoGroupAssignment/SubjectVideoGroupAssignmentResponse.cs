using ProjektGrupowy.API.DTOs.Labeler;

namespace ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;

public class SubjectVideoGroupAssignmentResponse
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int VideoGroupId { get; set; }
    public IEnumerable<LabelerResponse> Labelers { get; set; }
    public DateOnly CreationDate { get; set; }
    public DateOnly? ModificationDate { get; set; }
}