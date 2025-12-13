using FluentResults;
using VidMark.Application.CQRS;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.Projects.Queries.GetProjectStats;

public record GetProjectStatsQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<Project>>(UserId, IsAdmin);
