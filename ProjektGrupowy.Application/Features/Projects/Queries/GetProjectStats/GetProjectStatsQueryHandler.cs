using MediatR;
using ProjektGrupowy.Application.DTOs.Project;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetProjectStats;

public class GetProjectStatsQueryHandler : IRequestHandler<GetProjectStatsQuery, Result<ProjectStatsResponse>>
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

    public async Task<Result<ProjectStatsResponse>> Handle(GetProjectStatsQuery request, CancellationToken cancellationToken)
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

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var stats = new ProjectStatsResponse
        {
            Subjects = project.Subjects.Count,
            Videos = project.VideoGroups.Count,
            Assignments = project.Subjects.SelectMany(s => s.SubjectVideoGroupAssignments).Count(),
            Labelers = project.ProjectLabelers.Count,
            AccessCodes = project.AccessCodes.Count
        };

        return Result.Ok(stats);
    }
}
