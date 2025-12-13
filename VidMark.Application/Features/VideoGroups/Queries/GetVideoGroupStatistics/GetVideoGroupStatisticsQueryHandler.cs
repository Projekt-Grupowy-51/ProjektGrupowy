using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.VideoGroups.Queries.GetVideoGroupStatistics;

public class GetVideoGroupStatisticsQueryHandler(
    IVideoGroupRepository videoGroupRepository,
    IAssignmentCompletionRepository completionRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetVideoGroupStatisticsQuery, Result<VideoGroupStatistics>>
{
    public async Task<Result<VideoGroupStatistics>> Handle(
        GetVideoGroupStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var videoGroup = await videoGroupRepository.GetVideoGroupAsync(
            request.VideoGroupId,
            request.UserId,
            request.IsAdmin);

        if (videoGroup == null)
        {
            return Result.Fail("Video group not found");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            videoGroup,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videos = videoGroup.Videos.ToList();

        // Get all labels for videos in this video group
        var allLabels = videos
            .SelectMany(v => v.AssignedLabels)
            .ToList();

        // Calculate labels by type
        var labelsByType = allLabels
            .GroupBy(al => al.Label.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        // Calculate labels by subject
        var labelsBySubject = allLabels
            .GroupBy(al => al.Label.Subject.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        // Calculate video progress (how many labels each video has)
        var videoProgress = videos
            .ToDictionary(
                v => v.Id.ToString(),
                v => v.AssignedLabels.Count
            );

        // Calculate completed assignments (based on completion table)
        var assignments = videoGroup.SubjectVideoGroupAssignments.ToList();

        // Get completion status for all assignments in this video group
        var allCompletions = new List<Domain.Models.SubjectVideoGroupAssignmentCompletion>();
        foreach (var assignment in assignments)
        {
            var completions = await completionRepository.GetByAssignmentIdAsync(assignment.Id);
            allCompletions.AddRange(completions);
        }

        // Assignment is completed when all assigned labelers marked it as completed
        var completedAssignments = assignments.Count(a =>
        {
            var assignmentLabelers = a.Labelers.Select(l => l.Id).ToList();
            if (!assignmentLabelers.Any()) return false;

            var assignmentCompletions = allCompletions
                .Where(c => c.SubjectVideoGroupAssignmentId == a.Id)
                .ToList();

            return assignmentLabelers.All(labelerId =>
                assignmentCompletions.Any(c => c.LabelerId == labelerId && c.IsCompleted));
        });

        var completionPercentage = assignments.Count > 0
            ? Math.Round((double)completedAssignments / assignments.Count * 100, 2)
            : 0;

        var statistics = new VideoGroupStatistics
        {
            TotalVideos = videos.Count,
            TotalLabels = allLabels.Count,
            CompletedVideos = completedAssignments,
            CompletionPercentage = completionPercentage,
            LabelsByType = labelsByType,
            LabelsBySubject = labelsBySubject,
            VideoProgress = videoProgress
        };

        return Result.Ok(statistics);
    }
}
