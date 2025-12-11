using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Projects.Commands.UnassignLabelersFromProject;

public class UnassignLabelersFromProjectCommandHandler : IRequestHandler<UnassignLabelersFromProjectCommand, Result<bool>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public UnassignLabelersFromProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<bool>> Handle(UnassignLabelersFromProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Update));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var assignments = project.Subjects
            .SelectMany(s => s.SubjectVideoGroupAssignments)
            .ToList();

        foreach (var assignment in assignments)
        {
            assignment.ClearLabelers(request.UserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(true);
    }
}
