using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabel;

public record GetAssignedLabelQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<AssignedLabel>>(UserId, IsAdmin);
