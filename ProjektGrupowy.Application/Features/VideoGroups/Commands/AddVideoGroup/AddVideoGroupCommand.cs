using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Commands.AddVideoGroup;

public record AddVideoGroupCommand(
    string Name,
    string Description,
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<VideoGroup>>(UserId, IsAdmin);
