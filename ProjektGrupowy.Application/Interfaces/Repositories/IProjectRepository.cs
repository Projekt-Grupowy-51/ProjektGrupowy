using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetProjectsAsync(string userId, bool isAdmin);
    Task<Project> GetProjectAsync(int id, string userId, bool isAdmin);
    Task AddProjectAsync(Project project);
    void DeleteProject(Project project);
    Task<Project> GetProjectByAccessCodeAsync(string code);
    Task<Dictionary<int, int>> GetLabelerCountForAssignments(int projectId, string userId, bool isAdmin);
}