using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectService(
    IProjectRepository projectRepository,
    IScientistRepository scientistRepository,
    IProjectAccessCodeRepository projectAccessCodeRepository,
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
        var projectId = labelerAssignmentDto.ProjectId;
        var projectByIdOpt = await projectRepository.GetProjectAsync(projectId);
        var labelerId = labelerAssignmentDto.LabelerId;

        if (projectByIdOpt.IsFailure) 
        {
            return Optional<bool>.Failure("No project found!");
        }

        var code = labelerAssignmentDto.AccessCode;
        var projectByCodeOpt = await projectAccessCodeRepository
            .GetAccessCodeByCodeAsync(code);

        if (projectByCodeOpt.IsFailure)
        {
            return Optional<bool>.Failure("No project found!");
        }

        // Check if project id and access code project id match
        if (projectByIdOpt.GetValueOrThrow().Id != projectByCodeOpt.GetValueOrThrow().Project.Id)
        {
            return Optional<bool>.Failure("Project id and access code project id do not match!");
        }

        var project = projectByIdOpt.GetValueOrThrow();

        // Round robin
        var videoGroupAssignment = project
            .Subjects
            .SelectMany(s => s.SubjectVideoGroupAssignments)
            .MinBy(sv => sv.Labelers.Count);

        if (videoGroupAssignment is null)
        {
            return Optional<bool>.Failure("No video group assignments found!");
        }

        var labelerOpt = await labelerRepository.GetLabelerAsync(labelerId);
        if (labelerOpt.IsFailure)
        {
            return Optional<bool>.Failure("No labeler found!");
        }

        var labeler = labelerOpt.GetValueOrThrow();

        videoGroupAssignment.Labelers.Add(labeler);

        await projectRepository.UpdateProjectAsync(project);

        return Optional<bool>.Success(true);
    }

    public async Task DeleteProjectAsync(int id)
    {
        var project = await projectRepository.GetProjectAsync(id);
        if (project.IsSuccess)
        {
            await projectRepository.DeleteProjectAsync(project.GetValueOrThrow());
        }
    }
}