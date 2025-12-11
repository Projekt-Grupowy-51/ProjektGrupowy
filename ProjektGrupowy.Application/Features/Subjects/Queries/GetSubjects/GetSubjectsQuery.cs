using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjects;

public record GetSubjectsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Subject>>>(UserId, IsAdmin);
