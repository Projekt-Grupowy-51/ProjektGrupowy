namespace ProjektGrupowy.API.DTOs.AssignedLabel;

public class AssignedLabelResponse
{
    public int Id { get; set; }
    public int LabelId { get; set; }
    public int SubjectVideoGroupAssignmentId { get; set; }
    public int LabelerId { get; set; }
}