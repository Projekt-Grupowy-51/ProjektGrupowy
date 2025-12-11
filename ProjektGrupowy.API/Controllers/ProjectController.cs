using AutoMapper;
using MediatR;
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
using ProjektGrupowy.Application.Features.ProjectReport.Queries.GetReports;
using ProjektGrupowy.Application.Features.Projects.Commands.AddLabelerToProject;
using ProjektGrupowy.Application.Features.Projects.Commands.AddProject;
using ProjektGrupowy.Application.Features.Projects.Commands.DeleteProject;
using ProjektGrupowy.Application.Features.Projects.Commands.DistributeLabelersEqually;
using ProjektGrupowy.Application.Features.Projects.Commands.UnassignLabelersFromProject;
using ProjektGrupowy.Application.Features.Projects.Commands.UpdateProject;
using ProjektGrupowy.Application.Features.Projects.Queries.GetLabelersByProject;
using ProjektGrupowy.Application.Features.Projects.Queries.GetProject;
using ProjektGrupowy.Application.Features.Projects.Queries.GetProjects;
using ProjektGrupowy.Application.Features.Projects.Queries.GetProjectStats;
using ProjektGrupowy.Application.Features.Projects.Queries.GetUnassignedLabelersOfProject;
using ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjectsByProject;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentsByProject;
using ProjektGrupowy.Application.Features.VideoGroups.Queries.GetVideoGroupsByProject;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing projects. Handles operations such as retrieving, creating, updating, deleting projects, and managing related entities like subjects, video groups, and labelers.
/// </summary>
[Route("api/projects")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class ProjectController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all projects.
    /// </summary>
    /// <returns>>A collection of projects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        var query = new GetProjectsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<ProjectResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetProjectQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<ProjectResponse>(result.Value);
        return Ok(response);
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
        var query = new GetReportsQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<GeneratedReportResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetUnassignedLabelersOfProjectQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<LabelerResponse>>(result.Value);
        return Ok(response);
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
        var command = new UpdateProjectCommand(
            id,
            projectRequest.Name,
            projectRequest.Description,
            projectRequest.Finished,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
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
        var command = new AddProjectCommand(
            projectRequest.Name,
            projectRequest.Description,
            projectRequest.Finished,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<ProjectResponse>(result.Value);
        return CreatedAtAction("GetProject", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Assign a labeler to a group assignment within a project.
    /// </summary>
    /// <param name="labelerAssignmentDto">The DTO containing the labeler assignment details.</param>
    /// <returns>Ok if successful, or BadRequest if the assignment fails.</returns>
    [HttpPost("join")]
    public async Task<IActionResult> AssignLabelerToGroupAssignment(LabelerAssignmentDto labelerAssignmentDto)
    {
        var command = new AddLabelerToProjectCommand(
            labelerAssignmentDto.AccessCode,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Errors);
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
        var command = new UnassignLabelersFromProjectCommand(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Errors);
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
        var command = new DistributeLabelersEquallyCommand(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Errors);
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
        var command = new DeleteProjectCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
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
        var query = new GetSubjectsByProjectQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<SubjectResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetVideoGroupsByProjectQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<VideoGroupResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetSubjectVideoGroupAssignmentsByProjectQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetLabelersByProjectQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<LabelerResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetProjectStatsQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        return !result.IsSuccess ? (ActionResult<ProjectStatsResponse>)NotFound(result.Errors) : (ActionResult<ProjectStatsResponse>)Ok(result.Value);
    }
}
