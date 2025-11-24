using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;

public record GetUnassignedLabelersOfProjectQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<User>>>(UserId, IsAdmin);
