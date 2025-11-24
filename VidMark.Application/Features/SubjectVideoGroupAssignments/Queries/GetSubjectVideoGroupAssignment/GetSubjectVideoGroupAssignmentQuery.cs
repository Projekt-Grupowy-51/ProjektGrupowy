using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignment;

public record GetSubjectVideoGroupAssignmentQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
