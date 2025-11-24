using VidMark.Domain.Models;
using VidMark.Domain.Enums;
using FluentResults;
using VidMark.Application.CQRS;

namespace VidMark.Application.Features.ProjectAccessCodes.Commands.AddValidCodeToProject;

public record AddValidCodeToProjectCommand(
    int ProjectId,
    AccessCodeExpiration Expiration,
    int CustomExpiration,
    string UserId,
    bool IsAdmin)
    : BaseCommand<Result<ProjectAccessCode>>(UserId, IsAdmin);
