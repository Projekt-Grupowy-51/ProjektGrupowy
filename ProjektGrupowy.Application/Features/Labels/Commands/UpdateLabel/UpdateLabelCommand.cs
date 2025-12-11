using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Labels.Commands.UpdateLabel;

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
