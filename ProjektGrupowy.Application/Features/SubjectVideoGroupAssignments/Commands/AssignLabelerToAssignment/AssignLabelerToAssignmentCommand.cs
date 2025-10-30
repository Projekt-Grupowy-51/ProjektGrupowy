using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.AssignLabelerToAssignment;

public record AssignLabelerToAssignmentCommand(
    int AssignmentId,
    string LabelerId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
