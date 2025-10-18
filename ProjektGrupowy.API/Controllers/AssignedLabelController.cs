using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.AssignedLabel;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing assigned labels. Handles operations such as retrieving, adding, and deleting assigned labels.
/// </summary>
/// <param name="assignedLabelService"></param>
/// <param name="labelService"></param>
/// <param name="videoService"></param>
/// <param name="mapper"></param>
[Route("api/assigned-labels")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class AssignedLabelController(
    IAssignedLabelService assignedLabelService,
    ILabelService labelService,
    IVideoService videoService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all assigned labels.
    /// </summary>
    /// <returns>A collection of assigned labels.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsAsync()
    {
        var assignedLabels = await assignedLabelService.GetAssignedLabelsAsync();

        return assignedLabels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
            : NotFound(assignedLabels.GetErrorOrThrow());
    }

    /// <summary>
    /// Get a specific assigned label by its ID.
    /// </summary>
    /// <param name="id">The ID of the assigned label.</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignedLabelResponse>> GetAssignedLabelAsync(int id)
    {
        var assignedLabel = await assignedLabelService.GetAssignedLabelAsync(id);
        return assignedLabel.IsSuccess
            ? Ok(mapper.Map<AssignedLabelResponse>(assignedLabel.GetValueOrThrow()))
            : NotFound(assignedLabel.GetErrorOrThrow());
    }

    /// <summary>
    /// Create a new assigned label.
    /// </summary>
    /// <param name="assignedLabelRequest">The request containing the details of the label to be created.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<AssignedLabelResponse>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        var labelResult = await labelService.GetLabelAsync(assignedLabelRequest.LabelId);
        if (!labelResult.IsSuccess)
            return NotFound(labelResult.GetErrorOrThrow());

        var video = await videoService.GetVideoAsync(assignedLabelRequest.VideoId);
        if (!video.IsSuccess)
            return NotFound(video.GetErrorOrThrow());

        var result = await assignedLabelService.AddAssignedLabelAsync(assignedLabelRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdAssignedLabel = result.GetValueOrThrow();
        var assignedLabelResponse = mapper.Map<AssignedLabelResponse>(createdAssignedLabel);

        return CreatedAtAction("GetAssignedLabel", new { id = createdAssignedLabel.Id }, assignedLabelResponse);
    }

    /// <summary>
    /// Delete an assigned label by its ID.
    /// </summary>
    /// <param name="id">The ID of the label to be deleted.</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAssignedLabelAsync(int id)
    {
        var assignedLabelResult = await assignedLabelService.GetAssignedLabelAsync(id);
        if (assignedLabelResult.IsFailure)
        {
            return NotFound(assignedLabelResult.GetErrorOrThrow());
        }

        await assignedLabelService.DeleteAssignedLabelAsync(id);
        return NoContent();
    }
}