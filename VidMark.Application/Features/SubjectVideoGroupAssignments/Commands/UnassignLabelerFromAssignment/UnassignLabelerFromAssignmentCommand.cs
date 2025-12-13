using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.UnassignLabelerFromAssignment;

public record UnassignLabelerFromAssignmentCommand(
    int AssignmentId,
    string LabelerId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
