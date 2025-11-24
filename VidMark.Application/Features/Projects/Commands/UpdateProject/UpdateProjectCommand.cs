using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    int ProjectId,
    string Name,
    string Description,
    bool Finished,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Project>>(UserId, IsAdmin);
