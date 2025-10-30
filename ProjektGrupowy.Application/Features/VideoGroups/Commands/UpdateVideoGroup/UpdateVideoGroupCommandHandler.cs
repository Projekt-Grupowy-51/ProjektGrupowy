using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.VideoGroups.Commands.UpdateVideoGroup;

public class UpdateVideoGroupCommandHandler : IRequestHandler<UpdateVideoGroupCommand, Result<VideoGroup>>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public UpdateVideoGroupCommandHandler(
        IVideoGroupRepository videoGroupRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoGroupRepository = videoGroupRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<VideoGroup>> Handle(UpdateVideoGroupCommand request, CancellationToken cancellationToken)
    {
        var videoGroup = await _videoGroupRepository.GetVideoGroupAsync(request.VideoGroupId, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Update));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResultProject = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Update));

        if (!authResultProject.Succeeded)
        {
            throw new ForbiddenException();
        }

        videoGroup.Update(request.Name, request.Description, project, request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(videoGroup);
    }
}
