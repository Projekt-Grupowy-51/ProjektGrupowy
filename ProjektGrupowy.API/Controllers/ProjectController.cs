using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.Labeler;
using ProjektGrupowy.Application.DTOs.LabelerAssignment;
using ProjektGrupowy.Application.DTOs.Project;
using ProjektGrupowy.Application.DTOs.ProjectReport;
using ProjektGrupowy.Application.DTOs.Subject;
using ProjektGrupowy.Application.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.Application.DTOs.VideoGroup;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Application.Utils.Extensions;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing projects. Handles operations such as retrieving, creating, updating, deleting projects, and managing related entities like subjects, video groups, and labelers.
/// </summary>
/// <param name="projectService"></param>
/// <param name="subjectService"></param>
/// <param name="videoGroupService"></param>
/// <param name="projectReportService"></param>
/// <param name="messageService"></param>
/// <param name="subjectVideoGroupAssignmentService"></param>
/// <param name="mapper"></param>
[Route("api/projects")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class ProjectController(
    IProjectService projectService,
    ISubjectService subjectService,
    IVideoGroupService videoGroupService,
    IProjectReportService projectReportService,
    IMessageService messageService,
    ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all projects.
    /// </summary>
    /// <returns>>A collection of projects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        try
        {
            var projects = await projectService.GetProjectsAsync();
            if (!projects.IsSuccess)
                return NotFound(projects.GetErrorOrThrow());

            var result = mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get a specific project by its ID.
    /// </summary>
    /// <param name="id">The ID of the project.</param>
    /// <returns>The project with the specified ID, or NotFound if it does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectAsync(int id)
    {
        var project = await projectService.GetProjectAsync(id);
        return project.IsSuccess
            ? Ok(mapper.Map<ProjectResponse>(project.GetValueOrThrow()))
            : NotFound(project.GetErrorOrThrow());
    }

    /// <summary>
    /// Get all reports associated with a specific project.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A collection of project reports, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/reports")]
    public async Task<ActionResult<IEnumerable<GeneratedReportResponse>>> GetProjectReports(int id)
    {
        var result = await projectReportService.GetReportsAsync(id);
        return result.IsSuccess
            ? Ok(mapper.Map<IEnumerable<GeneratedReportResponse>>(result.GetValueOrThrow()))
            : NotFound(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Get all labelers not assigned to a specific project.
    /// </summary>
    /// <param name="id">The ID of the project.</param>
    /// <returns>A collection of unassigned labelers, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/unassigned-labelers")]
    public async Task<ActionResult<ProjectResponse>> GetUnassignedLabelers(int id)
    {
        var result = await projectService.GetUnassignedLabelersOfProjectAsync(id);
        return result.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(result.GetValueOrThrow()))
            : NotFound(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Update an existing project.
    /// </summary>
    /// <param name="id">The ID of the project to be updated.</param>
    /// <param name="projectRequest">The request containing the updated details of the project.</param>
    /// <returns>NoContent if successful, or BadRequest if the update fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProjectAsync(int id, ProjectRequest projectRequest)
    {
        var updateResult = await projectService.UpdateProjectAsync(id, projectRequest);
        return updateResult.IsSuccess
            ? NoContent()
            : BadRequest(updateResult.GetErrorOrThrow());
    }

    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="projectRequest">The request containing the details of the project to be created.</param>
    /// <returns>The created project, or BadRequest if the creation fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> PostProject(ProjectRequest projectRequest)
    {
        var projectResult = await projectService.AddProjectAsync(projectRequest);
        if (!projectResult.IsSuccess)
        {
            await messageService.SendErrorAsync(
                User.GetUserId(),
                $"Failed to create project: {projectResult.GetErrorOrThrow()}");
            return BadRequest(projectResult.GetErrorOrThrow());
        }

        var createdProject = projectResult.GetValueOrThrow();

        return CreatedAtAction("GetProject", new { id = createdProject.Id },
            mapper.Map<ProjectResponse>(createdProject));
    }

    /// <summary>
    /// Assign a labeler to a group assignment within a project.
    /// </summary>
    /// <param name="laveAssignmentDto">The DTO containing the labeler assignment details.</param>
    /// <returns>Ok if successful, or BadRequest if the assignment fails.</returns>
    [HttpPost("join")]
    public async Task<IActionResult> AssignLabelerToGroupAssignment(LabelerAssignmentDto laveAssignmentDto)
    {
        var result = await projectService.AddLabelerToProjectAsync(laveAssignmentDto);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Unassign all labelers as a single operation from a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project from which to unassign all labelers.</param>
    /// <returns>Ok if successful, or BadRequest if the operation fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/unassign-all")]
    public async Task<IActionResult> UnassignAllLabelers(int projectId)
    {
        var result = await projectService.UnassignLabelersFromProjectAsync(projectId);
        return result.IsSuccess
            ? Ok()
            : BadRequest(result.GetErrorOrThrow());
    }


    /// <summary>
    /// Distribute labelers equally (round-robin) among all subjects in a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project for which to distribute labelers.</param>
    /// <returns>Ok if successful, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/distribute")]
    public async Task<IActionResult> DistributeLabelersEqually(int projectId)
    {
        var result = await projectService.DistributeLabelersEquallyAsync(projectId);
        return result.IsSuccess
            ? Ok(result.GetValueOrThrow())
            : NotFound(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Delete a project by its ID.
    /// </summary>
    /// <param name="id">The ID of the project to be deleted.</param>
    /// <returns>NoContent if successful, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await projectService.GetProjectAsync(id);

        if (project.IsFailure)
        {
            return NotFound(project.GetErrorOrThrow());
        }

        await projectService.DeleteProjectAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get all subjects associated with a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of subjects for the specified project, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/subjects")]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsByProjectAsync(int projectId)
    {
        var subjectsResult = await subjectService.GetSubjectsByProjectAsync(projectId);
        return subjectsResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjectsResult.GetValueOrThrow()))
            : NotFound(subjectsResult.GetErrorOrThrow());
    }

    /// <summary>
    /// Get all video groups associated with a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of video groups for the specified project, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/video-groups")]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsByProjectAsync(int projectId)
    {
        var videoGroupsResult = await videoGroupService.GetVideoGroupsByProjectAsync(projectId);
        return videoGroupsResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroupsResult.GetValueOrThrow()))
            : NotFound(videoGroupsResult.GetErrorOrThrow());
    }

    /// <summary>
    /// Get all subject-video group assignments associated with a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of subject-video group assignments for the specified project, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/subject-video-group-assignments")]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        var assignmentsResult = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId);
        return assignmentsResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(assignmentsResult.GetValueOrThrow()))
            : NotFound(assignmentsResult.GetErrorOrThrow());
    }

    /// <summary>
    /// Get all labelers associated with a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of labelers for the specified project, or NotFound if the project does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/labelers")]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetLabelersByProjectAsync(int projectId)
    {
        var labelersResult = await projectService.GetLabelersByProjectAsync(projectId);
        return labelersResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(labelersResult.GetValueOrThrow()))
            : NotFound(labelersResult.GetErrorOrThrow());
    }

    /// <summary>
    /// Get useful statistics for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns></returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/stats")]
    public async Task<ActionResult<ProjectStatsResponse>> GetProjectStatsAsync(int projectId)
    {
        var statsResult = await projectService.GetProjectStatsAsync(projectId);
        return statsResult.IsSuccess
            ? Ok(statsResult.GetValueOrThrow())
            : NotFound(statsResult.GetErrorOrThrow());
    }
}