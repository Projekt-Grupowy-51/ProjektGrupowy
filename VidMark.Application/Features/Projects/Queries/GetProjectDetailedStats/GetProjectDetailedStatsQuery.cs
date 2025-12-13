using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Queries.GetProjectDetailedStats;

public record GetProjectDetailedStatsQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<ProjectDetailedStatistics>>(UserId, IsAdmin);

public class ProjectDetailedStatistics
{
    public int TotalSubjects { get; set; }
    public int TotalVideoGroups { get; set; }
    public int TotalVideos { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalLabelers { get; set; }
    public int TotalLabels { get; set; }
    public Dictionary<string, int> LabelsBySubject { get; set; } = new();
    public Dictionary<string, int> LabelsByLabeler { get; set; } = new();
    public Dictionary<string, int> LabelsByType { get; set; } = new();
    public int CompletedAssignments { get; set; }
    public double ProgressPercentage { get; set; }
}
