using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.Authorization;
using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Exceptions;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectService(
    IProjectRepository projectRepository,
    IMessageService messageService,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    UserManager<User> userManager) : IProjectService
{
    public async Task<Optional<IEnumerable<Project>>> GetProjectsAsync()
    {
        var projectsOptional = await projectRepository.GetProjectsAsync();
        if (projectsOptional.IsFailure)
        {
            return projectsOptional;
        }

        var projects = projectsOptional.GetValueOrThrow();

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, projects, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }
        
        return projectsOptional;
    }

    public async Task<Optional<Project>> GetProjectAsync(int id, string? userId = null, bool? isAdmin = null)
    {
        var projectOptional = await projectRepository.GetProjectAsync(id, userId, isAdmin);
        if (projectOptional.IsFailure)
        {
            return projectOptional;
        }

        var project = projectOptional.GetValueOrThrow();

        if (userId == null)
        {
            var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Read));
            if (!authResult.Succeeded && userId != null)
            {
                throw new ForbiddenException();
            }
        }

        return projectOptional;
    }

    public async Task<Optional<Project>> AddProjectAsync(ProjectRequest projectRequest)
    {
        var project = new Project
        {
            Name = projectRequest.Name,
            Description = projectRequest.Description,
            CreatedById = currentUserService.UserId,
            CreationDate = DateOnly.FromDateTime(DateTime.Today)
        };

        var projectOptional = await projectRepository.AddProjectAsync(project);

        if (projectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to create project");
        }
        else 
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                $"Project \"{projectRequest.Name}\" created successfully");
        }

        return projectOptional;
    }

    public async Task<Optional<Project>> UpdateProjectAsync(int projectId, ProjectRequest projectRequest)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);

        if (projectOptional.IsFailure)
        {
            return projectOptional;
        }

        var project = projectOptional.GetValueOrThrow();

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        project.Name = projectRequest.Name;
        project.Description = projectRequest.Description;
        project.CreatedById = currentUserService.UserId;
        project.ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        project.EndDate = projectRequest.Finished ? DateOnly.FromDateTime(DateTime.Today) : null;

        var result = await projectRepository.UpdateProjectAsync(project);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to update project");
        }
        else 
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Project updated successfully");
        }

        return result;
    }

    public async Task<Optional<bool>> AddLabelerToProjectAsync(LabelerAssignmentDto labelerAssignmentDto)
    {
        var joinerId = currentUserService.UserId;

        var projectByCodeOpt = await projectRepository.GetProjectByAccessCodeAsync(labelerAssignmentDto.AccessCode);

        var labelerOpt = await userManager.FindByIdAsync(joinerId);

        if (labelerOpt == null)
        {
            return Optional<bool>.Failure("No labeler found!");
        }

        if (projectByCodeOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                joinerId,
                "No project found!");
            return Optional<bool>.Failure("No project found!");
        }

        var project = projectByCodeOpt.GetValueOrThrow();

        project.ProjectLabelers.Add(labelerOpt);

        await projectRepository.UpdateProjectAsync(project);

        await messageService.SendSuccessAsync(
            joinerId,
            "Successfully joined the project!");

        await messageService.SendInfoAsync(
            project.CreatedById,
            $"Labeler \"{labelerOpt.UserName}\" joined the project \"{project.Name}\"");

        await messageService.SendMessageAsync(
            project.CreatedById, 
            MessageTypes.LabelersCountChanged,
            project.ProjectLabelers.Count);

        return Optional<bool>.Success(true);
    }

    public async Task<Optional<IEnumerable<User>>> GetUnassignedLabelersOfProjectAsync(int id)
    {
        var projectOptional = await projectRepository.GetProjectAsync(id);
        if (projectOptional.IsFailure)
        {
            return Optional<IEnumerable<User>>.Failure("No project found!");
        }

        var project = projectOptional.GetValueOrThrow();

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var unassignedLabelers = project.ProjectLabelers
            .Where(labeler => !project.Subjects
                .SelectMany(subject => subject.SubjectVideoGroupAssignments)
                .Any(assignment => assignment.Labelers!.Contains(labeler)))
            .ToList();

        return Optional<IEnumerable<User>>.Success(unassignedLabelers);
    }

    public async Task DeleteProjectAsync(int id)
    {
        var projectOpt = await projectRepository.GetProjectAsync(id);

        if (projectOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No project found!");
            return;
        }

        var project = projectOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await messageService.SendInfoAsync(currentUserService.UserId, $"Project \"{project.Name}\" deleted successfully");
        
        await projectRepository.DeleteProjectAsync(project);
    }

    public async Task<Optional<bool>> UnassignLabelersFromProjectAsync(int projectId)
    {
        var projectOpt = await projectRepository.GetProjectAsync(projectId);
        if (projectOpt.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }

        var project = projectOpt.GetValueOrThrow();

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var assignments = project.Subjects
            .SelectMany(s => s.SubjectVideoGroupAssignments)
            .ToList();

        foreach (var assignment in assignments)
        {
            assignment.Labelers.Clear();
            await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(assignment);
        }

        await messageService.SendSuccessAsync(currentUserService.UserId, "Labelers unassigned from project successfully");

        return Optional<bool>.Success(true);
    }

    public async Task<Optional<bool>> DistributeLabelersEquallyAsync(int projectId)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);
        if (projectOptional.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }
        
        var project = projectOptional.GetValueOrThrow();

        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var totalAssignmentCount = project.Subjects.Sum(subject => subject.SubjectVideoGroupAssignments.Count);
        if (totalAssignmentCount == 0)
        {
            await messageService.SendErrorAsync(currentUserService.UserId, "There are no assignments.");
            return Optional<bool>.Failure("There are no assignments.");
        }

        var unassignedLabelersResult = await GetUnassignedLabelersOfProjectAsync(projectId);
        if (unassignedLabelersResult.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No unassigned labelers found!");
            return Optional<bool>.Failure("No unassigned labelers found!");
        }

        var unassignedLabelers = unassignedLabelersResult.GetValueOrThrow().ToList();

        var result = await AssignEquallyAsync(projectId, unassignedLabelers);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to assign labelers");
        }
        else 
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Labelers assigned successfully");
        }

        return result;
    }

    private async Task<Optional<bool>> AssignEquallyAsync(int projectId, IReadOnlyCollection<User> labelers)
    {
        if (labelers.Count == 0)
        {
            return Optional<bool>.Success(true);
        }

        var countResult = await projectRepository.GetLabelerCountForAssignments(projectId);
        if (countResult.IsFailure)
        {
            return Optional<bool>.Failure("Failed to get labeler count for assignments");
        }

        var assignmentsCount = countResult.GetValueOrThrow();

        var n = assignmentsCount.Count;
        var totalSize = assignmentsCount.Values.Sum() + labelers.Count;
        var targetSize = Math.Max(1, totalSize / n);

        var remaining = labelers.Count;
        var assigned = 0;

        await using var transaction = await projectRepository.BeginTransactionAsync();

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

                var assignResult = await AssignLabelerToAssignmentAsync(assignmentId, toAssignList);

                if (assignResult.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<bool>.Failure("Failed to assign labeler to assignment");
                }

                assigned += toAssign;
                remaining -= toAssign;
            }

            // Handle remaining labelers
            if (remaining > 0)
            {
                var lastAssignmentId = assignmentsCount.Keys.Last();
                var assignResult = await AssignLabelerToAssignmentAsync(lastAssignmentId, labelers.Skip(assigned));
                if (assignResult.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<bool>.Failure("Failed to assign labeler to assignment");
                }
            }

            await transaction.CommitAsync();
            return Optional<bool>.Success(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Optional<bool>.Failure(e.Message);
        }
    }

    private async Task<Optional<bool>> AssignLabelerToAssignmentAsync(int assignmentId, IEnumerable<User> labelers)
    {
        var assignmentOptional =
            await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId);
        if (assignmentOptional.IsFailure)
        {
            return Optional<bool>.Failure("No assignment found!");
        }

        var assignment = assignmentOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignment, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        foreach (var labeler in labelers)
        {
            assignment.Labelers!.Add(labeler);
        }

        await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(assignment);

        return Optional<bool>.Success(true);
    }

    public async Task<Optional<IEnumerable<User>>> GetLabelersByProjectAsync(int projectId)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);
        if (projectOptional.IsFailure)
        {
            return Optional<IEnumerable<User>>.Failure("No project found!");
        }

        var project = projectOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return Optional<IEnumerable<User>>.Success(project.ProjectLabelers);
    }

    public async Task<Optional<Project>> UpdateProjectAsync(Project project) =>
        await projectRepository.UpdateProjectAsync(project);
}