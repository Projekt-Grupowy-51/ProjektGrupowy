using VidMark.API.DTOs.Labeler;

namespace VidMark.API.DTOs.SubjectVideoGroupAssignment;

/// <summary>
/// DTO for subject-video group assignment response - simple representation of a subject-video group assignment
/// </summary>
public class SubjectVideoGroupAssignmentResponse
{
    /// <summary>
    /// The unique identifier of the subject-video group assignment.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The identifier of the subject associated with the assignment.
    /// </summary>
    public int SubjectId { get; set; }
    
    /// <summary>
    /// The identifier of the video group associated with the assignment.
    /// </summary>
    public int VideoGroupId { get; set; }
    
    /// <summary>
    /// The identifier of the project associated with the assignment.
    /// </summary>
    public int ProjectId { get; set; }
    
    /// <summary>
    /// The name of the project associated with the assignment.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the subject associated with the assignment.
    /// </summary>
    public string SubjectName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the video group associated with the assignment.
    /// </summary>
    public string VideoGroupName { get; set; } = string.Empty;

    /// <summary>
    /// A collection of labelers associated with the assignment.
    /// </summary>
    public IEnumerable<LabelerResponse> Labelers { get; set; } = new List<LabelerResponse>();
    
    /// <summary>
    /// The date when the assignment was created.
    /// </summary>
    public DateOnly CreationDate { get; set; }
    
    /// <summary>
    /// The date when the assignment was last modified. Null if never modified.
    /// </summary>
    public DateOnly? ModificationDate { get; set; }
}