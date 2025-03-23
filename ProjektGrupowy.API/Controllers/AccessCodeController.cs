using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AccessCode;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class AccessCodeController(
    IProjectAccessCodeService service, 
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [HttpGet("project/{projectId:int}")]
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    public async Task<ActionResult<IEnumerable<AccessCodeResponse>>> GetAccessCodesByProjectAsync(int projectId)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsProjectAsync(User, projectId);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var accessCodes = await service.GetAccessCodesByProjectAsync(projectId);
        return accessCodes.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AccessCodeResponse>>(accessCodes.GetValueOrThrow()))
            : NotFound(accessCodes.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateAccessCodeAsync(AccessCodeRequest accessCodeRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var result = await service.ValidateAccessCode(accessCodeRequest);
        return Ok(result);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("project")]
    public async Task<ActionResult<AccessCodeResponse>> AddValidCodeToProjectAsync(
        CreateAccessCodeRequest createCodeRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsProjectAsync(User, createCodeRequest.ProjectId);
            if (authResult != null)
            {
                return authResult;
            }
        }

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