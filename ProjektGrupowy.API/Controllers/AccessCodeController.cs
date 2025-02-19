using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AccessCode;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccessCodeController(IProjectAccessCodeService service, IMapper mapper) : ControllerBase
{
    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult<IEnumerable<AccessCodeResponse>>> GetAccessCodesByProjectAsync(int projectId)
    {
        var accessCodes = await service.GetAccessCodesByProjectAsync(projectId);
        return accessCodes.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AccessCodeResponse>>(accessCodes.GetValueOrThrow()))
            : Unauthorized(accessCodes.GetErrorOrThrow());
    }

    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateAccessCodeAsync(AccessCodeRequest accessCodeRequest)
    {
        var result = await service.ValidateAccessCode(accessCodeRequest);
        return Ok(result);
    }

    [HttpPost("project")]
    public async Task<ActionResult<AccessCodeResponse>> AddValidCodeToProjectAsync(
        CreateAccessCodeRequest createCodeRequest)
    {
        var result = await service.AddValidCodeToProjectAsync(createCodeRequest);
        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());
        var createdAccessCode = result.GetValueOrThrow();
        var accessCodeResponse = mapper.Map<AccessCodeResponse>(createdAccessCode);
        return CreatedAtAction("GetAccessCodesByProject", new { projectId = createdAccessCode.Project.Id },
            accessCodeResponse);
    }
}