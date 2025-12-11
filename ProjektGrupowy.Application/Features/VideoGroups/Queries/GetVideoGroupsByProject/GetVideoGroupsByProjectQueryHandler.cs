using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideoGroupsByProject;

public class GetVideoGroupsByProjectQueryHandler : IRequestHandler<GetVideoGroupsByProjectQuery, Result<List<VideoGroup>>>
{
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public GetVideoGroupsByProjectQueryHandler(
        IVideoGroupRepository videoGroupRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoGroupRepository = videoGroupRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<List<VideoGroup>>> Handle(GetVideoGroupsByProjectQuery request, CancellationToken cancellationToken)
    {
        var videoGroups = await _videoGroupRepository.GetVideoGroupsByProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

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
