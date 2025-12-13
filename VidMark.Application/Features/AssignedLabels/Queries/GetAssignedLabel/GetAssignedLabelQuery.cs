using FluentResults;
using VidMark.Application.CQRS;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabel;

public record GetAssignedLabelQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<AssignedLabel>>(UserId, IsAdmin);
