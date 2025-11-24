using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Commands.UnassignLabelersFromProject;

public record UnassignLabelersFromProjectCommand(
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<bool>>(UserId, IsAdmin);
