using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectAccessCodes.Queries.GetAccessCodesByProject;

public record GetAccessCodesByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<ProjectAccessCode>>>(UserId, IsAdmin);
