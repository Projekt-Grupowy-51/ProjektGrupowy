using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Features.Projects.Commands.DistributeLabelersEqually;

public class DistributeLabelersEquallyCommandHandler : IRequestHandler<DistributeLabelersEquallyCommand, Result<bool>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISubjectVideoGroupAssignmentRepository _subjectVideoGroupAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMediator _mediator;

    public DistributeLabelersEquallyCommandHandler(
        IProjectRepository projectRepository,
        ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IMediator mediator)
    {
        _projectRepository = projectRepository;
        _subjectVideoGroupAssignmentRepository = subjectVideoGroupAssignmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(DistributeLabelersEquallyCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectAsync(request.ProjectId, request.UserId, request.IsAdmin);

        if (project is null)
        {
            return Result.Fail("No project found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            project,
            new ResourceOperationRequirement(ResourceOperation.Update));

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
        var unassignedLabelersResult = await _mediator.Send(unassignedLabelersQuery, cancellationToken);

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

        var assignmentsCount = await _projectRepository.GetLabelerCountForAssignments(projectId, userId, isAdmin);

        var n = assignmentsCount.Count;
        var totalSize = assignmentsCount.Values.Sum() + labelers.Count;
        var targetSize = Math.Max(1, totalSize / n);

        var remaining = labelers.Count;
        var assigned = 0;

        try
        {
            foreach (var (assignmentId, labelerCount) in assignmentsCount)
            {
                var toAssign = Math.Max(0, targetSize - labelerCount);

                if (toAssign == 0)
                {
                    continue;
                }

                var toAssignList = labelers
                    .Skip(assigned)
                    .Take(toAssign);

                var assignResult = await AssignLabelerToAssignmentAsync(assignmentId, toAssignList, userId, isAdmin);

                if (assignResult.IsFailed)
                {
                    return Result.Fail("Failed to assign labeler to assignment");
                }

                assigned += toAssign;
                remaining -= toAssign;
            }

            // Handle remaining labelers
            if (remaining > 0)
            {
                var lastAssignmentId = assignmentsCount.Keys.Last();
                var assignResult = await AssignLabelerToAssignmentAsync(lastAssignmentId, labelers.Skip(assigned), userId, isAdmin);
                if (assignResult.IsFailed)
                {
                    return Result.Fail("Failed to assign labeler to assignment");
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(true);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    private async Task<Result<bool>> AssignLabelerToAssignmentAsync(int assignmentId, IEnumerable<User> labelers, string userId, bool isAdmin)
    {
        var assignment = await _subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId, userId, isAdmin);

        if (assignment is null)
        {
            return Result.Fail("No assignment found!");
        }

        var authResult = await _authorizationService.AuthorizeAsync(
            _currentUserService.User,
            assignment,
            new ResourceOperationRequirement(ResourceOperation.Update));

        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        assignment.AddLabelers(labelers, userId);

        return Result.Ok(true);
    }
}
