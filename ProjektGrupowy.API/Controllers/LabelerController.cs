using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Labeler;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
public class LabelerController(ILabelerService labelerService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetLabelersAsync()
    {
        var labelers = await labelerService.GetLabelersAsync();
        return labelers.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(labelers.GetValueOrThrow()))
            : NotFound(labelers.GetErrorOrThrow());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LabelerResponse>> GetLabelerAsync(int id)
    {
        var labeler = await labelerService.GetLabelerAsync(id);
        return labeler.IsSuccess
            ? Ok(mapper.Map<LabelerResponse>(labeler.GetValueOrThrow()))
            : NotFound(labeler.GetErrorOrThrow());
    }
        
    [HttpPost]
    public async Task<ActionResult<LabelerResponse>> AddLabelerAsync(LabelerRequest labelerRequest)
    {
        var result = await labelerService.AddLabelerAsync(labelerRequest);

        if (result.IsSuccess)
        {
            var createdLabeler = result.GetValueOrThrow();

            var labelerResponse = mapper.Map<LabelerResponse>(createdLabeler);

            return CreatedAtAction("GetLabeler", new { id = createdLabeler.Id }, labelerResponse);
        }

        return BadRequest(result.GetErrorOrThrow());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLabelerAsync(int id, LabelerRequest labelerRequest)
    {
        var result = await labelerService.UpdateLabelerAsync(id, labelerRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLabelerAsync(int id)
    {
        await labelerService.DeleteLabelerAsync(id);

        return NoContent();
    }
}