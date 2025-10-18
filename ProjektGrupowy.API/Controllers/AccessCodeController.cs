using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.AccessCode;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing access codes. Handles operations such as retrieving, validating, retiring, and creating access codes.
/// </summary>
/// <param name="service"></param>
/// <param name="projectService"></param>
/// <param name="mapper"></param>
[Route("api/access-codes")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class AccessCodeController(
    IProjectAccessCodeService service,
    IProjectService projectService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all access codes associated with a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of access codes for the specified project.</returns>
    [HttpGet("projects/{projectId:int}")]
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    public async Task<ActionResult<IEnumerable<AccessCodeResponse>>> GetAccessCodesByProjectAsync(int projectId)
    {
        var accessCodes = await service.GetAccessCodesByProjectAsync(projectId);
        return accessCodes.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AccessCodeResponse>>(accessCodes.GetValueOrThrow()))
            : NotFound(accessCodes.GetErrorOrThrow());
    }

    /// <summary>
    /// Retire (deactivate) an access code by its code string.
    /// </summary>
    /// <param name="code">The access code to be retired.</param>
    /// <returns>Empty result if successful, or NotFound if the code does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{code:required}/retire")]
    public async Task<IActionResult> RetireCodeAsync(string code)
    {
        var result = await service.RetireAccessCodeAsync(code);
        return result.IsSuccess
            ? Ok()
            : NotFound(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Check if a given access code is valid.
    /// </summary>
    /// <param name="accessCodeRequest">The access code request containing the code to be validated.</param>
    /// <returns>A boolean indicating whether the access code is valid.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateAccessCodeAsync(AccessCodeRequest accessCodeRequest)
    {
        var result = await service.ValidateAccessCode(accessCodeRequest);
        return Ok(result);
    }

    /// <summary>
    /// Add a new valid access code to a specific project. Invalidates any existing codes for that project.
    /// </summary>
    /// <param name="createCodeRequest">The request containing details for the new access code.</param>
    /// <returns>The created access code response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("project")]
    public async Task<ActionResult<AccessCodeResponse>> AddValidCodeToProjectAsync(
        CreateAccessCodeRequest createCodeRequest)
    {
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