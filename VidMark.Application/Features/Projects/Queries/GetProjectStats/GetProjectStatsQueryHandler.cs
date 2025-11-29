using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.Projects.Queries.GetProjectStats;

public class GetProjectStatsQueryHandler : IRequestHandler<GetProjectStatsQuery, Result<Project>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetProjectStatsQueryHandler(
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Project>> Handle(GetProjectStatsQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Read));

        return !authResult.Succeeded ? throw new ForbiddenException() : Result.Ok(project);
    }
}
