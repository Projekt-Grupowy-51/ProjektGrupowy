using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Labels.Commands.AddLabel;

public record AddLabelCommand(
    string Name,
    string? Description,
    string UserId,
    int SubjectId,
    string ColorHex,
    char Shortcut,
    string Type,
    bool IsAdmin)
    : BaseCommand<Result<Label>>(UserId, IsAdmin);
