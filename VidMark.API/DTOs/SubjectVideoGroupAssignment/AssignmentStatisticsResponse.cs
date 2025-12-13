using System.Text.Json.Serialization;

namespace VidMark.API.DTOs.SubjectVideoGroupAssignment;

/// <summary>
/// Statistics for a Subject-VideoGroup Assignment
/// </summary>
public class AssignmentStatisticsResponse
{
    /// <summary>
    /// Total number of videos in the video group
    /// </summary>
    [JsonPropertyName("total_videos")]
    public int TotalVideos { get; set; }

    /// <summary>
    /// Total number of labels assigned in this assignment
    /// </summary>
    [JsonPropertyName("total_labels")]
    public int TotalLabels { get; set; }

    /// <summary>
    /// Number of labelers assigned to this assignment
    /// </summary>
    [JsonPropertyName("assigned_labelers_count")]
    public int AssignedLabelersCount { get; set; }

    /// <summary>
    /// Overall completion percentage
    /// </summary>
    [JsonPropertyName("completion_percentage")]
    public double CompletionPercentage { get; set; }

    /// <summary>
    /// Progress for each video
    /// </summary>
    [JsonPropertyName("videos")]
    public List<VideoProgressInfo> Videos { get; set; } = new();

    /// <summary>
    /// Top 10 most active labelers
    /// </summary>
    [JsonPropertyName("top_labelers")]
    public Dictionary<string, int> TopLabelers { get; set; } = new();

    /// <summary>
    /// All labelers with their statistics
    /// </summary>
    [JsonPropertyName("all_labelers")]
    public List<LabelerStatInfo> AllLabelers { get; set; } = new();

    /// <summary>
    /// Video status breakdown
    /// </summary>
    [JsonPropertyName("video_status")]
    public VideoStatusBreakdown VideoStatus { get; set; } = new();
}

/// <summary>
/// Progress information for a single video
/// </summary>
public class VideoProgressInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("labels_received")]
    public int LabelsReceived { get; set; }

    [JsonPropertyName("expected_labels")]
    public int ExpectedLabels { get; set; }

    [JsonPropertyName("completion_percentage")]
    public double CompletionPercentage { get; set; }
}

/// <summary>
/// Statistics for a single labeler
/// </summary>
public class LabelerStatInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("label_count")]
    public int LabelCount { get; set; }

    [JsonPropertyName("completion_percentage")]
    public double CompletionPercentage { get; set; }
}

/// <summary>
/// Breakdown of video completion status
/// </summary>
public class VideoStatusBreakdown
{
    [JsonPropertyName("completed")]
    public int Completed { get; set; }

    [JsonPropertyName("in_progress")]
    public int InProgress { get; set; }

    [JsonPropertyName("not_started")]
    public int NotStarted { get; set; }
}
