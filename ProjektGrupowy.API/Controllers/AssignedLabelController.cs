using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using System.Security.Claims;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class AssignedLabelController(
    IAssignedLabelService assignedLabelService,
    ILabelerService labelerService,
    ILabelService labelService,
    ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService,
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsAsync()
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var labelerResult = await authHelper.GetLabelerFromUserAsync(User);
            if (labelerResult.Error != null)
                return labelerResult.Error;

            var labelerAssignedLabels = await assignedLabelService.GetAssignedLabelsByLabelerIdAsync(labelerResult.Labeler!.Id);
            return labelerAssignedLabels.IsSuccess 
                ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(labelerAssignedLabels.GetValueOrThrow())) 
                : NotFound(labelerAssignedLabels.GetErrorOrThrow());
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;

            var assignedLabels = await assignedLabelService.GetAssignedLabelsAsync();
            return assignedLabels.IsSuccess 
                ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow())) 
                : NotFound(assignedLabels.GetErrorOrThrow());
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignedLabelResponse>> GetAssignedLabelAsync(int id)
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var authResult = await authHelper.EnsureLabelerOwnsAssignedLabelAsync(User, id);
            if (authResult != null)
                return authResult;
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;
        }

        var assignedLabel = await assignedLabelService.GetAssignedLabelAsync(id);
        return assignedLabel.IsSuccess 
            ? Ok(mapper.Map<AssignedLabelResponse>(assignedLabel.GetValueOrThrow())) 
            : NotFound(assignedLabel.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<AssignedLabelResponse>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var labelerResult = await authHelper.GetLabelerFromUserAsync(User);
            if (labelerResult.Error != null)
                return labelerResult.Error;

            var authResult = await authHelper.EnsureLabelerCanAccessAssignmentAsync(User, assignedLabelRequest.SubjectVideoGroupAssignmentId);
            if (authResult != null)
                return authResult;

            assignedLabelRequest.LabelerId = labelerResult.Labeler!.Id;
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;
        }

        var result = await assignedLabelService.AddAssignedLabelAsync(assignedLabelRequest);

        if (result.IsFailure) 
            return BadRequest(result.GetErrorOrThrow());
        
        var createdAssignedLabel = result.GetValueOrThrow();
        var assignedLabelResponse = mapper.Map<AssignedLabelResponse>(createdAssignedLabel);

        return CreatedAtAction("GetAssignedLabel", new { id = createdAssignedLabel.Id }, assignedLabelResponse);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAssignedLabelAsync(int id)
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var authResult = await authHelper.EnsureLabelerOwnsAssignedLabelAsync(User, id);
            if (authResult != null)
                return authResult;
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;
        }

        await assignedLabelService.DeleteAssignedLabelAsync(id);
        return NoContent();
    }
}