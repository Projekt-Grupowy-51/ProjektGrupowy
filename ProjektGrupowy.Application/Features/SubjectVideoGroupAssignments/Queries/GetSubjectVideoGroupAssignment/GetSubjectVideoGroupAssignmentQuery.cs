using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignment;

public record GetSubjectVideoGroupAssignmentQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
