using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetLabelerAssignmentStatistics;

public class GetLabelerAssignmentStatisticsQueryHandler(
    ISubjectVideoGroupAssignmentRepository assignmentRepository,
    IAssignmentCompletionRepository completionRepository,
    IKeycloakUserRepository userRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetLabelerAssignmentStatisticsQuery, Result<LabelerAssignmentStatistics>>
{
    public async Task<Result<LabelerAssignmentStatistics>> Handle(
        GetLabelerAssignmentStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var assignment = await assignmentRepository.GetSubjectVideoGroupAssignmentAsync(
            request.AssignmentId,
            request.UserId,
            request.IsAdmin);

        if (assignment == null)
        {
            return Result.Fail("Assignment not found");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // Check if labeler is assigned to this assignment
        if (!assignment.Labelers.Any(l => l.Id == request.LabelerId))
        {
            return Result.Fail("Labeler is not assigned to this assignment");
        }

        // Get labeler info
        var labeler = await userRepository.FindByIdAsync(request.LabelerId);
        var labelerName = labeler != null
            ? $"{labeler.FirstName} {labeler.LastName}".Trim()
            : "Unknown";
        if (string.IsNullOrWhiteSpace(labelerName))
        {
            labelerName = labeler?.UserName ?? "Unknown";
        }
        var labelerEmail = labeler?.Email ?? "";

        // Get all videos in the video group
        var videos = assignment.VideoGroup.Videos.ToList();
        var totalVideos = videos.Count;

        // Get all labels created by this labeler for this subject
        var labelerLabels = videos
            .SelectMany(v => v.AssignedLabels)
            .Where(al => al.Label.Subject.Id == assignment.Subject.Id &&
                        al.CreatedById == request.LabelerId)
            .ToList();

        var totalLabels = labelerLabels.Count;

        // Calculate which videos have been labeled by this labeler
        var videoProgress = new List<VideoLabelingProgress>();
        int labeledVideos = 0;

        foreach (var video in videos)
        {
            var videoLabels = video.AssignedLabels
                .Where(al => al.Label.Subject.Id == assignment.Subject.Id &&
                            al.CreatedById == request.LabelerId)
                .ToList();

            var hasLabeled = videoLabels.Any();
            if (hasLabeled)
            {
                labeledVideos++;
            }

            videoProgress.Add(new VideoLabelingProgress
            {
                VideoId = video.Id,
                VideoTitle = video.Title,
                LabelCount = videoLabels.Count,
                HasLabeled = hasLabeled
            });
        }

        // Check completion status
        var completion = await completionRepository.GetByAssignmentAndLabelerAsync(
            request.AssignmentId,
            request.LabelerId);
        var isCompleted = completion?.IsCompleted ?? false;

        var statistics = new LabelerAssignmentStatistics
        {
            LabelerId = request.LabelerId,
            LabelerName = labelerName,
            LabelerEmail = labelerEmail,
            AssignmentId = assignment.Id,
            SubjectName = assignment.Subject.Name,
            VideoGroupName = assignment.VideoGroup.Name,
            TotalVideos = totalVideos,
            LabeledVideos = labeledVideos,
            TotalLabels = totalLabels,
            IsCompleted = isCompleted,
            Videos = videoProgress
        };

        return Result.Ok(statistics);
    }
}
