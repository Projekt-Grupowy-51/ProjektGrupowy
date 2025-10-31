using FluentResults;
using ProjektGrupowy.Application.CQRS;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;

public record GetAssignedLabelsByVideoIdAndSubjectIdCountQuery(int VideoId, int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<int>>(UserId, IsAdmin);