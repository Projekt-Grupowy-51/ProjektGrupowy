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

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class SubjectVideoGroupAssignmentController(
    ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        var subjectVideoGroupAssignments = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsAsync();

        return subjectVideoGroupAssignments.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(subjectVideoGroupAssignments.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignments.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
        return subjectVideoGroupAssignment.IsSuccess
            ? Ok(mapper.Map<SubjectVideoGroupAssignmentResponse>(subjectVideoGroupAssignment.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignment.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/Labelers")]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetSubjectVideoGroupAssignmentLabelersAsync(int id)
    {
        var labelers = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsLabelersAsync(id);
        return labelers.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(labelers.GetValueOrThrow()))
            : NotFound(labelers.GetErrorOrThrow());
    }

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

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{assignmentId}/assign-labeler/{labelerId}")]
    public async Task<IActionResult> AssignLabelerToSubjectVideoGroup(int assignmentId, string labelerId)
    {
        var result = await subjectVideoGroupAssignmentService.AssignLabelerToAssignmentAsync(assignmentId, labelerId);
        return result.IsSuccess ? Ok("Labeler assigned successfully") : NotFound(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{assignmentId}/unassign-labeler/{labelerId}")]
    public async Task<IActionResult> UnassignLabelerFromSubjectVideoGroup(int assignmentId, string labelerId)
    {
        var result = await subjectVideoGroupAssignmentService.UnassignLabelerFromAssignmentAsync(assignmentId, labelerId);
        return result.IsSuccess ? Ok("Labeler unassigned successfully") : NotFound(result.GetErrorOrThrow());
    }
}