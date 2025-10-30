using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.AssignedLabels.Commands.DeleteAssignedLabel;

public record DeleteAssignedLabelCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
