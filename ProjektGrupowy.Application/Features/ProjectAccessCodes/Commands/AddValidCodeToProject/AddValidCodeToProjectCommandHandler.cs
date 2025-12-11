using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.ProjectAccessCodes.Commands.AddValidCodeToProject;

public class AddValidCodeToProjectCommandHandler : IRequestHandler<AddValidCodeToProjectCommand, Result<ProjectAccessCode>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProjectAccessCodeRepository _projectAccessCodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddValidCodeToProjectCommandHandler(IProjectRepository projectRepository, IAuthorizationService authorizationService, ICurrentUserService currentUserService, IProjectAccessCodeRepository projectAccessCodeRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _projectAccessCodeRepository = projectAccessCodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectAccessCode>> Handle(AddValidCodeToProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);
        if (project is null)
        {
            return Result.Fail("Project does not exist!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You dont have permission to perform this action");
        }

        var accessCode = await _projectAccessCodeRepository.GetValidAccessCodeByProjectAsync(project.Id, request.UserId, request.IsAdmin);
        accessCode?.Retire(request.UserId);

        var newAccessCode = ProjectAccessCode.Create(project, request.Expiration, request.UserId);

        await _projectAccessCodeRepository.AddAccessCodeAsync(newAccessCode);
        await _unitOfWork.SaveChangesAsync();

        return newAccessCode;
    }
}
