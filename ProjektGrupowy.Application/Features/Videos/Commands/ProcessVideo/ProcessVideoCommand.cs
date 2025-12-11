using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Videos.Commands.ProcessVideo;

public record ProcessVideoCommand(int Id, string UserId, bool IsAdmin) : BaseCommand<Result>(UserId, IsAdmin);