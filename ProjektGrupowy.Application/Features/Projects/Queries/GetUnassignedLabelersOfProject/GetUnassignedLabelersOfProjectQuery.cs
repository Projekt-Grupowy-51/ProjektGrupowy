using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;

public record GetUnassignedLabelersOfProjectQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<User>>>(UserId, IsAdmin);
