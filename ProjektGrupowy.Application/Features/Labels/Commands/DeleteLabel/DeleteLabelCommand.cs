using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Labels.Commands.DeleteLabel;

public record DeleteLabelCommand(
    int Id,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result>(UserId, IsAdmin);
