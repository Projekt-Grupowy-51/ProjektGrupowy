using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectService(
    IProjectRepository projectRepository,
    IMessageService messageService,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    UserManager<User> userManager) : IProjectService
{
    public async Task<Optional<IEnumerable<Project>>> GetProjectsAsync() => await projectRepository.GetProjectsAsync();

    public async Task<Optional<Project>> GetProjectAsync(int id)
    {
        // return await projectRepository.GetProjectAsync(id);
        var projectOptional = await projectRepository.GetProjectAsync(id);
        if (projectOptional.IsFailure)
        {
            return projectOptional;
        }
        
        var project = projectOptional.GetValueOrThrow();
        var userId = project.Owner.Id;
        await messageService.SendMessageAsync(
            userId, 
            MessageTypes.LabelersCountChanged, 
            project.ProjectLabelers.Count);
        
        return projectOptional;
    }

    public async Task<Optional<Project>> AddProjectAsync(ProjectRequest projectRequest)
    {
        var owner = await userManager.FindByIdAsync(projectRequest.OwnerId);

        if (owner == null)
        {
            return Optional<Project>.Failure("Owner not found.");
        }

        var project = new Project
        {
            Name = projectRequest.Name,
            Description = projectRequest.Description,
            Owner = owner,
            CreationDate = DateOnly.FromDateTime(DateTime.Today)
        };

        // return await projectRepository.AddProjectAsync(project);
        var projectOptional = await projectRepository.AddProjectAsync(project);
        if (projectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                projectRequest.OwnerId,
                "Failed to create project");
            return projectOptional;
        }
        else 
        {
            await messageService.SendSuccessAsync(
                projectRequest.OwnerId,
                $"Project \"{projectRequest.Name}\" created successfully");
            return projectOptional;
        } 
    } 

    public async Task<Optional<Project>> UpdateProjectAsync(int projectId, ProjectRequest projectRequest)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);

        if (projectOptional.IsFailure)
        {
            return projectOptional;
        }

        var project = projectOptional.GetValueOrThrow();

        var owner = await userManager.FindByIdAsync(projectRequest.OwnerId);
        if (owner == null)
        {
            return Optional<Project>.Failure("Owner not found.");
        }

        project.Name = projectRequest.Name;
        project.Description = projectRequest.Description;
        project.Owner = owner;
        project.ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        project.EndDate = projectRequest.Finished ? DateOnly.FromDateTime(DateTime.Today) : project.EndDate;

        // return await projectRepository.UpdateProjectAsync(project);
        var result = await projectRepository.UpdateProjectAsync(project);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                projectRequest.OwnerId,
                "Failed to update project");
            return result;
        }
        else 
        {
            await messageService.SendSuccessAsync(
                projectRequest.OwnerId,
                "Project updated successfully");
            return result;
        }
    }

    public async Task<Optional<bool>> AddLabelerToProjectAsync(LabelerAssignmentDto labelerAssignmentDto)
    {
        var projectByCodeOpt = await projectRepository.GetProjectByAccessCodeAsync(labelerAssignmentDto.AccessCode);

        var labelerOpt = await userManager.FindByIdAsync(labelerAssignmentDto.LabelerId);

        if (labelerOpt == null)
        {
            return Optional<bool>.Failure("No labeler found!");
        }

        if (projectByCodeOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                labelerAssignmentDto.LabelerId,
                "No project found!");
            return Optional<bool>.Failure("No project found!");
        }

        var project = projectByCodeOpt.GetValueOrThrow();

        project.ProjectLabelers.Add(labelerOpt);

        await projectRepository.UpdateProjectAsync(project);

        await messageService.SendSuccessAsync(
            labelerAssignmentDto.LabelerId,
            "Labeler added to project successfully");

        await messageService.SendInfoAsync(
            project.Owner.Id,
            $"Labeler \"{labelerOpt.UserName}\" joined the project \"{project.Name}\"");

        await messageService.SendMessageAsync(
            project.Owner.Id, 
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
        if (projectOpt.IsSuccess)
        {
            var project = projectOpt.GetValueOrThrow();
            await messageService.SendInfoAsync(
                project.Owner.Id,
                $"Project \"{project.Name}\" deleted successfully");
            await projectRepository.DeleteProjectAsync(project);
        }
    }

    public async Task<Optional<bool>> UnassignLabelersFromProjectAsync(int projectId)
    {
        var projectOpt = await projectRepository.GetProjectAsync(projectId);
        if (projectOpt.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }

        var project = projectOpt.GetValueOrThrow();
        var assignments = project.Subjects
            .SelectMany(s => s.SubjectVideoGroupAssignments)
            .ToList();

        foreach (var assignment in assignments)
        {
            assignment.Labelers.Clear();
            await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(assignment);
        }

        await messageService.SendSuccessAsync(
            project.Owner.Id,
            "Labelers unassigned from project successfully");
        return Optional<bool>.Success(true);
    }

    public async Task<Optional<bool>> DistributeLabelersEquallyAsync(int projectId)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);
        if (projectOptional.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }

        var projectOwnerId = projectOptional.GetValueOrThrow().Owner.Id;

        var unassignedLabelersResult = await GetUnassignedLabelersOfProjectAsync(projectId);
        if (unassignedLabelersResult.IsFailure)
        {
            await messageService.SendErrorAsync(
                projectOwnerId,
                "No unassigned labelers found!");
            return Optional<bool>.Failure("No unassigned labelers found!");
        }

        var unassignedLabelers = unassignedLabelersResult.GetValueOrThrow().ToList();

        // return await AssignEquallyAsync(projectId, unassignedLabelers);
        var result = await AssignEquallyAsync(projectId, unassignedLabelers);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                projectOwnerId,
                "Failed to assign labelers");
            return result;
        }
        else 
        {
            await messageService.SendSuccessAsync(
                projectOwnerId,
                "Labelers assigned successfully");
            return result;
        }
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
        return Optional<IEnumerable<User>>.Success(project.ProjectLabelers);
    }
}