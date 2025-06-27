using ProjektGrupowy.Application.DTOs.LabelerAssignment;
using ProjektGrupowy.Application.DTOs.Project;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface IProjectService
{
    Task<Optional<IEnumerable<Project>>> GetProjectsAsync();
    Task<Optional<Project>> GetProjectAsync(int id, string? userId = null, bool? isAdmin = null);
    Task<Optional<Project>> AddProjectAsync(ProjectRequest projectRequest);
    Task<Optional<Project>> UpdateProjectAsync(int projectId, ProjectRequest projectRequest);
    Task<Optional<Project>> UpdateProjectAsync(Project project);
    Task<Optional<bool>> AddLabelerToProjectAsync(LabelerAssignmentDto labelerAssignmentDto);
    Task<Optional<IEnumerable<User>>> GetUnassignedLabelersOfProjectAsync(int id);
    Task DeleteProjectAsync(int id);
    Task<Optional<bool>> UnassignLabelersFromProjectAsync(int projectId);
    Task<Optional<bool>> DistributeLabelersEquallyAsync(int projectId);
    Task<Optional<IEnumerable<User>>> GetLabelersByProjectAsync(int projectId);
}