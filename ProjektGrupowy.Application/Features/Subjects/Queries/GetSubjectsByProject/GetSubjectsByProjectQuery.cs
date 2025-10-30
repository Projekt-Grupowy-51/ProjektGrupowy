using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjectsByProject;

public record GetSubjectsByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Subject>>>(UserId, IsAdmin);
