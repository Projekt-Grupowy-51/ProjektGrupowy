using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.LabelerAssignment;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
public class ProjectController(IProjectService projectService, ISubjectService subjectService, IVideoGroupService videoGroupService, ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        var projects = await projectService.GetProjectsAsync();
        return projects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow()))
            : NotFound(projects.GetErrorOrThrow());
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