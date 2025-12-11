using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.ProjectAccessCodes.Queries.GetAccessCodesByProject;

public class GetAccessCodesByProjectQueryHandler : IRequestHandler<GetAccessCodesByProjectQuery, Result<List<ProjectAccessCode>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectAccessCodeRepository _projectAccessCodeRepository;
    public GetAccessCodesByProjectQueryHandler(IAuthorizationService authorizationService, ICurrentUserService currentUserService, IProjectAccessCodeRepository projectAccessCodeRepository)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _projectAccessCodeRepository = projectAccessCodeRepository;
    }

    public async Task<Result<List<ProjectAccessCode>>> Handle(GetAccessCodesByProjectQuery request, CancellationToken cancellationToken)
    {
        var accessCodes = await _projectAccessCodeRepository.GetAccessCodesByProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, accessCodes, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to perform this action");
        }

        return Result.Ok(accessCodes);
    }
}
