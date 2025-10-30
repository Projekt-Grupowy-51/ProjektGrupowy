using MediatR;
using ProjektGrupowy.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Features.Videos.Commands.AddVideo;

public class AddVideoCommandHandler : IRequestHandler<AddVideoCommand, Result<Video>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoGroupRepository _videoGroupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public AddVideoCommandHandler(
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

    public async Task<Result<Video>> Handle(AddVideoCommand request, CancellationToken cancellationToken)
    {
        var videoGroup = await _videoGroupRepository.GetVideoGroupAsync(request.VideoGroupId, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Create));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoProject = videoGroup.Project;

        var videoGroupId = videoGroup.Id.ToString();
        var videoProjectId = videoProject.Id.ToString();

        var videoRootDirectory = _configuration["Videos:RootDirectory"] ?? "videos";

        var filename = $"{Guid.NewGuid():N}{Path.GetExtension(request.File.FileName)}";

        var directoryPath = Path.Combine(AppContext.BaseDirectory, videoRootDirectory, videoProjectId, videoGroupId);

        Directory.CreateDirectory(directoryPath);

        var videoPath = Path.Combine(directoryPath, filename);

        await using (var fileStream = new FileStream(videoPath, FileMode.Create))
        {
            await request.File.CopyToAsync(fileStream, cancellationToken);
        }

        var video = Video.Create(
            request.Title,
            videoPath,
            videoGroup,
            request.File.ContentType,
            request.PositionInQueue,
            request.UserId);

        await _videoRepository.AddVideoAsync(video);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(video);
    }
}
