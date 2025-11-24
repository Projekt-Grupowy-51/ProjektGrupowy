using Microsoft.AspNetCore.Http;
using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Videos.Commands.AddVideo;

public record AddVideoCommand(
    string Title,
    IFormFile File,
    int VideoGroupId,
    int PositionInQueue,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Video>>(UserId, IsAdmin);
