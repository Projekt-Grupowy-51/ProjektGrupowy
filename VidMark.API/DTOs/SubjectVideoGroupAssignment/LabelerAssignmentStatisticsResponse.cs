using System.Text.Json.Serialization;

namespace VidMark.API.DTOs.SubjectVideoGroupAssignment;

/// <summary>
/// Statistics for a specific labeler in a specific assignment
/// </summary>
public class LabelerAssignmentStatisticsResponse
{
    [JsonPropertyName("labeler_id")]
    public string LabelerId { get; set; } = string.Empty;

    [JsonPropertyName("labeler_name")]
    public string LabelerName { get; set; } = string.Empty;

    [JsonPropertyName("labeler_email")]
    public string LabelerEmail { get; set; } = string.Empty;

    [JsonPropertyName("assignment_id")]
    public int AssignmentId { get; set; }

    [JsonPropertyName("subject_name")]
    public string SubjectName { get; set; } = string.Empty;

    [JsonPropertyName("video_group_name")]
    public string VideoGroupName { get; set; } = string.Empty;

    [JsonPropertyName("total_videos")]
    public int TotalVideos { get; set; }

    [JsonPropertyName("labeled_videos")]
    public int LabeledVideos { get; set; }

    [JsonPropertyName("total_labels")]
    public int TotalLabels { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("videos")]
    public List<VideoLabelingProgressResponse> Videos { get; set; } = new();
}

public class VideoLabelingProgressResponse
{
    [JsonPropertyName("video_id")]
    public int VideoId { get; set; }

    [JsonPropertyName("video_title")]
    public string VideoTitle { get; set; } = string.Empty;

    [JsonPropertyName("label_count")]
    public int LabelCount { get; set; }

    [JsonPropertyName("has_labeled")]
    public bool HasLabeled { get; set; }
}
