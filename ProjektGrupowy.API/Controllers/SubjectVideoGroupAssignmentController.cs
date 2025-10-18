using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.Labeler;
using ProjektGrupowy.Application.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing subject video group assignments. Handles operations such as retrieving, adding, updating, and deleting subject video group assignments.
/// </summary>
/// <param name="subjectVideoGroupAssignmentService"></param>
/// <param name="mapper"></param>
[Route("api/subject-video-group-assignments")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class SubjectVideoGroupAssignmentController(
    ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all subject video group assignments.
    /// </summary>
    /// <returns>A collection of subject video group assignments.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        var subjectVideoGroupAssignments = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsAsync();

        return subjectVideoGroupAssignments.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(subjectVideoGroupAssignments.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignments.GetErrorOrThrow());
    }

    /// <summary>
    /// Get a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <returns>The subject video group assignment with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
        return subjectVideoGroupAssignment.IsSuccess
            ? Ok(mapper.Map<SubjectVideoGroupAssignmentResponse>(subjectVideoGroupAssignment.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignment.GetErrorOrThrow());
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
        var labelers = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsLabelersAsync(id);
        return labelers.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(labelers.GetValueOrThrow()))
            : NotFound(labelers.GetErrorOrThrow());
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
        var result = await subjectVideoGroupAssignmentService.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdAssignment = result.GetValueOrThrow();
        var response = mapper.Map<SubjectVideoGroupAssignmentResponse>(createdAssignment);

        return CreatedAtAction("GetSubjectVideoGroupAssignment", new { id = createdAssignment.Id }, response);
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
        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
        if (subjectVideoGroupAssignment.IsFailure)
            return NotFound(subjectVideoGroupAssignment.GetErrorOrThrow());

        var result = await subjectVideoGroupAssignmentService.UpdateSubjectVideoGroupAssignmentAsync(id, subjectVideoGroupAssignmentRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
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
        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
        if (subjectVideoGroupAssignment.IsFailure)
            return NotFound(subjectVideoGroupAssignment.GetErrorOrThrow());

        await subjectVideoGroupAssignmentService.DeleteSubjectVideoGroupAssignmentAsync(id);

        return NoContent();
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
        var result = await subjectVideoGroupAssignmentService.AssignLabelerToAssignmentAsync(assignmentId, labelerId);
        return result.IsSuccess ? Ok("Labeler assigned successfully") : NotFound(result.GetErrorOrThrow());
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
        var result = await subjectVideoGroupAssignmentService.UnassignLabelerFromAssignmentAsync(assignmentId, labelerId);
        return result.IsSuccess ? Ok("Labeler unassigned successfully") : NotFound(result.GetErrorOrThrow());
    }
}