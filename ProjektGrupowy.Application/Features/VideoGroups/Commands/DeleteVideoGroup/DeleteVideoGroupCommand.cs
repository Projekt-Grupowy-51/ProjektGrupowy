using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.VideoGroups.Commands.DeleteVideoGroup;

public record DeleteVideoGroupCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
