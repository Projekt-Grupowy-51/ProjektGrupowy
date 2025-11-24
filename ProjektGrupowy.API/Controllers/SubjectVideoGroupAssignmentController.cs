using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Labeler;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.AddSubjectVideoGroupAssignment;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.AssignLabelerToAssignment;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.DeleteSubjectVideoGroupAssignment;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.UnassignLabelerFromAssignment;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Commands.UpdateSubjectVideoGroupAssignment;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignment;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentLabelers;
using ProjektGrupowy.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignments;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing subject video group assignments. Handles operations such as retrieving, adding, updating, and deleting subject video group assignments.
/// </summary>
[Route("api/subject-video-group-assignments")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class SubjectVideoGroupAssignmentController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all subject video group assignments.
    /// </summary>
    /// <returns>A collection of subject video group assignments.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        var query = new GetSubjectVideoGroupAssignmentsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <returns>The subject video group assignment with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        var query = new GetSubjectVideoGroupAssignmentQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<SubjectVideoGroupAssignmentResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get labelers assigned to a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <returns>A collection of labelers assigned to the specified subject video group assignment.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/labelers")]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetSubjectVideoGroupAssignmentLabelersAsync(int id)
    {
        var query = new GetSubjectVideoGroupAssignmentLabelersQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<LabelerResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Create a new subject video group assignment.
    /// </summary>
    /// <param name="subjectVideoGroupAssignmentRequest">The request containing the details of the subject video group assignment to be created.</param>
    /// <returns>The created subject video group assignment.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var command = new AddSubjectVideoGroupAssignmentCommand(
            subjectVideoGroupAssignmentRequest.SubjectId,
            subjectVideoGroupAssignmentRequest.VideoGroupId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<SubjectVideoGroupAssignmentResponse>(result.Value);
        return CreatedAtAction("GetSubjectVideoGroupAssignment", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Update an existing subject video group assignment.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment to be updated.</param>
    /// <param name="subjectVideoGroupAssignmentRequest">The request containing the updated details of the subject video group assignment.</param>
    /// <returns>No content if the update was successful, otherwise a bad request or not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectVideoGroupAssignmentAsync(int id, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var command = new UpdateSubjectVideoGroupAssignmentCommand(
            id,
            subjectVideoGroupAssignmentRequest.SubjectId,
            subjectVideoGroupAssignmentRequest.VideoGroupId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
    }

    /// <summary>
    /// Delete a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment to be deleted.</param>
    /// <returns>No content if the deletion was successful, otherwise a not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectVideoGroupAssignmentAsync(int id)
    {
        var command = new DeleteSubjectVideoGroupAssignmentCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Assign a labeler to a specific subject video group assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the subject video group assignment.</param>
    /// <param name="labelerId">The ID of the labeler to be assigned.</param>
    /// <returns>Ok if the assignment was successful, otherwise a not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{assignmentId:int}/assign-labeler/{labelerId}")]
    public async Task<IActionResult> AssignLabelerToSubjectVideoGroup(int assignmentId, string labelerId)
    {
        var command = new AssignLabelerToAssignmentCommand(assignmentId, labelerId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok("Labeler assigned successfully")
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Unassign a labeler from a specific subject video group assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the subject video group assignment.</param>
    /// <param name="labelerId">The ID of the labeler to be unassigned.</param>
    /// <returns>>Ok if the unassignment was successful, otherwise a not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{assignmentId:int}/unassign-labeler/{labelerId}")]
    public async Task<IActionResult> UnassignLabelerFromSubjectVideoGroup(int assignmentId, string labelerId)
    {
        var command = new UnassignLabelerFromAssignmentCommand(assignmentId, labelerId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok("Labeler unassigned successfully")
            : NotFound(result.Errors);
    }
}
