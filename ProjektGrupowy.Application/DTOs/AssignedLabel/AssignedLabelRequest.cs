using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Application.DTOs.AssignedLabel;

public class AssignedLabelRequest
{
    [Required]
    public int LabelId { get; set; } // ID of the label to be assigned

    [Required]
    public int VideoId { get; set; } // ID of the video

    [Required]
    public string Start { get; set; } // Start time for the assigned label

    [Required]
    public string End { get; set; } // End time for the assigned label
}