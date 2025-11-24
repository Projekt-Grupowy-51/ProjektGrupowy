using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Commands.UpdateVideoGroup;

public record UpdateVideoGroupCommand(
    int VideoGroupId,
    string Name,
    string Description,
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<VideoGroup>>(UserId, IsAdmin);
