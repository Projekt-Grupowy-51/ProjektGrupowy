using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideoGroup;

public class GetVideoGroupQueryHandler : IRequestHandler<GetVideoGroupQuery, Result<VideoGroup>>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideoGroupQueryHandler(
        IVideoGroupRepository videoGroupRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoGroupRepository = videoGroupRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<VideoGroup>> Handle(GetVideoGroupQuery request, CancellationToken cancellationToken)
    {
        var videoGroup = await _videoGroupRepository.GetVideoGroupAsync(request.Id, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Result.Ok(videoGroup);
    }
}
