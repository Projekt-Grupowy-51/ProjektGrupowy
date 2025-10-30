using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Labels.Commands.AddLabel;

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
