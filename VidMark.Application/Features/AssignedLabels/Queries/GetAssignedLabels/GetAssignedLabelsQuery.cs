using FluentResults;
using VidMark.Application.CQRS;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabels;

public record GetAssignedLabelsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<AssignedLabel>>>(UserId, IsAdmin);
