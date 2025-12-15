using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VidMark.Application.Authorization;
using VidMark.Application.Exceptions;
using VidMark.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Interfaces.UnitOfWork;
using VidMark.Application.Services;
using VidMark.Domain.Models;

namespace VidMark.Application.Features.Projects.Commands.DistributeLabelersEqually;

public class DistributeLabelersEquallyCommandHandler(
    IProjectRepository projectRepository,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    IMediator mediator)
    : IRequestHandler<DistributeLabelersEquallyCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DistributeLabelersEquallyCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var totalAssignmentCount = project.Subjects.Sum(subject => subject.SubjectVideoGroupAssignments.Count);
        if (totalAssignmentCount == 0)
        {
            return Result.Fail("There are no assignments.");
        }

        var unassignedLabelersQuery = new GetUnassignedLabelersOfProjectQuery(request.ProjectId, request.UserId, request.IsAdmin);
        var unassignedLabelersResult = await mediator.Send(unassignedLabelersQuery, cancellationToken);

        if (unassignedLabelersResult.IsFailed)
        {
            return Result.Fail("No unassigned labelers found!");
        }

        var unassignedLabelers = unassignedLabelersResult.Value.ToList();

        var result = await AssignEquallyAsync(request.ProjectId, unassignedLabelers, request.UserId, request.IsAdmin, cancellationToken);

        return result;
    }

    private async Task<Result<bool>> AssignEquallyAsync(int projectId, IReadOnlyCollection<User> labelers, string userId, bool isAdmin, CancellationToken cancellationToken)
    {
        if (labelers.Count == 0)
        {
            return Result.Ok(true);
        }

        var assignmentsCount = await projectRepository.GetLabelerCountForAssignments(projectId, userId, isAdmin);

        if (assignmentsCount.Count == 0)
        {
            return Result.Fail("No assignments found.");
        }

        // Sort assignments by current count (ascending) to fill up the least populated ones first
        var sortedAssignments = assignmentsCount
            .OrderBy(x => x.Value)
            .ToList();

        // Create a distribution plan using round robin
        var distributionPlan = new Dictionary<int, List<User>>();
        foreach (var (assignmentId, _) in sortedAssignments)
        {
            distributionPlan[assignmentId] = new List<User>();
        }

        // Distribute labelers in round robin fashion
        var labelersList = labelers.ToList();
        var assignmentIndex = 0;

        foreach (var labeler in labelersList)
        {
            var (assignmentId, _) = sortedAssignments[assignmentIndex];
            distributionPlan[assignmentId].Add(labeler);

            assignmentIndex = (assignmentIndex + 1) % sortedAssignments.Count;
        }

        // Execute the distribution plan
        try
        {
            foreach (var (assignmentId, labelersToAssign) in distributionPlan)
            {
                if (labelersToAssign.Count == 0)
                {
                    continue;
                }

                var assignResult = await AssignLabelerToAssignmentAsync(assignmentId, labelersToAssign, userId, isAdmin);

                if (assignResult.IsFailed)
                {
                    return Result.Fail("Failed to assign labeler to assignment");
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(true);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private async Task<Result<bool>> AssignLabelerToAssignmentAsync(int assignmentId, IEnumerable<User> labelers, string userId, bool isAdmin)
    {
        var assignment = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, isAdmin);

        if (assignment is null)
        {
            return Result.Fail("No assignment found!");
        }

        var authResult = await authorizationService.AuthorizeAsync(
            currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Modify));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        assignment.AddLabelers(labelers, userId);

        return Result.Ok(true);
    }
}
