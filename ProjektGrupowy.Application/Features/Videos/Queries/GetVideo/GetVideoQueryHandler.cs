using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Videos.Queries.GetVideo;

public class GetVideoQueryHandler : IRequestHandler<GetVideoQuery, Result<Video>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideoQueryHandler(
        IVideoRepository videoRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoRepository = videoRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Video>> Handle(GetVideoQuery request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetVideoAsync(request.Id, request.UserId, request.IsAdmin);

        if (video is null)
        {
            return Result.Fail("No video found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            video,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(video);
    }
}
