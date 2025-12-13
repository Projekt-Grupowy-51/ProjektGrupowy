using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetVideoLabelStatistics;

public record GetVideoLabelStatisticsQuery(int VideoId, string UserId, bool IsAdmin)
    : BaseQuery<Result<VideoLabelStatistics>>(UserId, IsAdmin);

public class VideoLabelStatistics
{
    public int TotalLabels { get; set; }
    public Dictionary<string, int> LabelsByType { get; set; } = new();
    public Dictionary<string, int> LabelsBySubject { get; set; } = new();
    public Dictionary<string, int> LabelsByLabeler { get; set; } = new();
}
