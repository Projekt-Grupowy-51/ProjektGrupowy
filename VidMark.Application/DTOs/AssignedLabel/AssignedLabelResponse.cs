namespace VidMark.Application.DTOs.AssignedLabel;

/// <summary>
/// DTO for assigned label response - simple representation of an assigned label
/// </summary>
public class AssignedLabelResponse
{
    /// <summary>
    /// The unique identifier of the assigned label.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The unique identifier of the label associated with the assigned label.
    /// </summary>
    public int LabelId { get; set; }
    
    /// <summary>
    /// The name of the label associated with the assigned label.
    /// </summary>
    public string LabelName { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the video associated with the assigned label.
    /// </summary>
    public int VideoId { get; set; }
    
    /// <summary>
    /// The name of the video associated with the assigned label.
    /// </summary>
    public string VideoName { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the labeler who assigned the label.
    /// </summary>
    public string LabelerId { get; set; } = string.Empty;


    /// <summary>
    /// The name of the labeler who assigned the label.
    /// </summary>
    public string LabelerName { get; set; } = string.Empty;


    /// <summary>
    /// The start time of the label in the video.
    /// </summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>
    /// The end time of the label in the video.
    /// </summary>
    public string End { get; set; } = string.Empty;


    /// <summary>
    /// The date and time when the label was assigned.
    /// </summary>
    public DateTime InsDate { get; set; }

    /// <summary>
    /// The name of the subject associated with the video.
    /// </summary>
    public string SubjectName { get; set; } = string.Empty;
}