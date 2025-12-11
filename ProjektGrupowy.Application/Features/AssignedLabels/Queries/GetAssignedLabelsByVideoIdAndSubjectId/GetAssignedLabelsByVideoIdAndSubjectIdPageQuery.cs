using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

public record GetAssignedLabelsByVideoIdAndSubjectIdPageQuery(int Page, int PageSize, int[] VideoIds, int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<AssignedLabel>>>(UserId, IsAdmin);