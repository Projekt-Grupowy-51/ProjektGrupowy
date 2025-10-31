using Microsoft.AspNetCore.Http;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Videos.Commands.UpdateVideo;

public record UpdateVideoCommand(
    int VideoId,
    string Title,
    IFormFile? File,
    int VideoGroupId,
    int PositionInQueue,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<Video>>(UserId, IsAdmin);
