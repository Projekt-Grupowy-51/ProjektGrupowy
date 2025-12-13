namespace VidMark.API.DTOs.VideoGroup;

/// <summary>
/// Response DTO for video group statistics
/// </summary>
public class VideoGroupStatisticsResponse
{
    public int TotalVideos { get; set; }
    public int TotalLabels { get; set; }
    public int CompletedVideos { get; set; }
    public double CompletionPercentage { get; set; }
    public Dictionary<string, int> LabelsByType { get; set; } = new();
    public Dictionary<string, int> LabelsBySubject { get; set; } = new();
    public Dictionary<string, int> VideoProgress { get; set; } = new();
}
