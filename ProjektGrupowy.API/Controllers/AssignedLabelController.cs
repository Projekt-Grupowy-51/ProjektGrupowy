using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Services.Impl;
using ProjektGrupowy.API.Utils.Enums;
using System.Security.Claims;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class AssignedLabelController(IAssignedLabelService assignedLabelService, ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, IScientistService scientistService, ILabelerService labelerService, IMapper mapper) : ControllerBase
{
    private (ActionResult? Error, bool IsScientist, bool IsAdmin, bool IsLabeler, Scientist? Scientist, Labeler? Labeler) CheckGeneralAccess()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return (Unauthorized("User identity not found."), false, false, false, null, null);
        }

        bool isScientist = User.IsInRole(RoleEnum.Scientist.ToString());
        bool isAdmin = User.IsInRole(RoleEnum.Admin.ToString());
        bool isLabeler = User.IsInRole(RoleEnum.Labeler.ToString());

        if (!isScientist && !isAdmin && !isLabeler)
        {
            return (Forbid(), false, false, false, null, null);
        }

        if (isScientist)
        {
            return (null, true, false, false, scientistService.GetScientistByUserIdAsync(userId).Result.GetValueOrThrow(), null);
        }

        if (isLabeler)
        {
            return (null, false, false, true, null, labelerService.GetLabelerByUserIdAsync(userId).Result.GetValueOrThrow());
        }

        return (null, false, true, false, null, null);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsAsync()
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var labels = checkResult.IsAdmin switch {
            true => await assignedLabelService.GetAssignedLabelsAsync(),
            false => checkResult.IsScientist switch {
                true => await assignedLabelService.GetAssignedLabelsByScientistIdAsync(checkResult.Scientist!.Id),
                false => await assignedLabelService.GetAssignedLabelsByLabelerIdAsync(checkResult.Labeler!.Id)
            }
        };

        return labels.IsSuccess ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(labels.GetValueOrThrow())) : NotFound(labels.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignedLabelResponse>> GetAssignedLabelAsync(int id)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var labelResult = await assignedLabelService.GetAssignedLabelAsync(id);

        if (labelResult.IsFailure)
        {
            return NotFound(new { Message = "Label not found" });
        }

        var label = labelResult.GetValueOrThrow();

        if (checkResult.IsScientist)
        {
            if (label.SubjectVideoGroupAssignment.Subject.Project.Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid("Not your project");
            }

            return Ok(mapper.Map<AssignedLabelResponse>(label));
        }

        if (checkResult.IsLabeler)
        {
            if (label.Labeler.Id != checkResult.Labeler!.Id)
            {
                return Forbid("Not your assignment");
            }

            return Ok(mapper.Map<AssignedLabelResponse>(label));
        }

        return Ok(mapper.Map<AssignedLabelResponse>(label));
    }

    [HttpPost]
    public async Task<ActionResult<AssignedLabelResponse>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            return BadRequest("Not implemented");
        }

        if (checkResult.IsLabeler)
        {
            var isAssigned = await assignedLabelService.IsAssignedToLabelerAsync(assignedLabelRequest.SubjectVideoGroupAssignmentId, checkResult.Labeler!.Id);
            if (!isAssigned)
            {
                return Forbid("Not assigned to this assignment");
            }

            assignedLabelRequest.LabelerId = checkResult.Labeler.Id;
        }

        var result = await assignedLabelService.AddAssignedLabelAsync(assignedLabelRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdAssignedLabel = result.GetValueOrThrow();
        return CreatedAtAction(
            nameof(GetAssignedLabelAsync),
            new { id = createdAssignedLabel.Id },
            mapper.Map<AssignedLabelResponse>(createdAssignedLabel));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAssignedLabelAsync(int id)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var existingLabelResult = await assignedLabelService.GetAssignedLabelAsync(id);

        if (existingLabelResult.IsFailure)
        {
            return NotFound(new { Message = "Label not found" });
        }

        var existingLabel = existingLabelResult.GetValueOrThrow();

        if (checkResult.IsScientist)
        {
            if (existingLabel.SubjectVideoGroupAssignment.Subject.Project.Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid("Not your project");
            }
        }
    
        if (checkResult.IsLabeler)
        {
            if (existingLabel.Labeler.Id != checkResult.Labeler!.Id)
            {
                return Forbid("Not your assignment");
            }
        }

        await assignedLabelService.DeleteAssignedLabelAsync(id);

        return NoContent();
    }

}