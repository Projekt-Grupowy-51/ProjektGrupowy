using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Commands.AddProject;

public record AddProjectCommand(
    string Name,
    string Description,
    bool Finished,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Project>>(UserId, IsAdmin);
