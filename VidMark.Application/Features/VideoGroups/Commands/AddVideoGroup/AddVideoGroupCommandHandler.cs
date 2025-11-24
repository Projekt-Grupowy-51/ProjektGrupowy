using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.VideoGroups.Commands.AddVideoGroup;

public class AddVideoGroupCommandHandler : IRequestHandler<AddVideoGroupCommand, Result<VideoGroup>>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public AddVideoGroupCommandHandler(
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

    public async Task<Result<VideoGroup>> Handle(AddVideoGroupCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Create));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroup = VideoGroup.Create(request.Name, request.Description, project, request.UserId);

        await _videoGroupRepository.AddVideoGroupAsync(videoGroup);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(videoGroup);
    }
}
