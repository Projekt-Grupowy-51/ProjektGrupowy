using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Queries.GetProject;

public record GetProjectQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Project>>(UserId, IsAdmin);
