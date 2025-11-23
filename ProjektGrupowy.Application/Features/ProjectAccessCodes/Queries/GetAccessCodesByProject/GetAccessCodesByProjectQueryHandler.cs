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
    private readonly IProjectRepository _projectRepository;

    public GetAccessCodesByProjectQueryHandler(
        IAuthorizationService authorizationService,
        ICurrentUserService currentUserService,
        IProjectAccessCodeRepository projectAccessCodeRepository,
        IProjectRepository projectRepository)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _projectAccessCodeRepository = projectAccessCodeRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Result<List<ProjectAccessCode>>> Handle(GetAccessCodesByProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);
        if (project is null)
        {
            return Result.Fail("Project not found");
        }

        var accessCodes = await _projectAccessCodeRepository.GetAccessCodesByProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to perform this action");
        }

        return Result.Ok(accessCodes);
    }
}
