using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentsByProject;

public record GetSubjectVideoGroupAssignmentsByProjectQuery(int ProjectId, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<SubjectVideoGroupAssignment>>>(UserId, IsAdmin);
