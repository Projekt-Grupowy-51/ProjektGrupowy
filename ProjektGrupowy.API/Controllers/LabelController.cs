using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using ProjektGrupowy.API.Utils.Extensions;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class LabelController(
    ILabelService labelService,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetLabelsAsync()
    {
        var labels = await labelService.GetLabelsAsync();

        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabelResponse>> GetLabelAsync(int id)
    {
        var label = await labelService.GetLabelAsync(id);
        return label.IsSuccess
            ? Ok(mapper.Map<LabelResponse>(label.GetValueOrThrow()))
            : NotFound(label.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<LabelResponse>> AddLabelAsync(LabelRequest labelRequest)
    {
        labelRequest.OwnerId = User.GetUserId();

        var result = await labelService.AddLabelAsync(labelRequest);

        if (result.IsFailure) 
            return BadRequest(result.GetErrorOrThrow());
        
        var createdLabel = result.GetValueOrThrow();
        var labelResponse = mapper.Map<LabelResponse>(createdLabel);

        return CreatedAtAction("GetLabel", new { id = createdLabel.Id }, labelResponse);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutLabelAsync(int id, LabelRequest labelRequest)
    {
        labelRequest.OwnerId = User.GetUserId();

        var labelResult = await labelService.GetLabelAsync(id);
        if (labelResult.IsFailure)
        {
            return NotFound(labelResult.GetErrorOrThrow());
        }

        var result = await labelService.UpdateLabelAsync(id, labelRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLabelAsync(int id)
    {
        var labelResult = await labelService.GetLabelAsync(id);
        if (labelResult.IsFailure)
        {
            return NotFound(labelResult.GetErrorOrThrow());
        }

        await labelService.DeleteLabelAsync(id);

        return NoContent();
    }
}