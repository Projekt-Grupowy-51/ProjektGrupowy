using MediatR;
using VidMark.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Videos.Commands.UpdateVideo;

public class UpdateVideoCommandHandler : IRequestHandler<UpdateVideoCommand, Result<Video>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public UpdateVideoCommandHandler(
        IVideoRepository videoRepository,
        IVideoGroupRepository videoGroupRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _videoRepository = videoRepository;
        _videoGroupRepository = videoGroupRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Video>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.GetVideoAsync(request.VideoId, request.UserId, request.IsAdmin);

        if (video is null)
        {
            return Result.Fail("No video found");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            video,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroup = await _videoGroupRepository.GetVideoGroupAsync(request.VideoGroupId, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found!");
        }

        var authResultVG = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResultVG.Succeeded)
        {
            throw new ForbiddenException();
        }

        string? newVideoPath = null;
        string? newContentType = null;

        if (request.File is not null)
        {
            var filename = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            newVideoPath = Path.Combine(_configuration["Videos:Path"]!, filename);

            await using (var fileStream = new FileStream(newVideoPath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream, cancellationToken);
            }

            newContentType = request.File.ContentType;
        }

        video.Update(request.Title, videoGroup, newVideoPath, newContentType, request.PositionInQueue, request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(video);
    }
}
