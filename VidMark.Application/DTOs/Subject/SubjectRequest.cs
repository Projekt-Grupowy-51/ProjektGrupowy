using System.ComponentModel.DataAnnotations;

namespace VidMark.Application.DTOs.Subject;

/// <summary>
/// DTO for subject request - used for creating or updating a subject
/// </summary>
public class SubjectRequest
{
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