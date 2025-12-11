using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Subjects.Queries.GetSubject;

public record GetSubjectQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<Subject>>(UserId, IsAdmin);
