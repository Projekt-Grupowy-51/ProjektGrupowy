using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ProjektGrupowy.API.DTOs.AccessCode;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using ProjektGrupowy.API.Utils.Extensions;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class AccessCodeController(
    IProjectAccessCodeService service, 
    IProjectService projectService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("project/{projectId:int}")]
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    public async Task<ActionResult<IEnumerable<AccessCodeResponse>>> GetAccessCodesByProjectAsync(int projectId)
    {
        var accessCodes = await service.GetAccessCodesByProjectAsync(projectId);
        return accessCodes.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AccessCodeResponse>>(accessCodes.GetValueOrThrow()))
            : NotFound(accessCodes.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{code:required}/retire")]
    public async Task<IActionResult> RetireCodeAsync(string code)
    {
        var result = await service.RetireAccessCodeAsync(code);
        return result.IsSuccess
            ? Ok()
            : NotFound(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateAccessCodeAsync(AccessCodeRequest accessCodeRequest)
    {
        var result = await service.ValidateAccessCode(accessCodeRequest);
        return Ok(result);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("project")]
    public async Task<ActionResult<AccessCodeResponse>> AddValidCodeToProjectAsync(
        CreateAccessCodeRequest createCodeRequest)
    {
        createCodeRequest.OwnerId = User.GetUserId();

        var project = await projectService.GetProjectAsync(createCodeRequest.ProjectId);
        if (project.IsFailure)
            return NotFound(project.GetErrorOrThrow());

        var result = await service.AddValidCodeToProjectAsync(createCodeRequest);
        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());
            
        var createdAccessCode = result.GetValueOrThrow();
        var accessCodeResponse = mapper.Map<AccessCodeResponse>(createdAccessCode);
        

        return CreatedAtAction(
            "GetAccessCodesByProject",
            new { projectId = createdAccessCode.Project.Id },
            accessCodeResponse
        );
    }
}