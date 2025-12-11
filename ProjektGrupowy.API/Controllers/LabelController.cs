using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.Label;
using ProjektGrupowy.Application.Features.Labels.Commands.AddLabel;
using ProjektGrupowy.Application.Features.Labels.Commands.DeleteLabel;
using ProjektGrupowy.Application.Features.Labels.Commands.UpdateLabel;
using ProjektGrupowy.Application.Features.Labels.Queries.GetLabel;
using ProjektGrupowy.Application.Features.Labels.Queries.GetLabels;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing labels. Handles operations such as retrieving, adding, updating, and deleting labels.
/// </summary>
[Route("api/labels")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class LabelController(
    IMediator mediator,
    ICurrentUserService currentUserService,
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
        var query = new GetLabelsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<LabelResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetLabelQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<LabelResponse>(result.Value);
        return Ok(response);
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
        var command = new AddLabelCommand(
            labelRequest.Name,
            null,
            currentUserService.UserId,
            labelRequest.SubjectId,
            labelRequest.ColorHex,
            labelRequest.Shortcut,
            labelRequest.Type,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<LabelResponse>(result.Value);
        return CreatedAtAction("GetLabel", new { id = result.Value.Id }, response);
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
        var command = new UpdateLabelCommand(
            id,
            labelRequest.Name,
            null,
            currentUserService.UserId,
            labelRequest.SubjectId,
            labelRequest.Shortcut,
            labelRequest.ColorHex,
            labelRequest.Type,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
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
        var command = new DeleteLabelCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }
}
