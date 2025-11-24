using FluentResults;
using VidMark.Application.CQRS;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

public record GetAssignedLabelsByVideoIdAndSubjectIdCountQuery(int[] VideoIds, int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<int>>(UserId, IsAdmin);