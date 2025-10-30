using ProjektGrupowy.Domain.Models;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectAccessCodes.Commands.RetireAccessCode;

public record RetireAccessCodeCommand(
    string Code,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<ProjectAccessCode>>(UserId, IsAdmin);
