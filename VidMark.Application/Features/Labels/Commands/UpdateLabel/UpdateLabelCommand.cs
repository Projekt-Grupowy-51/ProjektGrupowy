using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Labels.Commands.UpdateLabel;

public record UpdateLabelCommand(
    int LabelId,
    string Name,
    string? Description,
    string UserId,
    int SubjectId,
    char Shortcut,
    string ColorHex,
    string Type,
    bool IsAdmin)
    : BaseCommand<Result<Label>>(UserId, IsAdmin);
