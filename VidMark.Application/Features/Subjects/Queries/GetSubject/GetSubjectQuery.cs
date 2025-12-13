using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Subjects.Queries.GetSubject;

public record GetSubjectQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Subject>>(UserId, IsAdmin);
