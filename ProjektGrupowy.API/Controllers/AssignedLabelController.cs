using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class AssignedLabelController(IAssignedLabelService assignedLabelService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsAsync()
    {
        var assignedLabels = await assignedLabelService.GetAssignedLabelsAsync();
        return assignedLabels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
            : NotFound(assignedLabels.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignedLabelResponse>> GetAssignedLabelAsync(int id)
    {
        var assignedLabel = await assignedLabelService.GetAssignedLabelAsync(id);
        return assignedLabel.IsSuccess
            ? Ok(mapper.Map<AssignedLabelResponse>(assignedLabel.GetValueOrThrow()))
            : NotFound(assignedLabel.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<AssignedLabelResponse>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        var result = await assignedLabelService.AddAssignedLabelAsync(assignedLabelRequest);

        if (result.IsFailure) 
            return BadRequest(result.GetErrorOrThrow());
        
        var createdAssignedLabel = result.GetValueOrThrow();

        var assignedLabelResponse = mapper.Map<AssignedLabelResponse>(createdAssignedLabel);

        return CreatedAtAction("GetAssignedLabel", new { id = createdAssignedLabel.Id }, assignedLabelResponse);

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAssignedLabelAsync(int id, AssignedLabelRequest assignedLabelRequest)
    {
        var result = await assignedLabelService.UpdateAssignedLabelAsync(id, assignedLabelRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAssignedLabelAsync(int id)
    {
        await assignedLabelService.DeleteAssignedLabelAsync(id);

        return NoContent();
    }

}