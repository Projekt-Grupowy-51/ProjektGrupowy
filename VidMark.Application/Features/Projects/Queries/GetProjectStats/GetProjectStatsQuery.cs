using VidMark.Application.DTOs.Project;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Queries.GetProjectStats;

public record GetProjectStatsQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<ProjectStatsResponse>>(UserId, IsAdmin);
