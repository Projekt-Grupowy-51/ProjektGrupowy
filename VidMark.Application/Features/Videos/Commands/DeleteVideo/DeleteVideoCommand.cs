using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Videos.Commands.DeleteVideo;

public record DeleteVideoCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
