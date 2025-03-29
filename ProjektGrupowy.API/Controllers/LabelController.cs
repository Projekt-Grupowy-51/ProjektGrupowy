using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class LabelController(
    ILabelService labelService,
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetLabelsAsync()
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        var labels = checkResult.IsAdmin
            ? await labelService.GetLabelsAsync()
            : await labelService.GetLabelsByScientistIdAsync(checkResult.Scientist!.Id);

        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LabelResponse>> GetLabelAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsLabelAsync(User, id);
            if (authResult != null)
                return authResult;
        }

        var label = await labelService.GetLabelAsync(id);
        return label.IsSuccess
            ? Ok(mapper.Map<LabelResponse>(label.GetValueOrThrow()))
            : NotFound(label.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<LabelResponse>> AddLabelAsync(LabelRequest labelRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, labelRequest.SubjectId);
            if (authResult != null)
                return authResult;
        }

        var result = await labelService.AddLabelAsync(labelRequest);

        if (result.IsFailure) 
            return BadRequest(result.GetErrorOrThrow());
        
        var createdLabel = result.GetValueOrThrow();
        var labelResponse = mapper.Map<LabelResponse>(createdLabel);

        return CreatedAtAction("GetLabel", new { id = createdLabel.Id }, labelResponse);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutLabelAsync(int id, LabelRequest labelRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsLabelAsync(User, id);
            if (authResult != null)
                return authResult;
                
            var subjectAuthResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, labelRequest.SubjectId);
            if (subjectAuthResult != null)
                return subjectAuthResult;
        }

        var result = await labelService.UpdateLabelAsync(id, labelRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLabelAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
            return checkResult.Error;

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsLabelAsync(User, id);
            if (authResult != null)
                return authResult;
        }

        await labelService.DeleteLabelAsync(id);

        return NoContent();
    }
}