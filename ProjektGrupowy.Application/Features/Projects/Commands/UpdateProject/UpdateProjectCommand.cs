using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    int ProjectId,
    string Name,
    string Description,
    bool Finished,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Project>>(UserId, IsAdmin);
