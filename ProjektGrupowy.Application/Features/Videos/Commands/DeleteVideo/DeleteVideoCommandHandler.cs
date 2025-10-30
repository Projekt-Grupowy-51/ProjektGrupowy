using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Videos.Commands.DeleteVideo;

public class DeleteVideoCommandHandler : IRequestHandler<DeleteVideoCommand, Result>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public DeleteVideoCommandHandler(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetVideoAsync(request.Id, request.UserId, request.IsAdmin);

        if (video is null)
        {
            return Result.Fail("No video found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            video,
            new ResourceOperationRequirement(ResourceOperation.Delete));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // TODO: add domain event for video deletion
        _videoRepository.DeleteVideo(video);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
