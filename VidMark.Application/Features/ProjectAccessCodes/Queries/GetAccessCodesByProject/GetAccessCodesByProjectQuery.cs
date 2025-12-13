using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectAccessCodes.Queries.GetAccessCodesByProject;

public record GetAccessCodesByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<ProjectAccessCode>>>(UserId, IsAdmin);
