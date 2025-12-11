using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Videos.Queries.GetVideos;

public class GetVideosQueryHandler : IRequestHandler<GetVideosQuery, Result<List<Video>>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideosQueryHandler(
        IVideoRepository videoRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoRepository = videoRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<Video>>> Handle(GetVideosQuery request, CancellationToken cancellationToken)
    {
        var videos = await _videoRepository.GetVideosAsync(request.UserId, request.IsAdmin);

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
