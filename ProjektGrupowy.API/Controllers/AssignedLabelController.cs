using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.Features.AssignedLabels.Commands.AddAssignedLabel;
using ProjektGrupowy.Application.Features.AssignedLabels.Commands.DeleteAssignedLabel;
using ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabel;
using ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabels;
using ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing assigned labels. Handles operations such as retrieving, adding, and deleting assigned labels.
/// </summary>
[Route("api/assigned-labels")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class AssignedLabelController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all assigned labels.
    /// </summary>
    /// <returns>A collection of assigned labels.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsAsync()
    {
        var query = new GetAssignedLabelsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<AssignedLabelResponse>>(result.Value);
        return Ok(response);
    }
    
    /// <summary>
    /// Get a specific assigned label by its ID.
    /// </summary>
    /// <param name="id">The ID of the assigned label.</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignedLabelResponse>> GetAssignedLabelAsync(int id)
    {
        var query = new GetAssignedLabelQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<AssignedLabelResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Create a new assigned label.
    /// </summary>
    /// <param name="assignedLabelRequest">The request containing the details of the label to be created.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<AssignedLabelResponse>> AddAssignedLabelAsync(
        AssignedLabelRequest assignedLabelRequest)
    {
        var command = new AddAssignedLabelCommand(
            assignedLabelRequest.LabelId,
            assignedLabelRequest.VideoId,
            assignedLabelRequest.Start,
            assignedLabelRequest.End,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<AssignedLabelResponse>(result.Value);
        return CreatedAtAction("GetAssignedLabel", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Delete an assigned label by its ID.
    /// </summary>
    /// <param name="id">The ID of the label to be deleted.</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAssignedLabelAsync(int id)
    {
        var command = new DeleteAssignedLabelCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }
}