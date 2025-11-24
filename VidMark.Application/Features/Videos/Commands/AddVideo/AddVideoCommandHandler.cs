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

namespace VidMark.Application.Features.Videos.Commands.AddVideo;

public class AddVideoCommandHandler(
    IVideoRepository videoRepository,
    IVideoGroupRepository videoGroupRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<AddVideoCommand, Result<Video>>
{
    public async Task<Result<Video>> Handle(AddVideoCommand request, CancellationToken cancellationToken)
    {
        var videoGroup = await videoGroupRepository.GetVideoGroupAsync(request.VideoGroupId, request.UserId, request.IsAdmin);

        if (videoGroup is null)
        {
            return Result.Fail("No video group found!");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Create));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoProject = videoGroup.Project;

        var videoGroupId = videoGroup.Id.ToString();
        var videoProjectId = videoProject.Id.ToString();

        var videoRootDirectory = configuration["Videos:RootDirectory"] ?? "videos";

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

        await videoRepository.AddVideoAsync(video);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(video);
    }
}
