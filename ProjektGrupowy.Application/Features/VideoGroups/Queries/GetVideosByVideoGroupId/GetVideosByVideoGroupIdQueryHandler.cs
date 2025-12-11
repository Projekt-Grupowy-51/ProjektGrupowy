using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideosByVideoGroupId;

public class GetVideosByVideoGroupIdQueryHandler : IRequestHandler<GetVideosByVideoGroupIdQuery, Result<List<Video>>>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideosByVideoGroupIdQueryHandler(
        IVideoGroupRepository videoGroupRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoGroupRepository = videoGroupRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<Video>>> Handle(GetVideosByVideoGroupIdQuery request, CancellationToken cancellationToken)
    {
        var videos = await _videoGroupRepository.GetVideosByVideoGroupIdAsync(request.Id, request.UserId, request.IsAdmin);

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
