namespace VidMark.API.DTOs.SubjectVideoGroupAssignment;

/// <summary>
/// DTO for subject video group assignment request - used for creating or updating a subject video group assignment
/// </summary>
public class SubjectVideoGroupAssignmentRequest
{
    /// <summary>
    /// The identifier of the subject associated with the assignment.
    /// </summary>
    public int SubjectId { get; set; }
    
    /// <summary>
    /// The identifier of the video group associated with the assignment.
    /// </summary>
    public int VideoGroupId { get; set; }
}