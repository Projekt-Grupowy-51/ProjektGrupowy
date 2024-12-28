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
        var result = await scientistService.AddScientistAsync(scientistRequest);

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
        var result = await scientistService.UpdateScientistAsync(id, scientistRequest);

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
}
