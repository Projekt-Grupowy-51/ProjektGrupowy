using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.ProjectAccessCodes.Commands.RetireAccessCode;

public class RetireAccessCodeCommandHandler : IRequestHandler<RetireAccessCodeCommand, Result<ProjectAccessCode>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectAccessCodeRepository _projectAccessCodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RetireAccessCodeCommandHandler(IAuthorizationService authorizationService, ICurrentUserService currentUserService, IProjectAccessCodeRepository projectAccessCodeRepository, IUnitOfWork unitOfWork)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _projectAccessCodeRepository = projectAccessCodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectAccessCode>> Handle(RetireAccessCodeCommand request, CancellationToken cancellationToken)
    {
        var accessCode = await _projectAccessCodeRepository.GetAccessCodeByCodeAsync(request.Code, request.UserId, request.IsAdmin);
        if (accessCode is null)
        {
            return Result.Fail("Access code does not exist!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, accessCode, new ResourceOperationRequirement(ResourceOperation.Modify));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You dont have permission to perform this action");
        }

        if (!accessCode.IsValid)
        {
            return Result.Fail("Access code is already retired");
        }

        accessCode.Retire(request.UserId);
        await _unitOfWork.SaveChangesAsync();

        return accessCode;
    }
}
