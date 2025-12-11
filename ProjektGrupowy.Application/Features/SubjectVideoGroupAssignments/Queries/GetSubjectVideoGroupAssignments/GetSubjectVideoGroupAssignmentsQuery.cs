using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignments;

public record GetSubjectVideoGroupAssignmentsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<SubjectVideoGroupAssignment>>>(UserId, IsAdmin);
