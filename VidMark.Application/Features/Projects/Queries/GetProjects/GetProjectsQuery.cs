using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Project>>>(UserId, IsAdmin);
