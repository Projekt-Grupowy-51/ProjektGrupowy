using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.Projects.Commands.DistributeLabelersEqually;

public record DistributeLabelersEquallyCommand(
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<bool>>(UserId, IsAdmin);
