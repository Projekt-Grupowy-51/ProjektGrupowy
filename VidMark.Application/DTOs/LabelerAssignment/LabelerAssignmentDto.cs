using System.ComponentModel.DataAnnotations;

namespace VidMark.Application.DTOs.LabelerAssignment;

/// <summary>
/// DTO for labeler assignment
/// </summary>
public class LabelerAssignmentDto
{
    /// <summary>
    /// The access code string (exactly 16 characters).
    /// </summary>
    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string AccessCode { get; set; } = string.Empty;
}