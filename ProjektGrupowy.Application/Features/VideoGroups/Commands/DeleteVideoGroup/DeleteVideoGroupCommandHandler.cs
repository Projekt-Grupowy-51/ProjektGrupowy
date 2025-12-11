using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.VideoGroups.Commands.DeleteVideoGroup;

public class DeleteVideoGroupCommandHandler : IRequestHandler<DeleteVideoGroupCommand, Result>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public DeleteVideoGroupCommandHandler(
        IVideoGroupRepository videoGroupRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoGroupRepository = videoGroupRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteVideoGroupCommand request, CancellationToken cancellationToken)
    {
        var videoGroup = await _videoGroupRepository.GetVideoGroupAsync(request.Id, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Delete));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // TODO: add domain event for video group deletion
        _videoGroupRepository.DeleteVideoGroup(videoGroup);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
