using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Commands.AddLabelerToProject;

public record AddLabelerToProjectCommand(
    string AccessCode,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<bool>>(UserId, IsAdmin);
