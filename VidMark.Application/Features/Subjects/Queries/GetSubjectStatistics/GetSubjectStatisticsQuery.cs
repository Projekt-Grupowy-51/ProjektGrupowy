using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Subjects.Queries.GetSubjectStatistics;

public record GetSubjectStatisticsQuery(int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<SubjectStatistics>>(UserId, IsAdmin);

public class SubjectStatistics
{
    public int TotalLabels { get; set; }
    public int TotalAssignedLabels { get; set; }
    public int UniqueVideosLabeled { get; set; }
    public int TotalAssignments { get; set; }
    public Dictionary<string, int> LabelUsageByType { get; set; } = new();
    public Dictionary<string, int> LabelsByLabeler { get; set; } = new();
    public Dictionary<string, int> LabelsByProject { get; set; } = new();
}
