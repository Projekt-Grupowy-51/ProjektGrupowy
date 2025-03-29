namespace ProjektGrupowy.API.DTOs.AssignedLabel;

public class AssignedLabelRequest
{
    public int LabelId { get; set; } // ID of the label to be assigned
    public int SubjectVideoGroupAssignmentId { get; set; } // ID of the video group assignment
    public int LabelerId { get; set; } // ID of the labeler (assignee)
    public TimeSpan Start { get; set; } // Start time for the assigned label
    public TimeSpan End { get; set; } // End time for the assigned label
}