using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IProjectRepository
{
    Task<Optional<IEnumerable<Project>>> GetProjectsAsync(string userId, bool isAdmin);
    Task<Optional<Project>> GetProjectAsync(int id, string userId, bool isAdmin);
    Task<Optional<Project>> AddProjectAsync(Project project);
    Task<Optional<Project>> UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Project project);

    Task<Optional<Project>> GetProjectByAccessCodeAsync(string code);

    Task<Optional<Dictionary<int, int>>> GetLabelerCountForAssignments(int projectId, string userId, bool isAdmin);

    Task<IDbContextTransaction> BeginTransactionAsync();
}