using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Commands.UnassignLabelersFromProject;

public record UnassignLabelersFromProjectCommand(
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<bool>>(UserId, IsAdmin);
