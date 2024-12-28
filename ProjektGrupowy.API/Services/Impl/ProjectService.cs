using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectService(IProjectRepository projectRepository, IScientistRepository scientistRepository) : IProjectService
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
            Scientist = scientistOptional.GetValueOrThrow()
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

        return await projectRepository.UpdateProjectAsync(project);
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