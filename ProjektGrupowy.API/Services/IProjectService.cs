using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IProjectService
{
    Task<Optional<IEnumerable<Project>>> GetProjectsAsync();
    Task<Optional<Project>> GetProjectAsync(int id);
    Task<Optional<Project>> AddProjectAsync(ProjectRequest projectRequest);
    Task<Optional<Project>> UpdateProjectAsync(int projectId, ProjectRequest projectRequest);

    Task<Optional<IEnumerable<Project>>> GetProjectsOfScientist(int scientistId);

    Task DeleteProjectAsync(int id);
}