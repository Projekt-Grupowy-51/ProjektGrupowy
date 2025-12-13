using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.AssignedLabels.Commands.AddAssignedLabel;

public class AddAssignedLabelCommandHandler : IRequestHandler<AddAssignedLabelCommand, Result<AssignedLabel>>
{
    private readonly IAssignedLabelRepository _assignedLabelRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddAssignedLabelCommandHandler(
        IAssignedLabelRepository assignedLabelRepository,
        ILabelRepository labelRepository,
        IVideoRepository videoRepository,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IUnitOfWork unitOfWork)
    {
        _assignedLabelRepository = assignedLabelRepository;
        _labelRepository = labelRepository;
        _videoRepository = videoRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssignedLabel>> Handle(AddAssignedLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _labelRepository.GetLabelAsync(request.LabelId, request.UserId, request.IsAdmin);

        if (label is null)
        {
            return Result.Fail("Label does not exist");
        }

        var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var video = await _videoRepository.GetVideoAsync(request.VideoId, request.UserId, request.IsAdmin);
        if (video is null)
        {
            return Result.Fail("No subject video group assignment found");
        }

        var authResultVideo = await _authorizationService.AuthorizeAsync(_currentUserService.User, video, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResultVideo.Succeeded)
        {
            throw new ForbiddenException();
        }

        var assignedLabel = AssignedLabel.Create(label, request.UserId, video, request.Start, request.End);

        await _assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return assignedLabel;
    }
}
