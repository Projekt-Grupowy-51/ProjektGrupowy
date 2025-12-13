using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.ProjectAccessCodes.Queries.ValidateAccessCode;

public class ValidateAccessCodeQueryHandler : IRequestHandler<ValidateAccessCodeQuery, Result<bool>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectAccessCodeRepository _projectAccessCodeRepository;

    public ValidateAccessCodeQueryHandler(IAuthorizationService authorizationService, ICurrentUserService currentUserService, IProjectAccessCodeRepository projectAccessCodeRepository)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _projectAccessCodeRepository = projectAccessCodeRepository;
    }

    public async Task<Result<bool>> Handle(ValidateAccessCodeQuery request, CancellationToken cancellationToken)
    {
        var accessCode = await _projectAccessCodeRepository.GetAccessCodeByCodeAsync(request.Code, request.UserId, request.IsAdmin);
        if (accessCode is null)
        {
            return Result.Ok(false); // Invalid/non-existent code is simply not valid
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, accessCode, new ResourceOperationRequirement(ResourceOperation.Read));
        return !authResult.Succeeded
            ? throw new ForbiddenException("You do not have permission to access this access code.")
            : Result.Ok(accessCode.IsValid);
    }
}
