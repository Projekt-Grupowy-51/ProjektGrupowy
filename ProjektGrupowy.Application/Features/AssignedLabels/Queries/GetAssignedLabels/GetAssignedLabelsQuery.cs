using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabels;

public record GetAssignedLabelsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<AssignedLabel>>>(UserId, IsAdmin);
