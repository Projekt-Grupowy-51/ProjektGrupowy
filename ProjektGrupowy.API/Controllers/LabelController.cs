using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class LabelController(ILabelService labelService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetLabelsAsync()
    {
        var labels = await labelService.GetLabelsAsync();
        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabelResponse>> GetLabelAsync(int id)
    {
        var label = await labelService.GetLabelAsync(id);
        return label.IsSuccess
            ? Ok(mapper.Map<LabelResponse>(label.GetValueOrThrow()))
            : NotFound(label.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<LabelResponse>> AddLabelAsync(LabelRequest labelRequest)
    {
        var result = await labelService.AddLabelAsync(labelRequest);

        if (result.IsFailure) 
            return BadRequest(result.GetErrorOrThrow());
        
        var createdLabel = result.GetValueOrThrow();

        var labelResponse = mapper.Map<LabelResponse>(createdLabel);

        return CreatedAtAction("GetLabel", new { id = createdLabel.Id }, labelResponse);

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutLabelAsync(int id, LabelRequest labelRequest)
    {
        var result = await labelService.UpdateLabelAsync(id, labelRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLabelAsync(int id)
    {
        await labelService.DeleteLabelAsync(id);

        return NoContent();
    }
}