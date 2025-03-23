using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class SubjectController(
    ISubjectService subjectService,
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsAsync()
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var subjects = checkResult.IsAdmin 
            ? await subjectService.GetSubjectsAsync()
            : await subjectService.GetSubjectsByScientistId(checkResult.Scientist!.Id);

        return subjects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjects.GetValueOrThrow()))
            : NotFound(subjects.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetSubjectAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var subject = await subjectService.GetSubjectAsync(id);
        return subject.IsSuccess 
            ? Ok(mapper.Map<SubjectResponse>(subject.GetValueOrThrow())) 
            : NotFound(subject.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> PostSubjectAsync(SubjectRequest subjectRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsProjectAsync(User, subjectRequest.ProjectId);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var result = await subjectService.AddSubjectAsync(subjectRequest);
        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdSubject = result.GetValueOrThrow();
        var subjectResponse = mapper.Map<SubjectResponse>(createdSubject);

        return CreatedAtAction("GetSubject", new { id = createdSubject.Id }, subjectResponse);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectAsync(int id, SubjectRequest subjectRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var subjectResult = await subjectService.GetSubjectAsync(id);
        if (subjectResult.IsFailure)
        {
            return NotFound(new { Message = "Subject not found" });
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
            
            var projectAuthResult = await authHelper.EnsureScientistOwnsProjectAsync(User, subjectRequest.ProjectId);
            if (projectAuthResult != null)
            {
                return projectAuthResult;
            }
        }

        var result = await subjectService.UpdateSubjectAsync(id, subjectRequest);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var subjectResult = await subjectService.GetSubjectAsync(id);
        if (subjectResult.IsFailure)
        {
            return NotFound(new { Message = "Subject not found" });
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        await subjectService.DeleteSubjectAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/label")]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetSubjectLabelsAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsSubjectAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }
        
        if (checkResult.IsLabeler)
        {
            var authResult = await authHelper.EnsureLabelerCanAccessSubjectAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var labels = await subjectService.GetSubjectLabelsAsync(id);
        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }
}
