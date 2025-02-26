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
using ProjektGrupowy.API.Utils.Enums;
using System.Security.Claims;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class ProjectController(IProjectService projectService, ISubjectService subjectService, IVideoGroupService videoGroupService, ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, IScientistService scientistService, ILabelerService labelerService, IMapper mapper) : ControllerBase
{
    private (ActionResult? Error, bool IsScientist, bool IsAdmin, Scientist? Scientist) CheckGeneralAccess()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return (Unauthorized("User identity not found."), false, false, null);
        }

        bool isScientist = User.IsInRole(RoleEnum.Scientist.ToString());
        bool isAdmin = User.IsInRole(RoleEnum.Admin.ToString());

        if (!isScientist && !isAdmin)
        {
            return (Forbid(), false, false, null);
        }

        if (isScientist)
        {
            return (null, true, false, scientistService.GetScientistByUserIdAsync(userId).Result.GetValueOrThrow());
        }

        return (null, false, true, null);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var projects = await projectService.GetProjectsOfScientist(checkResult.Scientist!.Id);
            return projects.IsSuccess ? Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow())) : NotFound(projects.GetErrorOrThrow());
        }

        if (checkResult.IsAdmin)
        {
            var projects = await projectService.GetProjectsAsync();
            return projects.IsSuccess ? Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow())) : NotFound(projects.GetErrorOrThrow());
        }

        return Forbid();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectAsync(int id)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var projects = await projectService.GetProjectsOfScientist(checkResult.Scientist!.Id);

            if (projects.IsFailure)
            {
                return NotFound(projects.GetErrorOrThrow());
            }

            var project = projects.GetValueOrThrow().FirstOrDefault(p => p.Id == id);

            return project != null ? Ok(mapper.Map<ProjectResponse>(project)) : NotFound("Project not found.");
        }

        if (checkResult.IsAdmin)
        {
            var project = await projectService.GetProjectAsync(id);
            return project.IsSuccess ? Ok(mapper.Map<ProjectResponse>(project.GetValueOrThrow())) : NotFound(project.GetErrorOrThrow());
        }

        return Forbid();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProjectAsync(int id, ProjectRequest projectRequest)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var project = await projectService.GetProjectAsync(id);

            if (project.IsFailure)
            {
                return NotFound(project.GetErrorOrThrow());
            }

            if (project.GetValueOrThrow().Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid();
            }

            projectRequest.ScientistId = checkResult.Scientist!.Id;
        }

        var updateResult = await projectService.UpdateProjectAsync(id, projectRequest);
        return updateResult.IsSuccess
            ? NoContent()
            : BadRequest(updateResult.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> PostProject(ProjectRequest projectRequest)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            projectRequest.ScientistId = checkResult.Scientist!.Id;
        }

        var projectResult = await projectService.AddProjectAsync(projectRequest);
        if (!projectResult.IsSuccess)
        {
            return BadRequest(projectResult.GetErrorOrThrow());
        }

        var createdProject = projectResult.GetValueOrThrow();

        return CreatedAtAction(nameof(GetProjectAsync), new { id = createdProject.Id },
            mapper.Map<ProjectResponse>(createdProject));
    }

    [HttpPost("join")]
    public async Task<IActionResult> AssignLabelerToGroupAssignment(LabelerAssignmentDto laveAssignmentDto)
    {
        if (!User.IsInRole(RoleEnum.Labeler.ToString()))
        {
            return Forbid();
        }

        var labeler = labelerService.GetLabelerByUserIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!).Result.GetValueOrThrow();

        laveAssignmentDto.LabelerId = labeler.Id;

        var result = await projectService.AddLabelerToProjectAsync(laveAssignmentDto);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpPost("{projectId:int}/distribute")]
    public async Task<IActionResult> DistributeLabelersEqually(int projectId)
    {
        var result = await projectService.DistributeLabelersEquallyAsync(projectId);
        return result.IsSuccess 
            ? Ok(result.GetValueOrThrow())
            : NotFound(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var project = await projectService.GetProjectAsync(id);

            if (project.IsFailure)
            {
                return NotFound(project.GetErrorOrThrow());
            }

            if (project.GetValueOrThrow().Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid();
            }
        }

        await projectService.DeleteProjectAsync(id);
        return NoContent();
    }

    [HttpGet("{projectId:int}/Subjects")]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsByProjectAsync(int projectId)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var projects = await projectService.GetProjectsOfScientist(checkResult.Scientist!.Id);

            if (projects.IsFailure)
            {
                return NotFound(projects.GetErrorOrThrow());
            }

            var project = projects.GetValueOrThrow().FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }
        }

        var subjectsResult = await subjectService.GetSubjectsByProjectAsync(projectId);

        if (!subjectsResult.IsSuccess)
        {
            return NotFound(subjectsResult.GetErrorOrThrow());
        }

        return Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjectsResult.GetValueOrThrow()));
    }

    [HttpGet("{projectId:int}/VideoGroups")]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsByProjectAsync(int projectId)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var projects = await projectService.GetProjectsOfScientist(checkResult.Scientist!.Id);

            if (projects.IsFailure)
            {
                return NotFound(projects.GetErrorOrThrow());
            }

            var project = projects.GetValueOrThrow().FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }
        }

        var videoGroupsResult = await videoGroupService.GetVideoGroupsByProjectAsync(projectId);
        if (!videoGroupsResult.IsSuccess)
        {
            return NotFound(videoGroupsResult.GetErrorOrThrow());
        }

        return Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroupsResult.GetValueOrThrow()));
    }

    [HttpGet("{projectId:int}/SubjectVideoGroupAssignments")]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var projects = await projectService.GetProjectsOfScientist(checkResult.Scientist!.Id);

            if (projects.IsFailure)
            {
                return NotFound(projects.GetErrorOrThrow());
            }

            var project = projects.GetValueOrThrow().FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Project not found.");
            }
        }

        var subjectVideoGroupAssignmentsResult = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId);
        if (!subjectVideoGroupAssignmentsResult.IsSuccess)
        {
            return NotFound(subjectVideoGroupAssignmentsResult.GetErrorOrThrow());
        }

        return Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(subjectVideoGroupAssignmentsResult.GetValueOrThrow()));
    }
}