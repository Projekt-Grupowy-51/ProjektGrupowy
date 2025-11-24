using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.AddSubjectVideoGroupAssignment;

public record AddSubjectVideoGroupAssignmentCommand(
    int SubjectId,
    int VideoGroupId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
