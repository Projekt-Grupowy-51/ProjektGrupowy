using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.LabelerAssignment;

public class LabelerAssignmentDto
{
    public string LabelerId { get; set; } = string.Empty;

    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string AccessCode { get; set; }
}