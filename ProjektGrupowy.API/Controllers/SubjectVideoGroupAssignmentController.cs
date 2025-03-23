using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.DTOs.Labeler;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class SubjectVideoGroupAssignmentController(
    ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, 
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var labelerResult = await authHelper.GetLabelerFromUserAsync(User);
            if (labelerResult.Error != null)
                return labelerResult.Error;

            var assignments = await subjectVideoGroupAssignmentService.GetAssignmentsForLabelerAsync(labelerResult.Labeler!.Id);
            
            return assignments.IsSuccess
                ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(assignments.GetValueOrThrow()))
                : NotFound(assignments.GetErrorOrThrow());
        }
        
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        var subjectVideoGroupAssignments = checkResult.IsAdmin
            ? await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsAsync()
            : await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByScientistIdAsync(checkResult.Scientist!.Id);

        return subjectVideoGroupAssignments.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(subjectVideoGroupAssignments.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignments.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var authResult = await authHelper.EnsureLabelerCanAccessAssignmentAsync(User, id);
            if (authResult != null)
                return authResult;
            
            var assignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
            return assignment.IsSuccess
                ? Ok(mapper.Map<SubjectVideoGroupAssignmentResponse>(assignment.GetValueOrThrow()))
                : NotFound(assignment.GetErrorOrThrow());
        }
        
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(User, id);
            if (authResult != null)
                return authResult;
        }

        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
        return subjectVideoGroupAssignment.IsSuccess
            ? Ok(mapper.Map<SubjectVideoGroupAssignmentResponse>(subjectVideoGroupAssignment.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignment.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/Labelers")]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetSubjectVideoGroupAssignmentLabelersAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(User, id);
            if (authResult != null)
                return authResult;
        }

        var labelers = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsLabelersAsync(id);
        return labelers.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelerResponse>>(labelers.GetValueOrThrow()))
            : NotFound(labelers.GetErrorOrThrow());
    }

    [HttpGet("{id:int}/AssignedLabels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetSubjectVideoGroupAssignmentAsignedLabelsAsync(int id)
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var authResult = await authHelper.EnsureLabelerCanAccessAssignmentAsync(User, id);
            if (authResult != null)
                return authResult;
            
            var labelerResult = await authHelper.GetLabelerFromUserAsync(User);
            if (labelerResult.Error != null)
                return labelerResult.Error;
            
            var labelerLabels = await subjectVideoGroupAssignmentService.GetLabelerAssignedLabelsAsync(id, labelerResult.Labeler!.Id);
            return labelerLabels.IsSuccess
                ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(labelerLabels.GetValueOrThrow()))
                : NotFound(labelerLabels.GetErrorOrThrow());
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;

            if (checkResult.IsScientist)
            {
                var authResult = await authHelper.EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(User, id);
                if (authResult != null)
                    return authResult;
            }
        }

        var labels = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsignedLabelsAsync(id);
        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var subjectAuthResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, subjectVideoGroupAssignmentRequest.SubjectId);
            if (subjectAuthResult != null)
                return subjectAuthResult;
            
            var videoGroupAuthResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, subjectVideoGroupAssignmentRequest.VideoGroupId);
            if (videoGroupAuthResult != null)
                return videoGroupAuthResult;
        }

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
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var assignmentAuthResult = await authHelper.EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(User, id);
            if (assignmentAuthResult != null)
                return assignmentAuthResult;
            
            var subjectAuthResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, subjectVideoGroupAssignmentRequest.SubjectId);
            if (subjectAuthResult != null)
                return subjectAuthResult;
            
            var videoGroupAuthResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, subjectVideoGroupAssignmentRequest.VideoGroupId);
            if (videoGroupAuthResult != null)
                return videoGroupAuthResult;
        }

        var result = await subjectVideoGroupAssignmentService.UpdateSubjectVideoGroupAssignmentAsync(id, subjectVideoGroupAssignmentRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectVideoGroupAssignmentAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(User, id);
            if (authResult != null)
                return authResult;
        }

        await subjectVideoGroupAssignmentService.DeleteSubjectVideoGroupAssignmentAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{assignmentId}/assign-labeler/{labelerId}")]
    public async Task<IActionResult> AssignLabelerToSubjectVideoGroup(int assignmentId, int labelerId)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(User, assignmentId);
            if (authResult != null)
                return authResult;
        }

        var result = await subjectVideoGroupAssignmentService.AssignLabelerToAssignmentAsync(assignmentId, labelerId);
        return result.IsSuccess ? Ok("Labeler assigned successfully") : NotFound(result.GetErrorOrThrow());
    }
}