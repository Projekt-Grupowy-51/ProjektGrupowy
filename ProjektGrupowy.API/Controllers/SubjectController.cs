using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.Features.Subjects.Commands.AddSubject;
using ProjektGrupowy.Application.Features.Subjects.Commands.DeleteSubject;
using ProjektGrupowy.Application.Features.Subjects.Commands.UpdateSubject;
using ProjektGrupowy.Application.Features.Subjects.Queries.GetSubject;
using ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjectLabels;
using ProjektGrupowy.Application.Features.Subjects.Queries.GetSubjects;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing subjects. Handles operations such as retrieving, adding, updating, and deleting subjects.
/// </summary>
[Route("api/subjects")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class SubjectController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all subjects.
    /// </summary>
    /// <returns>A collection of subjects.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsAsync()
    {
        var query = new GetSubjectsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<SubjectResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <returns>The subject with the specified ID.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetSubjectAsync(int id)
    {
        var query = new GetSubjectQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<SubjectResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Create a new subject.
    /// </summary>
    /// <param name="subjectRequest">The request containing the details of the subject to be created.</param>
    /// <returns>The created subject.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> PostSubjectAsync(SubjectRequest subjectRequest)
    {
        var command = new AddSubjectCommand(
            subjectRequest.Name,
            subjectRequest.Description,
            subjectRequest.ProjectId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<SubjectResponse>(result.Value);
        return CreatedAtAction("GetSubject", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Update an existing subject.
    /// </summary>
    /// <param name="id">The ID of the subject to be updated.</param>
    /// <param name="subjectRequest">The request containing the updated details of the subject.</param>
    /// <returns>No content if successful, or BadRequest if the update fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectAsync(int id, SubjectRequest subjectRequest)
    {
        var command = new UpdateSubjectCommand(
            id,
            subjectRequest.Name,
            subjectRequest.Description,
            subjectRequest.ProjectId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
    }

    /// <summary>
    /// Delete a subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject to be deleted.</param>
    /// <returns>No content if successful, or NotFound if the subject does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectAsync(int id)
    {
        var command = new DeleteSubjectCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Get all labels associated with a specific subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <returns>>A collection of labels associated with the subject.</returns>
    [HttpGet("{id:int}/labels")]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetSubjectLabelsAsync(int id)
    {
        var query = new GetSubjectLabelsQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<LabelResponse>>(result.Value);
        return Ok(response);
    }
}
