using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectService(IProjectRepository projectRepository) : IProjectService
{
    public async Task<Optional<IEnumerable<Project>>> GetProjectsAsync() => await projectRepository.GetProjectsAsync();

    public async Task<Optional<Project>> GetProjectAsync(int id) => await projectRepository.GetProjectAsync(id);

    public async Task<Optional<Project>> AddProjectAsync(Project project) =>
        await projectRepository.AddProjectAsync(project);

    public async Task<Optional<Project>> UpdateProjectAsync(Project project) =>
        await projectRepository.UpdateProjectAsync(project);

    public async Task DeleteProjectAsync(int id)
    {
        var project = await projectRepository.GetProjectAsync(id);
        if (project.IsSuccess)
        {
            await projectRepository.DeleteProjectAsync(project.GetValueOrThrow());
        }
    }
}