using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.UpdateSubjectVideoGroupAssignment;

public record UpdateSubjectVideoGroupAssignmentCommand(
    int AssignmentId,
    int SubjectId,
    int VideoGroupId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
