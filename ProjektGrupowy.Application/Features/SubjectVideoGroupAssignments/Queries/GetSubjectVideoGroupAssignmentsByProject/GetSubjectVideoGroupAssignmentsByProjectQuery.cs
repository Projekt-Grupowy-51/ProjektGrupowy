using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentsByProject;

public record GetSubjectVideoGroupAssignmentsByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<SubjectVideoGroupAssignment>>>(UserId, IsAdmin);
