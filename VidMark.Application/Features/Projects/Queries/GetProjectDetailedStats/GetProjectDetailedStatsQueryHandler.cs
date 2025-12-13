using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Projects.Queries.GetProjectDetailedStats;

public class GetProjectDetailedStatsQueryHandler(
    IProjectRepository projectRepository,
    IAssignmentCompletionRepository completionRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetProjectDetailedStatsQuery, Result<ProjectDetailedStatistics>>
{
    public async Task<Result<ProjectDetailedStatistics>> Handle(
        GetProjectDetailedStatsQuery request,
        CancellationToken cancellationToken)
    {
        // Get project with all related data
        var project = await projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("Project not found");
        }

        // Authorization check
        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // Calculate statistics
        var assignments = project.Subjects
            .SelectMany(s => s.SubjectVideoGroupAssignments)
            .DistinctBy(a => a.Id)
            .ToList();

        var allLabels = project.Subjects
            .SelectMany(s => s.Labels)
            .SelectMany(l => l.AssignedLabels)
            .Where(al => al.Video.VideoGroup.Project.Id == project.Id)
            .ToList();

        var totalVideos = project.VideoGroups
            .SelectMany(vg => vg.Videos)
            .Count();

        // Get completion status for all assignments in this project
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

        var statistics = new ProjectDetailedStatistics
        {
            TotalSubjects = project.Subjects.Count,
            TotalVideoGroups = project.VideoGroups.Count,
            TotalVideos = totalVideos,
            TotalAssignments = assignments.Count,
            TotalLabelers = project.ProjectLabelers.Count,
            TotalLabels = allLabels.Count,
            LabelsBySubject = allLabels
                .GroupBy(al => al.Label.Subject.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            LabelsByLabeler = allLabels
                .GroupBy(al => al.CreatedBy.UserName)
                .ToDictionary(g => g.Key, g => g.Count()),
            LabelsByType = allLabels
                .GroupBy(al => al.Label.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            CompletedAssignments = completedAssignments,
            ProgressPercentage = assignments.Count > 0
                ? Math.Round((double)completedAssignments / assignments.Count * 100, 2)
                : 0
        };

        return Result.Ok(statistics);
    }
}
