using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;

namespace VidMark.Application.Features.Subjects.Queries.GetSubjectStatistics;

public class GetSubjectStatisticsQueryHandler(
    ISubjectRepository subjectRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetSubjectStatisticsQuery, Result<SubjectStatistics>>
{
    public async Task<Result<SubjectStatistics>> Handle(
        GetSubjectStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var subject = await subjectRepository.GetSubjectAsync(
            request.SubjectId,
            request.UserId,
            request.IsAdmin);

        if (subject == null)
        {
            return Result.Fail("Subject not found");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            subject,
            new ResourceOperationRequirement(ResourceOperation.Read));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        // Get all labels for this subject
        var labels = subject.Labels.ToList();

        // Get all assigned labels for this subject
        var assignedLabels = labels
            .SelectMany(l => l.AssignedLabels)
            .ToList();

        // Calculate label usage by type (how many times each label was used)
        var labelUsageByType = labels
            .ToDictionary(
                l => l.Name,
                l => l.AssignedLabels.Count
            );

        // Calculate labels by labeler
        var labelsByLabeler = assignedLabels
            .GroupBy(al => al.CreatedBy.UserName)
            .ToDictionary(g => g.Key, g => g.Count());

        // Calculate labels by project
        var labelsByProject = assignedLabels
            .GroupBy(al => al.Video.VideoGroup.Project.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        // Count unique videos that have been labeled with this subject
        var uniqueVideosLabeled = assignedLabels
            .Select(al => al.Video.Id)
            .Distinct()
            .Count();

        // Count total assignments for this subject
        var totalAssignments = subject.SubjectVideoGroupAssignments.Count;

        var statistics = new SubjectStatistics
        {
            TotalLabels = labels.Count,
            TotalAssignedLabels = assignedLabels.Count,
            UniqueVideosLabeled = uniqueVideosLabeled,
            TotalAssignments = totalAssignments,
            LabelUsageByType = labelUsageByType,
            LabelsByLabeler = labelsByLabeler,
            LabelsByProject = labelsByProject
        };

        return Result.Ok(statistics);
    }
}
