namespace ProjektGrupowy.API.DTOs.AssignedLabel;

public class AssignedLabelResponse
{
    public int Id { get; set; }
    public int LabelId { get; set; }
    public int SubjectVideoGroupAssignmentId { get; set; }
    public int LabelerId { get; set; }
    public TimeSpan Start { get; set; } // Start time for the label
    public TimeSpan End { get; set; } // End time for the label
}