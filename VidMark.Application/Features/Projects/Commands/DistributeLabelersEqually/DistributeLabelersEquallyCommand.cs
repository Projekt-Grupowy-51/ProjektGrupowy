using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.Projects.Commands.DistributeLabelersEqually;

public record DistributeLabelersEquallyCommand(
    int ProjectId,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<bool>>(UserId, IsAdmin);
