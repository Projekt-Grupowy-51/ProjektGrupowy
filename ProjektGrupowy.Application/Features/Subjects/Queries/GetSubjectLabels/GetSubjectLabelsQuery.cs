using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjectLabels;

public record GetSubjectLabelsQuery(int SubjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<Label>>>(UserId, IsAdmin);
