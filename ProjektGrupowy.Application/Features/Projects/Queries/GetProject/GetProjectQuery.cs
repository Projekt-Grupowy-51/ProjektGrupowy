using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetProject;

public record GetProjectQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Project>>(UserId, IsAdmin);
