using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
