using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Subjects.Queries.GetSubjectLabels;

public record GetSubjectLabelsQuery(int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Label>>>(UserId, IsAdmin);
