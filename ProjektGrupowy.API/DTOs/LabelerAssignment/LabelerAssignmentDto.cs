using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.LabelerAssignment;

public class LabelerAssignmentDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int LabelerId { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string AccessCode { get; set; }
}