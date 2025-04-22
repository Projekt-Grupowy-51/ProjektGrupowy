using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using ProjektGrupowy.API.Utils.Extensions;
using System.Security.Claims;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class AssignedLabelController(
    IAssignedLabelService assignedLabelService,
    ILabelService labelService,
    IVideoService videoService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsAsync()
    {
            var assignedLabels = await assignedLabelService.GetAssignedLabelsAsync();
                
            return assignedLabels.IsSuccess 
                ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow())) 
                : NotFound(assignedLabels.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssignedLabelResponse>> GetAssignedLabelAsync(int id)
    {
        var assignedLabel = await assignedLabelService.GetAssignedLabelAsync(id);
        return assignedLabel.IsSuccess 
            ? Ok(mapper.Map<AssignedLabelResponse>(assignedLabel.GetValueOrThrow())) 
            : NotFound(assignedLabel.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<AssignedLabelResponse>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        assignedLabelRequest.LabelerId = User.GetUserId();
        
        var labelResult = await labelService.GetLabelAsync(assignedLabelRequest.LabelId);
        if (!labelResult.IsSuccess)
            return NotFound(labelResult.GetErrorOrThrow());

        var video = await videoService.GetVideoAsync(assignedLabelRequest.VideoId);
        if (!video.IsSuccess)
            return NotFound(video.GetErrorOrThrow());
        
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
        var assignedLabelResult = await assignedLabelService.GetAssignedLabelAsync(id);
        if (assignedLabelResult.IsFailure)
        {
            return NotFound(assignedLabelResult.GetErrorOrThrow());
        }

        await assignedLabelService.DeleteAssignedLabelAsync(id);
        return NoContent();
    }
}