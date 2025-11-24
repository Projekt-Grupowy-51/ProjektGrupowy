using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VidMark.API.DTOs.AccessCode;
using VidMark.API.Filters;
using VidMark.Application.Features.ProjectAccessCodes.Commands.AddValidCodeToProject;
using VidMark.Application.Features.ProjectAccessCodes.Commands.RetireAccessCode;
using VidMark.Application.Features.ProjectAccessCodes.Queries.GetAccessCodesByProject;
using VidMark.Application.Features.ProjectAccessCodes.Queries.ValidateAccessCode;
using VidMark.Application.Services;
using VidMark.Domain.Utils.Constants;

namespace VidMark.API.Controllers;

/// <summary>
/// Controller for managing access codes. Handles operations such as retrieving, validating, retiring, and creating access codes.
/// </summary>
[Route("api/access-codes")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class AccessCodeController(
    IMediator mediator,
    ICurrentUserService currentUserService,
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
        var query = new GetAccessCodesByProjectQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<AccessCodeResponse>>(result.Value);
        return Ok(response);
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
        var command = new RetireAccessCodeCommand(code, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok()
            : NotFound(result.Errors);
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
        var query = new ValidateAccessCodeQuery(accessCodeRequest.Code, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        return Ok(result.Value);
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
        var command = new AddValidCodeToProjectCommand(
            createCodeRequest.ProjectId,
            createCodeRequest.Expiration,
            createCodeRequest.CustomExpiration,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<AccessCodeResponse>(result.Value);
        return CreatedAtAction(
            "GetAccessCodesByProject",
            new { projectId = result.Value.Project.Id },
            response);
    }
}
