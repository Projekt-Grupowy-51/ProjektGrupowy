namespace VidMark.API.DTOs.Subject;

/// <summary>
/// Response DTO for subject statistics
/// </summary>
public class SubjectStatisticsResponse
{
    public int TotalLabels { get; set; }
    public int TotalAssignedLabels { get; set; }
    public int UniqueVideosLabeled { get; set; }
    public int TotalAssignments { get; set; }
    public Dictionary<string, int> LabelUsageByType { get; set; } = new();
    public Dictionary<string, int> LabelsByLabeler { get; set; } = new();
    public Dictionary<string, int> LabelsByProject { get; set; } = new();
}
