using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.AssignedLabel;

/// <summary>
/// DTO for assigned label request
/// </summary>
public class AssignedLabelRequest
{
    /// <summary>
    /// The unique identifier of the label to be assigned.
    /// </summary>
    [Required]
    public int LabelId { get; set; }

    /// <summary>
    /// The unique identifier of the video to which the label is assigned.
    /// </summary>
    [Required]
    public int VideoId { get; set; }

    /// <summary>
    /// The start time of the label in the video.
    /// </summary>
    [Required]
    public string Start { get; set; } = string.Empty;

    /// <summary>
    /// The end time of the label in the video.
    /// </summary>
    [Required]
    public string End { get; set; } = string.Empty;
}