using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.VideoGroups.Commands.DeleteVideoGroup;

public record DeleteVideoGroupCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
