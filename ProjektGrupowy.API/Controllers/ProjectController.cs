using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
public class ProjectController(IProjectService projectService, IMapper mapper) : ControllerBase
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

    [HttpGet("scientist/{scientistId:int}")]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsOfScientistAsync(int scientistId)
    {
        var projects = await projectService.GetProjectsOfScientist(scientistId);
        return projects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects.GetValueOrThrow()))
            : NotFound(projects.GetErrorOrThrow());
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await projectService.DeleteProjectAsync(id);

        return NoContent();
    }
}