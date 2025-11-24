namespace VidMark.Application.DTOs.Subject;

/// <summary>
/// DTO for subject response - simple representation of a subject
/// </summary>
public class SubjectResponse
{
    /// <summary>
    /// The unique identifier of the subject.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the subject.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the subject.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The unique identifier of the project associated with the subject.
    /// </summary>
    public int ProjectId { get; set; }
}