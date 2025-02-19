using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Enums;
using System.Security.Claims;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class ProjectController(IProjectService projectService, ISubjectService subjectService, IVideoGroupService videoGroupService, ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        if (!User.IsInRole(RoleEnum.Scientist.ToString()) && !User.IsInRole(RoleEnum.Admin.ToString()))
        {
            return Forbid();
        }

        var projects = await projectService.GetProjectsAsync();

        if (!projects.IsSuccess)
            return NotFound(projects.GetErrorOrThrow());

        if (User.IsInRole(RoleEnum.Scientist.ToString()))
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("User identity not found.");
            }

            var filteredProjects = projects.GetValueOrThrow().Where(x => x.Scientist.User.Id == userId);

            return Ok(mapper.Map<IEnumerable<ProjectResponse>>(filteredProjects));
        }

        return Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectAsync(int id)
    {
        var project = await projectService.GetProjectAsync(id);
        return project.IsSuccess
            ? Ok(mapper.Map<ProjectResponse>(project.GetValueOrThrow()))
            : NotFound(project.GetErrorOrThrow());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProjectAsync(int id, ProjectRequest projectRequest)
    {
        var p = await projectService.UpdateProjectAsync(id, projectRequest);

        return p.IsSuccess
            ? NoContent()
            : BadRequest(p.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> PostProject(ProjectRequest projectRequest)
    {
        var p = await projectService.AddProjectAsync(projectRequest);

        return p.IsSuccess
            ? CreatedAtAction("GetProject", new { id = p.GetValueOrThrow().Id },
                mapper.Map<ProjectResponse>(p.GetValueOrThrow()))
            : BadRequest(p.GetErrorOrThrow());
    }

    [HttpPost("assignment")]
    public async Task<IActionResult> AssignLabelerToGroupAssignment(LabelerAssignmentDto laveAssignmentDto)
    {
        var result = await projectService.AddLabelerToProjectAsync(laveAssignmentDto);
        return result.IsSuccess
            ? Ok()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await projectService.DeleteProjectAsync(id);

        return NoContent();
    }

    [HttpGet("{projectId:int}/Subjects")]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsByProjectAsync(int projectId)
    {
        var subjects = await subjectService.GetSubjectsByProjectAsync(projectId);
        return subjects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjects.GetValueOrThrow()))
            : NotFound(subjects.GetErrorOrThrow());
    }

    [HttpGet("{projectId:int}/VideoGroups")]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsByProjectAsync(int projectId)
    {
        var videoGroups = await videoGroupService.GetVideoGroupsByProjectAsync(projectId);
        return videoGroups.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroups.GetValueOrThrow()))
            : NotFound(videoGroups.GetErrorOrThrow());
    }

    [HttpGet("{projectId:int}/SubjectVideoGroupAssignments")]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        var subjectVideoGroupAssignments =
            await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId);

        return subjectVideoGroupAssignments.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(subjectVideoGroupAssignments
                .GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignments.GetErrorOrThrow());
    }
}