using FluentResults;
using VidMark.Application.CQRS;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

public record GetAssignedLabelsByVideoIdAndSubjectIdPageQuery(int Page, int PageSize, int[] VideoIds, int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<AssignedLabel>>>(UserId, IsAdmin);