using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectService(
    IProjectRepository projectRepository,
    IScientistRepository scientistRepository,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    ILabelerRepository labelerRepository)
    : IProjectService
{
    public async Task<Optional<IEnumerable<Project>>> GetProjectsAsync() => await projectRepository.GetProjectsAsync();

    public async Task<Optional<Project>> GetProjectAsync(int id) => await projectRepository.GetProjectAsync(id);

    public async Task<Optional<Project>> AddProjectAsync(ProjectRequest projectRequest)
    {
        var scientistOptional = await scientistRepository.GetScientistAsync(projectRequest.ScientistId);

        if (scientistOptional.IsFailure)
        {
            return Optional<Project>.Failure("No scientist found!");
        }

        var project = new Project
        {
            Name = projectRequest.Name,
            Description = projectRequest.Description,
            Scientist = scientistOptional.GetValueOrThrow(),
            CreationDate = DateOnly.FromDateTime(DateTime.Today)
        };

        return await projectRepository.AddProjectAsync(project);
    }

    public async Task<Optional<Project>> UpdateProjectAsync(int projectId, ProjectRequest projectRequest)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);

        if (projectOptional.IsFailure)
        {
            return projectOptional;
        }

        var project = projectOptional.GetValueOrThrow();

        var scientistOptional = await scientistRepository.GetScientistAsync(projectRequest.ScientistId);

        if (scientistOptional.IsFailure)
        {
            return Optional<Project>.Failure("No scientist found!");
        }

        project.Name = projectRequest.Name;
        project.Description = projectRequest.Description;
        project.Scientist = scientistOptional.GetValueOrThrow();
        project.ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        project.EndDate = projectRequest.Finished ? DateOnly.FromDateTime(DateTime.Today) : project.EndDate;

        return await projectRepository.UpdateProjectAsync(project);
    }

    public async Task<Optional<IEnumerable<Project>>> GetProjectsOfScientist(int scientistId)
        => await projectRepository.GetProjectsOfScientist(scientistId);

    public async Task<Optional<bool>> AddLabelerToProjectAsync(LabelerAssignmentDto labelerAssignmentDto)
    {
        var projectByCodeOpt = await projectRepository.GetProjectByAccessCodeAsync(labelerAssignmentDto.AccessCode);
        var labelerByIdOpt = await labelerRepository.GetLabelerAsync(labelerAssignmentDto.LabelerId);

        if (projectByCodeOpt.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }

        if (labelerByIdOpt.IsFailure)
        {
            return Optional<bool>.Failure("No labeler found!");
        }

        var project = projectByCodeOpt.GetValueOrThrow();
        var labeler = labelerByIdOpt.GetValueOrThrow();

        labeler.ProjectLabelers.Add(project);
        project.ProjectLabelers.Add(labeler);

        await projectRepository.UpdateProjectAsync(project);
        await labelerRepository.UpdateLabelerAsync(labeler);

        return Optional<bool>.Success(true);
    }

    public async Task<Optional<IEnumerable<Labeler>>> GetUnassignedLabelersOfProjectAsync(int id)
    {
        return await labelerRepository.GetUnassignedLabelersOfProjectAsync(id);
    }

    public async Task DeleteProjectAsync(int id)
    {
        var project = await projectRepository.GetProjectAsync(id);
        if (project.IsSuccess)
        {
            await projectRepository.DeleteProjectAsync(project.GetValueOrThrow());
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

        return Optional<bool>.Success(true);
    }

    public async Task<Optional<bool>> DistributeLabelersEquallyAsync(int projectId)
    {
        var projectOptional = await projectRepository.GetProjectAsync(projectId);
        if (projectOptional.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }

        var unassignedLabelersResult = await labelerRepository.GetUnassignedLabelersOfProjectAsync(projectId);

        if (unassignedLabelersResult.IsFailure)
        {
            return Optional<bool>.Failure("No labelers found!");
        }

        var unassignedLabelers = unassignedLabelersResult.GetValueOrThrow().ToList();

        return await AssignEquallyAsync(projectId, unassignedLabelers);
    }

    private async Task<Optional<bool>> AssignEquallyAsync(int projectId, IReadOnlyCollection<Labeler> labelers)
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

    private async Task<Optional<bool>> AssignLabelerToAssignmentAsync(int assignmentId, IEnumerable<Labeler> labelers)
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
            labeler.SubjectVideoGroups.Add(assignment);

            await labelerRepository.UpdateLabelerAsync(labeler);
        }

        await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(assignment);

        return Optional<bool>.Success(true);
    }
}