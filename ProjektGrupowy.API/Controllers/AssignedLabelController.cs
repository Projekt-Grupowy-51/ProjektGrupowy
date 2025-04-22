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
    IVideoService videoService,
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

            var assignedLabels = checkResult.IsAdmin
                ? await assignedLabelService.GetAssignedLabelsAsync()
                : await assignedLabelService.GetAssignedLabelsByScientistIdAsync(checkResult.Scientist!.Id);
                
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
                
            if (checkResult.IsScientist)
            {
                var assignedLabelResult = await assignedLabelService.GetAssignedLabelAsync(id);
                if (!assignedLabelResult.IsSuccess)
                    return NotFound(assignedLabelResult.GetErrorOrThrow());
                    
                var assignedLabelValue = assignedLabelResult.GetValueOrThrow();
                
                var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, assignedLabelValue.Video.Id);
                if (authResult != null)
                    return authResult;
            }
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

            var authResult = await authHelper.EnsureLabelerCanAccessVideoAsync(User, assignedLabelRequest.VideoId);
            if (authResult != null)
                return authResult;

            assignedLabelRequest.LabelerId = labelerResult.Labeler!.Id;
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;
                
            if (checkResult.IsScientist)
            {
                var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, assignedLabelRequest.VideoId);
                if (authResult != null)
                    return authResult;
                    
                var labelResult = await labelService.GetLabelAsync(assignedLabelRequest.LabelId);
                if (!labelResult.IsSuccess)
                    return NotFound("Label not found");
                    
                var label = labelResult.GetValueOrThrow();
                if (label.Subject.Project.Scientist.Id != checkResult.Scientist!.Id)
                    return Forbid("You don't have permission to use this label");
            }
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
                
            if (checkResult.IsScientist)
            {
                var assignedLabelResult = await assignedLabelService.GetAssignedLabelAsync(id);
                if (!assignedLabelResult.IsSuccess)
                    return NotFound(assignedLabelResult.GetErrorOrThrow());
                    
                var assignedLabel = assignedLabelResult.GetValueOrThrow();
                
                var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, assignedLabel.Video.Id);
                if (authResult != null)
                    return authResult;
            }
        }

        await assignedLabelService.DeleteAssignedLabelAsync(id);
        return NoContent();
    }
}