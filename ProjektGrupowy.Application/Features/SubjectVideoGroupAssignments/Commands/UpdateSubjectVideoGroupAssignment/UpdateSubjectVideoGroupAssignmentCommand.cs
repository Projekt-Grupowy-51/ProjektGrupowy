using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.UpdateSubjectVideoGroupAssignment;

public record UpdateSubjectVideoGroupAssignmentCommand(
    int AssignmentId,
    int SubjectId,
    int VideoGroupId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
