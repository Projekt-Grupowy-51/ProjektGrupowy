using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController(IProjectService projectService, IMapper mapper) : ControllerBase
{
    // GET: api/Project
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
    {
        var projects = await projectService.GetProjectsAsync();
        return Ok(mapper.Map<IEnumerable<ProjectResponse>>(projects));
    }

    // GET: api/Project/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectAsync(int id)
    {
        var project = await projectService.GetProjectAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<ProjectResponse>(project));
    }

    // PUT: api/Project/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProjectAsync(int id, ProjectRequest projectRequest)
    {
        var existingProject = await projectService.GetProjectAsync(id);

        if (existingProject == null)
        {
            return BadRequest();
        }

        var project = mapper.Map<Project>(projectRequest);

        var p = await projectService.UpdateProjectAsync(project);

        return Ok(mapper.Map<ProjectResponse>(p));
    }

    // POST: api/Project
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> PostProject(ProjectRequest projectRequest)
    {
        var project = mapper.Map<Project>(projectRequest);

        var p = await projectService.AddProjectAsync(project);

        return CreatedAtAction("GetProject", new { id = project.Id }, mapper.Map<ProjectResponse>(p));
    }

    // DELETE: api/Project/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await projectService.DeleteProjectAsync(id);

        return NoContent();
    }
}