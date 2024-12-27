using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Scientist;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
public class ScientistController(IScientistService scientistService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScientistResponse>>> GetScientistsAsync()
    {
        var scientists = await scientistService.GetScientistsAsync();
        return scientists.IsSuccess
            ? Ok(mapper.Map<IEnumerable<ScientistResponse>>(scientists.GetValueOrThrow()))
            : NotFound(scientists.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScientistResponse>> GetScientistAsync(int id)
    {
        var scientist = await scientistService.GetScientistAsync(id);
        return scientist.IsSuccess
            ? Ok(mapper.Map<ScientistResponse>(scientist.GetValueOrThrow()))
            : NotFound(scientist.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<ScientistResponse>> AddScientistAsync(ScientistRequest scientistRequest)
    {
        var scientist = new Scientist
        {
            FirstName = scientistRequest.FirstName,
            LastName = scientistRequest.LastName,
            Title = scientistRequest.Title,
            Description = scientistRequest.Description
        };
        var result = await scientistService.AddScientistAsync(scientist);

        if (result.IsSuccess)
        {
            var createdScientist = result.GetValueOrThrow();

            var scientistResponse = mapper.Map<ScientistResponse>(createdScientist);

            return CreatedAtAction("GetScientist", new { id = createdScientist.Id }, scientistResponse);
        }

        return BadRequest(result.GetErrorOrThrow());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutScientistAsync(int id, ScientistRequest scientistRequest)
    {
        var existingScientist = await scientistService.GetScientistAsync(id);

        if (existingScientist.IsFailure)
        {
            return BadRequest(existingScientist.GetErrorOrThrow());
        }

        var scientist = new Scientist
        {
            FirstName = scientistRequest.FirstName,
            LastName = scientistRequest.LastName,
            Title = scientistRequest.Title,
            Description = scientistRequest.Description
        };

        var updatedScientist = existingScientist.GetValueOrThrow();
        updatedScientist.FirstName = scientistRequest.FirstName;
        updatedScientist.LastName = scientistRequest.LastName;
        updatedScientist.Title = scientistRequest.Title;
        updatedScientist.Description = scientistRequest.Description;

        var result = await scientistService.UpdateScientistAsync(updatedScientist);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteScientistAsync(int id)
    {
        await scientistService.DeleteScientistAsync(id);
        return NoContent();
    }

    [HttpPost("{scientistId:int}/project")]
    public async Task<ActionResult> AddProjectToScientist(int scientistId, ProjectRequest projectRequest)
    {
        var project = new Project
        {
            Name = projectRequest.Name,
            Description = projectRequest.Description
        };
        var result = await scientistService.AddProjectToScientist(scientistId, project);
        return result.IsSuccess
            ? CreatedAtAction("GetProject", new { id = result.GetValueOrThrow().Id })
            : BadRequest(result.GetErrorOrThrow());
    }
}
