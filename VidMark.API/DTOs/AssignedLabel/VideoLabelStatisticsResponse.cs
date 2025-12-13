namespace VidMark.API.DTOs.AssignedLabel;

/// <summary>
/// DTO for video label statistics response - contains aggregated statistics about labels assigned to a video
/// </summary>
public class VideoLabelStatisticsResponse
{
    /// <summary>
    /// Total number of labels assigned to the video.
    /// </summary>
    public int TotalLabels { get; set; }

    /// <summary>
    /// Count of labels grouped by label type/name.
    /// </summary>
    public Dictionary<string, int> LabelsByType { get; set; } = new();

    /// <summary>
    /// Count of labels grouped by subject.
    /// </summary>
    public Dictionary<string, int> LabelsBySubject { get; set; } = new();

    /// <summary>
    /// Count of labels grouped by labeler.
    /// </summary>
    public Dictionary<string, int> LabelsByLabeler { get; set; } = new();
}
