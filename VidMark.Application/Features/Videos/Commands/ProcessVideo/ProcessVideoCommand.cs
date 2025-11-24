using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Videos.Commands.ProcessVideo;

public record ProcessVideoCommand(int Id, string UserId, bool IsAdmin) : BaseCommand<Result>(UserId, IsAdmin);