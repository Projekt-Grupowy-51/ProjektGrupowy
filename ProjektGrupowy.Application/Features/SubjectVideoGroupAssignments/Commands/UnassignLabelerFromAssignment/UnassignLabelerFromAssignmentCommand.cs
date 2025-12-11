using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.UnassignLabelerFromAssignment;

public record UnassignLabelerFromAssignmentCommand(
    int AssignmentId,
    string LabelerId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
