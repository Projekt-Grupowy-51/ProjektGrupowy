using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using ProjektGrupowy.API.Utils.Extensions;  // Add this using directive
using System.Security.Claims;
using ProjektGrupowy.API.DTOs.Labeler;
using Hangfire;
using ProjektGrupowy.API.Services.Background;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class ProjectController(
    IProjectService projectService, 
    ISubjectService subjectService, 
    IVideoGroupService videoGroupService,
    IProjectReportService projectReportService,
    ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, 
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        var projects = await projectService.GetProjectsAsync();
        return projects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow()))
            : NotFound(projects.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectAsync(int id)
    {
        var project = await projectService.GetProjectAsync(id);
        return project.IsSuccess
            ? Ok(mapper.Map<ProjectResponse>(project.GetValueOrThrow()))
            : NotFound(project.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/unassigned-labelers")]
    public async Task<ActionResult<ProjectResponse>> GetUnassignedLabelers(int id)
    {
        var result = await projectService.GetUnassignedLabelersOfProjectAsync(id);
        return result.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(result.GetValueOrThrow()))
            : NotFound(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProjectAsync(int id, ProjectRequest projectRequest)
    {
        projectRequest.OwnerId = User.GetUserId();

        var updateResult = await projectService.UpdateProjectAsync(id, projectRequest);
        return updateResult.IsSuccess
            ? NoContent()
            : BadRequest(updateResult.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> PostProject(ProjectRequest projectRequest)
    {
        projectRequest.OwnerId = User.GetUserId();

        var projectResult = await projectService.AddProjectAsync(projectRequest);
        if (!projectResult.IsSuccess)
        {
            return BadRequest(projectResult.GetErrorOrThrow());
        }

        var createdProject = projectResult.GetValueOrThrow();

        return CreatedAtAction("GetProject", new { id = createdProject.Id },
            mapper.Map<ProjectResponse>(createdProject));
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/generate-report")]
    public IActionResult GenerateReport(int projectId)
    {
        var result = BackgroundJob.Enqueue<IReportGenerator>(g => g.GenerateAsync(projectId));
        
        return result is null
            ? BadRequest("Failed to enqueue report generation job.")
            : Accepted("Report generation job has been enqueued.");
    }

    [HttpPost("join")]
    public async Task<IActionResult> AssignLabelerToGroupAssignment(LabelerAssignmentDto laveAssignmentDto)
    {
        laveAssignmentDto.LabelerId = User.GetUserId();

        var result = await projectService.AddLabelerToProjectAsync(laveAssignmentDto);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/unassign-all")]
    public async Task<IActionResult> UnassignAllLabelers(int projectId)
    {
        var result = await projectService.UnassignLabelersFromProjectAsync(projectId);
        return result.IsSuccess
            ? Ok()
            : BadRequest(result.GetErrorOrThrow());
    }


    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/distribute")]
    public async Task<IActionResult> DistributeLabelersEqually(int projectId)
    {
        var result = await projectService.DistributeLabelersEquallyAsync(projectId);
        return result.IsSuccess 
            ? Ok(result.GetValueOrThrow())
            : NotFound(result.GetErrorOrThrow());
    }

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

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/Subjects")]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsByProjectAsync(int projectId)
    {
        var subjectsResult = await subjectService.GetSubjectsByProjectAsync(projectId);
        return subjectsResult.IsSuccess 
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjectsResult.GetValueOrThrow()))
            : NotFound(subjectsResult.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/VideoGroups")]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsByProjectAsync(int projectId)
    {
        var videoGroupsResult = await videoGroupService.GetVideoGroupsByProjectAsync(projectId);
        return videoGroupsResult.IsSuccess 
            ? Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroupsResult.GetValueOrThrow()))
            : NotFound(videoGroupsResult.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/SubjectVideoGroupAssignments")]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        var assignmentsResult = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId);
        return assignmentsResult.IsSuccess 
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(assignmentsResult.GetValueOrThrow()))
            : NotFound(assignmentsResult.GetErrorOrThrow());
    }
    
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{projectId:int}/Labelers")]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetLabelersByProjectAsync(int projectId)
    {
        var labelersResult = await projectService.GetLabelersByProjectAsync(projectId);
        return labelersResult.IsSuccess 
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(labelersResult.GetValueOrThrow()))
            : NotFound(labelersResult.GetErrorOrThrow());
    }
}