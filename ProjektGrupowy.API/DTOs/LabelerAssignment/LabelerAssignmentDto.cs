using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.LabelerAssignment;

public class LabelerAssignmentDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SubjectVideoGroupAssignmentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int LabelerId { get; set; }
}