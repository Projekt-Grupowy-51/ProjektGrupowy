using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;

public record GetAssignedLabelsCountQuery(int VideoId, string UserId, bool IsAdmin)
    : BaseQuery<Result<int>>(UserId, IsAdmin);