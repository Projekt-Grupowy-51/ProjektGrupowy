using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Project>>>(UserId, IsAdmin);
