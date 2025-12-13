using System.Text.Json.Serialization;

namespace VidMark.API.DTOs.Project;

/// <summary>
/// Detailed statistics for a project including label counts, trends, and breakdowns
/// </summary>
public class ProjectDetailedStatsResponse
{
    /// <summary>
    /// Basic counts
    /// </summary>
    [JsonPropertyName("total_subjects")]
    public int TotalSubjects { get; set; }

    [JsonPropertyName("total_video_groups")]
    public int TotalVideoGroups { get; set; }

    [JsonPropertyName("total_videos")]
    public int TotalVideos { get; set; }

    [JsonPropertyName("total_assignments")]
    public int TotalAssignments { get; set; }

    [JsonPropertyName("total_labelers")]
    public int TotalLabelers { get; set; }

    [JsonPropertyName("total_labels")]
    public int TotalLabels { get; set; }

    /// <summary>
    /// Labels grouped by subject name
    /// </summary>
    [JsonPropertyName("labels_by_subject")]
    public Dictionary<string, int> LabelsBySubject { get; set; } = new();

    /// <summary>
    /// Labels grouped by labeler username
    /// </summary>
    [JsonPropertyName("labels_by_labeler")]
    public Dictionary<string, int> LabelsByLabeler { get; set; } = new();

    /// <summary>
    /// Labels grouped by label name/type
    /// </summary>
    [JsonPropertyName("labels_by_type")]
    public Dictionary<string, int> LabelsByType { get; set; } = new();

    /// <summary>
    /// Number of completed assignments (all videos labeled)
    /// </summary>
    [JsonPropertyName("completed_assignments")]
    public int CompletedAssignments { get; set; }

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    [JsonPropertyName("progress_percentage")]
    public double ProgressPercentage { get; set; }
}
