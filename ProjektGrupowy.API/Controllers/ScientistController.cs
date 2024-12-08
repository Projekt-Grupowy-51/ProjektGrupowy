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
        var scientist = mapper.Map<Scientist>(scientistRequest);
        var result = await scientistService.AddScientistAsync(scientist);
        return result.IsSuccess
            ? CreatedAtAction("GetScientist", new { id = result.GetValueOrThrow().Id }, result.GetValueOrThrow())
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutScientistAsync(int id, ScientistRequest scientistRequest)
    {
        var existingScientist = await scientistService.GetScientistAsync(id);

        if (existingScientist.IsFailure)
        {
            return BadRequest(existingScientist.GetErrorOrThrow());
        }

        var scientist = mapper.Map<Scientist>(scientistRequest);

        existingScientist.GetValueOrThrow().FirstName = scientist.FirstName;
        existingScientist.GetValueOrThrow().LastName = scientist.LastName;
        existingScientist.GetValueOrThrow().Description = scientist.Description;
        existingScientist.GetValueOrThrow().Title = scientist.Title;

        var s = await scientistService.UpdateScientistAsync(existingScientist.GetValueOrThrow());

        return s.IsSuccess
            ? NoContent()
            : BadRequest(s.GetErrorOrThrow());
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
        var project = mapper.Map<Project>(projectRequest);
        var result = await scientistService.AddProjectToScientist(scientistId, project);
        return result.IsSuccess
            ? CreatedAtAction("GetProject", new { id = result.GetValueOrThrow().Id }, result.GetValueOrThrow())
            : BadRequest(result.GetErrorOrThrow());
    }
}
