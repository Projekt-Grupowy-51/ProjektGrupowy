using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

public record GetAssignedLabelsCountQuery(int VideoId, string UserId, bool IsAdmin)
    : BaseQuery<Result<int>>(UserId, IsAdmin);