using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetAssignmentStatistics;

public class GetAssignmentStatisticsQueryHandler(
    ISubjectVideoGroupAssignmentRepository assignmentRepository,
    IAssignmentCompletionRepository completionRepository,
    IKeycloakUserRepository userRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetAssignmentStatisticsQuery, Result<AssignmentStatistics>>
{
    public async Task<Result<AssignmentStatistics>> Handle(
        GetAssignmentStatisticsQuery request,
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

        var videos = assignment.VideoGroup.Videos.ToList();
        var assignedLabelerIds = assignment.Labelers.Select(u => u.Id).ToList();
        var expectedLabelsPerVideo = assignedLabelerIds.Count;

        // Get all labels for this subject in these videos
        var allLabels = videos
            .SelectMany(v => v.AssignedLabels)
            .Where(al => al.Label.Subject.Id == assignment.Subject.Id)
            .ToList();

        // Calculate video progress
        var videoProgressList = new List<VideoProgress>();
        int completedVideos = 0;
        int inProgressVideos = 0;
        int notStartedVideos = 0;

        foreach (var video in videos)
        {
            var videoLabels = video.AssignedLabels
                .Where(al => al.Label.Subject.Id == assignment.Subject.Id &&
                             assignedLabelerIds.Contains(al.CreatedById))
                .ToList();

            var uniqueLabelers = videoLabels.Select(al => al.CreatedById).Distinct().Count();
            var completionPercentage = expectedLabelsPerVideo > 0
                ? Math.Round((double)uniqueLabelers / expectedLabelsPerVideo * 100, 2)
                : 0;

            videoProgressList.Add(new VideoProgress
            {
                Id = video.Id,
                Title = video.Title,
                LabelsReceived = uniqueLabelers,
                ExpectedLabels = expectedLabelsPerVideo,
                CompletionPercentage = completionPercentage
            });

            if (uniqueLabelers == expectedLabelsPerVideo && expectedLabelsPerVideo > 0)
                completedVideos++;
            else if (uniqueLabelers > 0)
                inProgressVideos++;
            else
                notStartedVideos++;
        }

        // Calculate labeler statistics
        var labelerStats = new Dictionary<string, (int count, string name, string email)>();

        foreach (var labelerId in assignedLabelerIds)
        {
            var labelerLabelCount = allLabels.Count(al => al.CreatedById == labelerId);

            // Fetch user info
            var user = await userRepository.FindByIdAsync(labelerId);
            var name = user != null
                ? $"{user.FirstName} {user.LastName}".Trim()
                : "Unknown";
            if (string.IsNullOrWhiteSpace(name))
            {
                name = user?.UserName ?? "Unknown";
            }
            var email = user?.Email ?? "";

            labelerStats[labelerId] = (labelerLabelCount, name, email);
        }

        // Top 10 labelers
        var topLabelers = labelerStats
            .OrderByDescending(kvp => kvp.Value.count)
            .Take(10)
            .ToDictionary(kvp => kvp.Value.name, kvp => kvp.Value.count);

        // Get completion records for this assignment
        var completions = await completionRepository.GetByAssignmentIdAsync(request.AssignmentId);
        var completionsDict = completions.ToDictionary(c => c.LabelerId, c => c.IsCompleted);
        var completedCount = completions.Count(c => c.IsCompleted);
        var totalLabelers = assignedLabelerIds.Count;

        // All labelers with detailed stats
        var allLabelersList = labelerStats
            .OrderByDescending(kvp => kvp.Value.count)
            .Select(kvp => new LabelerStat
            {
                Id = kvp.Key,
                Name = kvp.Value.name,
                Email = kvp.Value.email,
                LabelCount = kvp.Value.count,
                CompletionPercentage = completionsDict.GetValueOrDefault(kvp.Key, false) ? 100 : 0
            })
            .ToList();

        // Overall completion percentage based on labeler completions
        var overallCompletion = totalLabelers > 0
            ? Math.Round((double)completedCount / totalLabelers * 100, 2)
            : 0;

        var statistics = new AssignmentStatistics
        {
            TotalVideos = videos.Count,
            TotalLabels = allLabels.Count,
            AssignedLabelersCount = assignedLabelerIds.Count,
            CompletionPercentage = overallCompletion,
            Videos = videoProgressList,
            TopLabelers = topLabelers,
            AllLabelers = allLabelersList,
            VideoStatus = new VideoStatusCounts
            {
                Completed = completedVideos,
                InProgress = inProgressVideos,
                NotStarted = notStartedVideos
            }
        };

        return Result.Ok(statistics);
    }
}
