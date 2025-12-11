using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetLabelersByProject;

public record GetLabelersByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<User>>>(UserId, IsAdmin);
