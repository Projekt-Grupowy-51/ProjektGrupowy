using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VidMark.API.DTOs.Labeler;
using VidMark.API.DTOs.SubjectVideoGroupAssignment;
using VidMark.API.Filters;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.AddSubjectVideoGroupAssignment;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.AssignLabelerToAssignment;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.DeleteSubjectVideoGroupAssignment;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.ToggleAssignmentCompletion;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.UnassignLabelerFromAssignment;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Commands.UpdateSubjectVideoGroupAssignment;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignment;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignmentLabelers;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetSubjectVideoGroupAssignments;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetAssignmentStatistics;
using VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetLabelerAssignmentStatistics;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Application.Services;
using VidMark.Domain.Utils.Constants;

namespace VidMark.API.Controllers;

/// <summary>
/// Controller for managing subject video group assignments. Handles operations such as retrieving, adding, updating, and deleting subject video group assignments.
/// </summary>
[Route("api/subject-video-group-assignments")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class SubjectVideoGroupAssignmentController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper,
    IAssignmentCompletionRepository completionRepository) : ControllerBase
{
    /// <summary>
    /// Get all subject video group assignments.
    /// </summary>
    /// <returns>A collection of subject video group assignments.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        var query = new GetSubjectVideoGroupAssignmentsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        // Get all completions for these assignments
        var assignmentIds = result.Value.Select(a => a.Id).ToList();
        var allCompletions = new List<VidMark.Domain.Models.SubjectVideoGroupAssignmentCompletion>();

        foreach (var assignmentId in assignmentIds)
        {
            var completions = await completionRepository.GetByAssignmentIdAsync(assignmentId);
            allCompletions.AddRange(completions);
        }

        var completionsByAssignment = allCompletions
            .GroupBy(c => c.SubjectVideoGroupAssignmentId)
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );

        var response = result.Value.Select(assignment =>
        {
            var mapped = mapper.Map<SubjectVideoGroupAssignmentResponse>(assignment);

            // Calculate IsCompleted: true if all assigned labelers have completed
            var assignedLabelerIds = assignment.Labelers.Select(l => l.Id).ToHashSet();
            var completions = completionsByAssignment.GetValueOrDefault(assignment.Id, new List<VidMark.Domain.Models.SubjectVideoGroupAssignmentCompletion>());

            if (assignedLabelerIds.Count == 0)
            {
                mapped.IsCompleted = false;
            }
            else
            {
                var completedLabelerIds = completions
                    .Where(c => c.IsCompleted)
                    .Select(c => c.LabelerId)
                    .ToHashSet();

                mapped.IsCompleted = assignedLabelerIds.All(id => completedLabelerIds.Contains(id));
            }

            return mapped;
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get all assignments for the current labeler with their personal completion status.
    /// </summary>
    /// <returns>A collection of assignments with completion status for the current user.</returns>
    [HttpGet("labeler")]
    public async Task<ActionResult<IEnumerable<LabelerAssignmentResponse>>> GetLabelerAssignmentsAsync()
    {
        var query = new GetSubjectVideoGroupAssignmentsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        // Get completion status for current user
        var completions = await completionRepository.GetByLabelerIdAsync(currentUserService.UserId);
        var completionDict = completions.ToDictionary(c => c.SubjectVideoGroupAssignmentId, c => c.IsCompleted);

        var response = result.Value.Select(assignment => new LabelerAssignmentResponse
        {
            Id = assignment.Id,
            SubjectId = assignment.Subject.Id,
            VideoGroupId = assignment.VideoGroup.Id,
            ProjectId = assignment.Subject.Project?.Id ?? 0,
            ProjectName = assignment.Subject.Project?.Name ?? string.Empty,
            SubjectName = assignment.Subject.Name,
            VideoGroupName = assignment.VideoGroup.Name,
            IsCompleted = completionDict.GetValueOrDefault(assignment.Id, false),
            CreationDate = assignment.CreationDate,
            ModificationDate = assignment.ModificationDate
        });

        return Ok(response);
    }

    /// <summary>
    /// Get a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <returns>The subject video group assignment with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        var query = new GetSubjectVideoGroupAssignmentQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<SubjectVideoGroupAssignmentResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get labelers assigned to a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <returns>A collection of labelers assigned to the specified subject video group assignment.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/labelers")]
    public async Task<ActionResult<IEnumerable<LabelerResponse>>> GetSubjectVideoGroupAssignmentLabelersAsync(int id)
    {
        var query = new GetSubjectVideoGroupAssignmentLabelersQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<LabelerResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get statistics for a subject video group assignment.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <returns>Statistics for the assignment including video progress, labeler activity, and completion metrics.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/statistics")]
    public async Task<ActionResult<AssignmentStatisticsResponse>> GetAssignmentStatisticsAsync(int id)
    {
        var query = new GetAssignmentStatisticsQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = new AssignmentStatisticsResponse
        {
            TotalVideos = result.Value.TotalVideos,
            TotalLabels = result.Value.TotalLabels,
            AssignedLabelersCount = result.Value.AssignedLabelersCount,
            CompletionPercentage = result.Value.CompletionPercentage,
            Videos = result.Value.Videos.Select(v => new VideoProgressInfo
            {
                Id = v.Id,
                Title = v.Title,
                LabelsReceived = v.LabelsReceived,
                ExpectedLabels = v.ExpectedLabels,
                CompletionPercentage = v.CompletionPercentage
            }).ToList(),
            TopLabelers = result.Value.TopLabelers,
            AllLabelers = result.Value.AllLabelers.Select(l => new LabelerStatInfo
            {
                Id = l.Id,
                Name = l.Name,
                Email = l.Email,
                LabelCount = l.LabelCount,
                CompletionPercentage = l.CompletionPercentage
            }).ToList(),
            VideoStatus = new VideoStatusBreakdown
            {
                Completed = result.Value.VideoStatus.Completed,
                InProgress = result.Value.VideoStatus.InProgress,
                NotStarted = result.Value.VideoStatus.NotStarted
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Get statistics for a specific labeler in a specific assignment.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <param name="labelerId">The ID of the labeler.</param>
    /// <returns>Statistics for the labeler in this assignment including videos labeled and progress.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}/labelers/{labelerId}/statistics")]
    public async Task<ActionResult<LabelerAssignmentStatisticsResponse>> GetLabelerAssignmentStatisticsAsync(int id, string labelerId)
    {
        var query = new GetLabelerAssignmentStatisticsQuery(id, labelerId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = new LabelerAssignmentStatisticsResponse
        {
            LabelerId = result.Value.LabelerId,
            LabelerName = result.Value.LabelerName,
            LabelerEmail = result.Value.LabelerEmail,
            AssignmentId = result.Value.AssignmentId,
            SubjectName = result.Value.SubjectName,
            VideoGroupName = result.Value.VideoGroupName,
            TotalVideos = result.Value.TotalVideos,
            LabeledVideos = result.Value.LabeledVideos,
            TotalLabels = result.Value.TotalLabels,
            IsCompleted = result.Value.IsCompleted,
            Videos = result.Value.Videos.Select(v => new VideoLabelingProgressResponse
            {
                VideoId = v.VideoId,
                VideoTitle = v.VideoTitle,
                LabelCount = v.LabelCount,
                HasLabeled = v.HasLabeled
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Toggle completion status for an assignment by the current user.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment.</param>
    /// <param name="request">The completion status request.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id:int}/completion")]
    public async Task<ActionResult> ToggleAssignmentCompletionAsync(int id, [FromBody] ToggleCompletionRequest request)
    {
        var command = new ToggleAssignmentCompletionCommand(
            id,
            request.IsCompleted,
            currentUserService.UserId,
            currentUserService.IsAdmin);

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Create a new subject video group assignment.
    /// </summary>
    /// <param name="subjectVideoGroupAssignmentRequest">The request containing the details of the subject video group assignment to be created.</param>
    /// <returns>The created subject video group assignment.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var command = new AddSubjectVideoGroupAssignmentCommand(
            subjectVideoGroupAssignmentRequest.SubjectId,
            subjectVideoGroupAssignmentRequest.VideoGroupId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<SubjectVideoGroupAssignmentResponse>(result.Value);
        return CreatedAtAction("GetSubjectVideoGroupAssignment", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Update an existing subject video group assignment.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment to be updated.</param>
    /// <param name="subjectVideoGroupAssignmentRequest">The request containing the updated details of the subject video group assignment.</param>
    /// <returns>No content if the update was successful, otherwise a bad request or not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectVideoGroupAssignmentAsync(int id, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var command = new UpdateSubjectVideoGroupAssignmentCommand(
            id,
            subjectVideoGroupAssignmentRequest.SubjectId,
            subjectVideoGroupAssignmentRequest.VideoGroupId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
    }

    /// <summary>
    /// Delete a specific subject video group assignment by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject video group assignment to be deleted.</param>
    /// <returns>No content if the deletion was successful, otherwise a not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectVideoGroupAssignmentAsync(int id)
    {
        var command = new DeleteSubjectVideoGroupAssignmentCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Assign a labeler to a specific subject video group assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the subject video group assignment.</param>
    /// <param name="labelerId">The ID of the labeler to be assigned.</param>
    /// <returns>Ok if the assignment was successful, otherwise a not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{assignmentId:int}/assign-labeler/{labelerId}")]
    public async Task<IActionResult> AssignLabelerToSubjectVideoGroup(int assignmentId, string labelerId)
    {
        var command = new AssignLabelerToAssignmentCommand(assignmentId, labelerId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok("Labeler assigned successfully")
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Unassign a labeler from a specific subject video group assignment.
    /// </summary>
    /// <param name="assignmentId">The ID of the subject video group assignment.</param>
    /// <param name="labelerId">The ID of the labeler to be unassigned.</param>
    /// <returns>>Ok if the unassignment was successful, otherwise a not found response.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{assignmentId:int}/unassign-labeler/{labelerId}")]
    public async Task<IActionResult> UnassignLabelerFromSubjectVideoGroup(int assignmentId, string labelerId)
    {
        var command = new UnassignLabelerFromAssignmentCommand(assignmentId, labelerId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Ok("Labeler unassigned successfully")
            : NotFound(result.Errors);
    }
}
