using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Videos.Queries.GetVideosByGroupAndPosition;

public class GetVideosByGroupAndPositionQueryHandler : IRequestHandler<GetVideosByGroupAndPositionQuery, Result<List<Video>>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideosByGroupAndPositionQueryHandler(
        IVideoRepository videoRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoRepository = videoRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<Video>>> Handle(GetVideosByGroupAndPositionQuery request, CancellationToken cancellationToken)
    {
        var videos = await _videoRepository.GetVideosAsync(request.VideoGroupId, request.PositionInQueue, request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videos,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(videos);
    }
}
