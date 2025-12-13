using FluentResults;
using MediatR;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetLabelerAssignmentStatistics;

public record GetLabelerAssignmentStatisticsQuery(
    int AssignmentId,
    string LabelerId,
    string UserId,
    bool IsAdmin) : IRequest<Result<LabelerAssignmentStatistics>>;

public class LabelerAssignmentStatistics
{
    public string LabelerId { get; set; } = string.Empty;
    public string LabelerName { get; set; } = string.Empty;
    public string LabelerEmail { get; set; } = string.Empty;
    public int AssignmentId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string VideoGroupName { get; set; } = string.Empty;
    public int TotalVideos { get; set; }
    public int LabeledVideos { get; set; }
    public int TotalLabels { get; set; }
    public bool IsCompleted { get; set; }
    public List<VideoLabelingProgress> Videos { get; set; } = new();
}

public class VideoLabelingProgress
{
    public int VideoId { get; set; }
    public string VideoTitle { get; set; } = string.Empty;
    public int LabelCount { get; set; }
    public bool HasLabeled { get; set; }
}
