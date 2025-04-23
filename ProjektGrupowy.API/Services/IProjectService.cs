using Microsoft.CodeAnalysis.Options;
using ProjektGrupowy.API.DTOs.LabelerAssignment;
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
    Task<Optional<bool>> AddLabelerToProjectAsync(LabelerAssignmentDto labelerAssignmentDto);
    Task<Optional<IEnumerable<User>>> GetUnassignedLabelersOfProjectAsync(int id);
    Task DeleteProjectAsync(int id);
    Task<Optional<bool>> UnassignLabelersFromProjectAsync(int projectId);
    Task<Optional<bool>> DistributeLabelersEquallyAsync(int projectId);
    Task<Optional<IEnumerable<User>>> GetLabelersByProjectAsync(int projectId);
}