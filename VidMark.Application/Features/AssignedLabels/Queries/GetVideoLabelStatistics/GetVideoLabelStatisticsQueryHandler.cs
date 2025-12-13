using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.AssignedLabels.Queries.GetVideoLabelStatistics;

public class GetVideoLabelStatisticsQueryHandler(
    IAssignedLabelRepository assignedLabelRepository,
    IVideoRepository videoRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetVideoLabelStatisticsQuery, Result<VideoLabelStatistics>>
{
    public async Task<Result<VideoLabelStatistics>> Handle(GetVideoLabelStatisticsQuery request, CancellationToken cancellationToken)
    {
        // First check if video exists and user has access
        var video = await videoRepository.GetVideoAsync(request.VideoId, request.UserId, request.IsAdmin);
        if (video is null)
        {
            return Result.Fail("Video not found");
        }

        var videoAuthResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            video,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!videoAuthResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // Get all assigned labels for this video
        var assignedLabels = await assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(
            request.VideoId,
            request.UserId,
            request.IsAdmin);

        if (assignedLabels is null || !assignedLabels.Any())
        {
            return new VideoLabelStatistics
            {
                TotalLabels = 0,
                LabelsByType = new Dictionary<string, int>(),
                LabelsBySubject = new Dictionary<string, int>(),
                LabelsByLabeler = new Dictionary<string, int>()
            };
        }

        // Calculate statistics
        var statistics = new VideoLabelStatistics
        {
            TotalLabels = assignedLabels.Count,
            LabelsByType = assignedLabels
                .GroupBy(al => al.Label.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            LabelsBySubject = assignedLabels
                .GroupBy(al => al.Label.Subject.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            LabelsByLabeler = assignedLabels
                .GroupBy(al => al.CreatedBy.UserName)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return Result.Ok(statistics);
    }
}
