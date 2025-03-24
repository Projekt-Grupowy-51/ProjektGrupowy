using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class SubjectVideoGroupAssignmentService(ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository, ISubjectRepository subjectRepository, IVideoGroupRepository videoGroupRepository) : ISubjectVideoGroupAssignmentService
{
    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsAsync();
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(id);
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectVideoGroupAssignmentRequest.SubjectId);

        if (subjectOptional.IsFailure)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("No subject found!");
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(subjectVideoGroupAssignmentRequest.VideoGroupId);

        if (videoGroupOptional.IsFailure)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("No video group found!");
        }

        var subjectVideoGroupAssignment = new SubjectVideoGroupAssignment
        {
            Subject = subjectOptional.GetValueOrThrow(),
            VideoGroup = videoGroupOptional.GetValueOrThrow(),
            CreationDate = DateOnly.FromDateTime(DateTime.Today)
        };

        return await subjectVideoGroupAssignmentRepository.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var subjectVideoGroupAssignmentOptional = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId);

        if (subjectVideoGroupAssignmentOptional.IsFailure)
        {
            return subjectVideoGroupAssignmentOptional;
        }

        var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOptional.GetValueOrThrow();

        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectVideoGroupAssignmentRequest.SubjectId);
        if (subjectOptional.IsFailure)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("No subject found!");
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(subjectVideoGroupAssignmentRequest.VideoGroupId);
        if (videoGroupOptional.IsFailure)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("No video group found!");
        }

        subjectVideoGroupAssignment.Subject = subjectOptional.GetValueOrThrow();
        subjectVideoGroupAssignment.VideoGroup = videoGroupOptional.GetValueOrThrow();
        subjectVideoGroupAssignment.ModificationDate = DateOnly.FromDateTime(DateTime.Today);

        return await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
    }

    public async Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId)
    {
        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId);
        if (subjectVideoGroupAssignment.IsSuccess)
        {
            await subjectVideoGroupAssignmentRepository.DeleteSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment.GetValueOrThrow());
        }
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId);
    }

    public async Task<Optional<IEnumerable<Labeler>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsLabelersAsync(id);
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetSubjectVideoGroupAssignmentAsignedLabelsAsync(int id)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsignedLabelsAsync(id);
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, int labelerId)
    {
        return await subjectVideoGroupAssignmentRepository.AssignLabelerToAssignmentAsync(assignmentId, labelerId);
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(int videoGroupId)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(videoGroupId);
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByScientistIdAsync(int scientistId)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsByScientistIdAsync(scientistId);
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetAssignmentsForLabelerAsync(int labelerId)
    {
        return await subjectVideoGroupAssignmentRepository.GetAssignmentsForLabelerAsync(labelerId);
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetLabelerAssignedLabelsAsync(int assignmentId, int labelerId)
    {
        var assignmentResult = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId);
        if (assignmentResult.IsFailure)
        {
            return Optional<IEnumerable<AssignedLabel>>.Failure(assignmentResult.GetErrorOrThrow());
        }

        return await subjectVideoGroupAssignmentRepository.GetLabelerAssignedLabelsAsync(assignmentId, labelerId);
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsBySubjectIdAsync(int subjectId)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsBySubjectIdAsync(subjectId);
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, int labelerId)
    {
        return await subjectVideoGroupAssignmentRepository.UnassignLabelerFromAssignmentAsync(assignmentId, labelerId);
    }
}