using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IProjectService
{
    Task<Optional<IEnumerable<Project>>> GetProjectsAsync();
    Task<Optional<Project>> GetProjectAsync(int id);
    Task<Optional<Project>> AddProjectAsync(Project project);
    Task<Optional<Project>> UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(int id);
}