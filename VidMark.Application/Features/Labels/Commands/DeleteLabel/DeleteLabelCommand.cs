using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Labels.Commands.DeleteLabel;

public record DeleteLabelCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
