namespace ProjektGrupowy.API.DTOs.AssignedLabel;

public class AssignedLabelRequest
{
    public int LabelId { get; set; }
    public int SubjectVideoGroupAssignmentId { get; set; }

    public int LabelerId { get; set; }
}