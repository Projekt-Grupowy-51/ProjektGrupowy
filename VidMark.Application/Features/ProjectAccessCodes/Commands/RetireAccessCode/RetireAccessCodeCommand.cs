using VidMark.Domain.Models;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectAccessCodes.Commands.RetireAccessCode;

public record RetireAccessCodeCommand(
    string Code,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<ProjectAccessCode>>(UserId, IsAdmin);
