using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.ToggleAssignmentCompletion;

public record ToggleAssignmentCompletionCommand(
    int AssignmentId,
    bool IsCompleted,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
