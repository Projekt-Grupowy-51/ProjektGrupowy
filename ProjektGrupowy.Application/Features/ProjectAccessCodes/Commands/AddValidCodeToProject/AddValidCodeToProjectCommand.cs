using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Enums;
using FluentResults;
using ProjektGrupowy.Application.CQRS;

namespace ProjektGrupowy.Application.Features.ProjectAccessCodes.Commands.AddValidCodeToProject;

public record AddValidCodeToProjectCommand(
    int ProjectId,
    AccessCodeExpiration Expiration,
    int CustomExpiration,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<ProjectAccessCode>>(UserId, IsAdmin);
