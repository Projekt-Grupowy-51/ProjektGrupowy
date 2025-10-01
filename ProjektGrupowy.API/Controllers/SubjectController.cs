using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.Label;
using ProjektGrupowy.Application.DTOs.Subject;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing subjects. Handles operations such as retrieving, adding, updating, and deleting subjects.
/// </summary>
/// <param name="subjectService"></param>
/// <param name="mapper"></param>
[Route("api/subjects")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class SubjectController(
    ISubjectService subjectService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all subjects.
    /// </summary>
    /// <returns>A collection of subjects.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsAsync()
    {
        var subjects = await subjectService.GetSubjectsAsync();

        return subjects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjects.GetValueOrThrow()))
            : NotFound(subjects.GetErrorOrThrow());
    }

    /// <summary>
    /// Get a specific subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <returns>The subject with the specified ID.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetSubjectAsync(int id)
    {
        var subject = await subjectService.GetSubjectAsync(id);
        return subject.IsSuccess
            ? Ok(mapper.Map<SubjectResponse>(subject.GetValueOrThrow()))
            : NotFound(subject.GetErrorOrThrow());
    }

    /// <summary>
    /// Create a new subject.
    /// </summary>
    /// <param name="subjectRequest">The request containing the details of the subject to be created.</param>
    /// <returns>The created subject.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> PostSubjectAsync(SubjectRequest subjectRequest)
    {
        var result = await subjectService.AddSubjectAsync(subjectRequest);
        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdSubject = result.GetValueOrThrow();
        var subjectResponse = mapper.Map<SubjectResponse>(createdSubject);

        return CreatedAtAction("GetSubject", new { id = createdSubject.Id }, subjectResponse);
    }

    /// <summary>
    /// Update an existing subject.
    /// </summary>
    /// <param name="id">The ID of the subject to be updated.</param>
    /// <param name="subjectRequest">The request containing the updated details of the subject.</param>
    /// <returns>No content if successful, or BadRequest if the update fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectAsync(int id, SubjectRequest subjectRequest)
    {
        var result = await subjectService.UpdateSubjectAsync(id, subjectRequest);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Delete a subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject to be deleted.</param>
    /// <returns>No content if successful, or NotFound if the subject does not exist.</returns>
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

    /// <summary>
    /// Get all labels associated with a specific subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <returns>>A collection of labels associated with the subject.</returns>
    [HttpGet("{id:int}/labels")]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetSubjectLabelsAsync(int id)
    {
        var labels = await subjectService.GetSubjectLabelsAsync(id);
        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());
    }
}
