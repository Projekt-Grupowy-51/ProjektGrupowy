using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideoGroupStatistics;

public record GetVideoGroupStatisticsQuery(int VideoGroupId, string UserId, bool IsAdmin)
    : BaseQuery<Result<VideoGroupStatistics>>(UserId, IsAdmin);

public class VideoGroupStatistics
{
    public int TotalVideos { get; set; }
    public int TotalLabels { get; set; }
    public int CompletedVideos { get; set; }
    public double CompletionPercentage { get; set; }
    public Dictionary<string, int> LabelsByType { get; set; } = new();
    public Dictionary<string, int> LabelsBySubject { get; set; } = new();
    public Dictionary<string, int> VideoProgress { get; set; } = new();
}
