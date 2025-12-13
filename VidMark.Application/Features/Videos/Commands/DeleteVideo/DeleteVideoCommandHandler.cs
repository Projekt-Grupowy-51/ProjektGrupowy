using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Videos.Commands.DeleteVideo;

public class DeleteVideoCommandHandler(
    IVideoRepository videoRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<DeleteVideoCommand, Result>
{
    public async Task<Result> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.GetVideoAsync(request.Id, request.UserId, request.IsAdmin);

        if (video is null)
        {
            return Result.Fail("No video found");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            video,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // TODO: add domain event for video deletion
        videoRepository.DeleteVideo(video);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
