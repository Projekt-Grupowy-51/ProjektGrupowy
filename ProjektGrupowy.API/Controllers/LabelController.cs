using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.Label;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing labels. Handles operations such as retrieving, adding, updating, and deleting labels.
/// </summary>
/// <param name="labelService"></param>
/// <param name="mapper"></param>
[Route("api/labels")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class LabelController(
    ILabelService labelService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all labels.
    /// </summary>
    /// <returns>A collection of labels.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetLabelsAsync()
    {
        var labels = await labelService.GetLabelsAsync();

        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }
    
    /// <summary>
    /// Get a specific label by its ID.
    /// </summary>
    /// <param name="id">The ID of the label.</param>
    /// <returns>The label with the specified ID.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabelResponse>> GetLabelAsync(int id)
    {
        var label = await labelService.GetLabelAsync(id);
        return label.IsSuccess
            ? Ok(mapper.Map<LabelResponse>(label.GetValueOrThrow()))
            : NotFound(label.GetErrorOrThrow());
    }
    
    /// <summary>
    /// Create a new label.
    /// </summary>
    /// <param name="labelRequest">The request containing the details of the label to be created.</param>
    /// <returns>The created label.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
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

    /// <summary>
    /// Update an existing label.
    /// </summary>
    /// <param name="id">The ID of the label to be updated.</param>
    /// <param name="labelRequest">The request containing the updated details of the label.</param>
    /// <returns>No content if successful, or BadRequest if the update fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutLabelAsync(int id, LabelRequest labelRequest)
    {
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

    /// <summary>
    /// Delete a label by its ID.
    /// </summary>
    /// <param name="id">The ID of the label to be deleted.</param>
    /// <returns>No content if successful, or NotFound if the label does not exist.</returns>
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