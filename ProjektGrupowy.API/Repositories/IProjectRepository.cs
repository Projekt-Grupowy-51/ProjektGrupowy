using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IProjectRepository
{
    Task<Optional<IEnumerable<Project>>> GetProjectsAsync();
    Task<Optional<Project>> GetProjectAsync(int id);
    Task<Optional<Project>> AddProjectAsync(Project project);
    Task<Optional<Project>> UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Project project);

    Task<Optional<Project>> GetProjectByAccessCodeAsync(string code);

    Task<Optional<Dictionary<int, int>>> GetLabelerCountForAssignments(int projectId);

    Task<IDbContextTransaction> BeginTransactionAsync();
}