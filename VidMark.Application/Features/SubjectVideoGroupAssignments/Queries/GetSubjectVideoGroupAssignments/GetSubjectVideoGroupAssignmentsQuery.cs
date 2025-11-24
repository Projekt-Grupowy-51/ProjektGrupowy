using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignments;

public record GetSubjectVideoGroupAssignmentsQuery(string UserId, bool IsAdmin)
    : BaseQuery<Result<List<SubjectVideoGroupAssignment>>>(UserId, IsAdmin);
