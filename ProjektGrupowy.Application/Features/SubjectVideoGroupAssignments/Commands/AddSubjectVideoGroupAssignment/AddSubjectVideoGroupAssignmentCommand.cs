using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.AddSubjectVideoGroupAssignment;

public record AddSubjectVideoGroupAssignmentCommand(
    int SubjectId,
    int VideoGroupId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<SubjectVideoGroupAssignment>>(UserId, IsAdmin);
