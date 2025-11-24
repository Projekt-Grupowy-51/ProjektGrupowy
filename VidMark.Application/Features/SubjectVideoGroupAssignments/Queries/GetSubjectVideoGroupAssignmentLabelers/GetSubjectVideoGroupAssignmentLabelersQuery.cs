using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentLabelers;

public record GetSubjectVideoGroupAssignmentLabelersQuery(int Id, string UserId, bool IsAdmin)
    : BaseQuery<Result<List<User>>>(UserId, IsAdmin);
