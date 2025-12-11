using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;

public class GetUnassignedLabelersOfProjectQueryHandler : IRequestHandler<GetUnassignedLabelersOfProjectQuery, Result<List<User>>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetUnassignedLabelersOfProjectQueryHandler(
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<User>>> Handle(GetUnassignedLabelersOfProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.Id, request.UserId, request.IsAdmin);

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

        var unassignedLabelers = project.ProjectLabelers
            .Where(labeler => !project.Subjects
                .SelectMany(subject => subject.SubjectVideoGroupAssignments)
                .Any(assignment => assignment.Labelers!.Contains(labeler)))
            .ToList();

        return Result.Ok(unassignedLabelers);
    }
}
