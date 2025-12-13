using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.AssignedLabels.Commands.DeleteAssignedLabel;

public record DeleteAssignedLabelCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
