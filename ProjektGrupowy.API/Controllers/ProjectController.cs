using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjectsAsync()
        {
            var projects = await _projectService.GetProjectsAsync();
            return Ok(_mapper.Map<IEnumerable<ProjectResponse>>(projects));
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetProjectAsync(int id)
        {
            var project = await _projectService.GetProjectAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProjectResponse>(project));
        }

        // PUT: api/Project/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{name}")]
        public async Task<IActionResult> PutProjectAsync(int id, ProjectRequest projectRequest)
        {
            var existingProject = await _projectService.GetProjectAsync(id);

            if (existingProject == null)
            {
                return BadRequest();
            }

            var project = _mapper.Map<Project>(projectRequest);

            var p = await _projectService.UpdateProjectAsync(project);

            return Ok(_mapper.Map<ProjectResponse>(p));
        }

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> PostProject(ProjectRequest projectRequest)
        {
            var project = _mapper.Map<Project>(projectRequest);

            var p = await _projectService.AddProjectAsync(project);

            return CreatedAtAction("GetProject", new { id = project.Id }, _mapper.Map<ProjectResponse>(p));
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteProjectAsync(id);

            return NoContent();
        }
    }
}
