using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.LabelerAssignment;

public class LabelerAssignmentDto
{
    [Required]
    [StringLength(16, MinimumLength = 16)]
    public string AccessCode { get; set; }
}