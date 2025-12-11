using ProjektGrupowy.Application.DTOs.Project;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetProjectStats;

public record GetProjectStatsQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<ProjectStatsResponse>>(UserId, IsAdmin);
