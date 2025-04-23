using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.DTOs.Subject;
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
public class SubjectController(
    ISubjectService subjectService,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsAsync()
    {
        var subjects = await subjectService.GetSubjectsAsync();

        return subjects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjects.GetValueOrThrow()))
            : NotFound(subjects.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetSubjectAsync(int id)
    {
        var subject = await subjectService.GetSubjectAsync(id);
        return subject.IsSuccess 
            ? Ok(mapper.Map<SubjectResponse>(subject.GetValueOrThrow())) 
            : NotFound(subject.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> PostSubjectAsync(SubjectRequest subjectRequest)
    {
        subjectRequest.OwnerId = User.GetUserId();
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
        subjectRequest.OwnerId = User.GetUserId();

        var result = await subjectService.UpdateSubjectAsync(id, subjectRequest);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectAsync(int id)
    {
        var subject = await subjectService.GetSubjectAsync(id);

        if (subject.IsFailure)
            return NotFound(subject.GetErrorOrThrow());

        await subjectService.DeleteSubjectAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/label")]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetSubjectLabelsAsync(int id)
    {
        var labels = await subjectService.GetSubjectLabelsAsync(id);
        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }
}
