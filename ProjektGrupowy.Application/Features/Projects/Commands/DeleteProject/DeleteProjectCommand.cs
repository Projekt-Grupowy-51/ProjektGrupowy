using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
