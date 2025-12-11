using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Commands.UpdateVideoGroup;

public record UpdateVideoGroupCommand(
    int VideoGroupId,
    string Name,
    string Description,
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<VideoGroup>>(UserId, IsAdmin);
