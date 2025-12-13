using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideoGroups;

public class GetVideoGroupsQueryHandler : IRequestHandler<GetVideoGroupsQuery, Result<List<VideoGroup>>>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideoGroupsQueryHandler(
        IVideoGroupRepository videoGroupRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoGroupRepository = videoGroupRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<VideoGroup>>> Handle(GetVideoGroupsQuery request, CancellationToken cancellationToken)
    {
        var videoGroups = await _videoGroupRepository.GetVideoGroupsAsync(request.UserId, request.IsAdmin);

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroups,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(videoGroups);
    }
}
