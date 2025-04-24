using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class SubjectVideoGroupAssignmentService(
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository, 
    ISubjectRepository subjectRepository, 
    IVideoGroupRepository videoGroupRepository,
    IMessageService messageService, 
    UserManager<User> userManager) : ISubjectVideoGroupAssignmentService
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
        var owner = await userManager.FindByIdAsync(subjectVideoGroupAssignmentRequest.OwnerId);
        if (owner == null)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("No labeler found");
        }

        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectVideoGroupAssignmentRequest.SubjectId);

        if (subjectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "No subject found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No subject found!");
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(subjectVideoGroupAssignmentRequest.VideoGroupId);

        if (videoGroupOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "No video group found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No video group found!");
        }


        var subjectVideoGroupAssignment = new SubjectVideoGroupAssignment
        {
            Subject = subjectOptional.GetValueOrThrow(),
            VideoGroup = videoGroupOptional.GetValueOrThrow(),
            CreationDate = DateOnly.FromDateTime(DateTime.Today),
            Owner = owner,
        };

        // return await subjectVideoGroupAssignmentRepository.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        var result = await subjectVideoGroupAssignmentRepository.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "Failed to add subject video group assignment");
            return result;
        }
        await messageService.SendSuccessAsync(
            subjectVideoGroupAssignmentRequest.OwnerId,
            "Subject video group assignment added successfully");
        return result;
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var owner = await userManager.FindByIdAsync(subjectVideoGroupAssignmentRequest.OwnerId);
        if (owner == null)
        {
            return Optional<SubjectVideoGroupAssignment>.Failure("No labeler found");
        }

        var subjectVideoGroupAssignmentOptional = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId);

        if (subjectVideoGroupAssignmentOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "No subject video group assignment found");
            return subjectVideoGroupAssignmentOptional;
        }

        var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOptional.GetValueOrThrow();

        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectVideoGroupAssignmentRequest.SubjectId);
        if (subjectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "No subject found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No subject found!");
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(subjectVideoGroupAssignmentRequest.VideoGroupId);
        if (videoGroupOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "No video group found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No video group found!");
        }


        subjectVideoGroupAssignment.Subject = subjectOptional.GetValueOrThrow();
        subjectVideoGroupAssignment.VideoGroup = videoGroupOptional.GetValueOrThrow();
        subjectVideoGroupAssignment.ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        subjectVideoGroupAssignment.Owner = owner;

        // return await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        var result = await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                subjectVideoGroupAssignmentRequest.OwnerId,
                "Failed to update subject video group assignment");
            return result;
        }
        await messageService.SendSuccessAsync(
            subjectVideoGroupAssignmentRequest.OwnerId,
            "Subject video group assignment updated successfully");
        return result;
    }

    public async Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId)
    {
        var subjectVideoGroupAssignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId);
        if (subjectVideoGroupAssignmentOpt.IsSuccess)
        {
            var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOpt.GetValueOrThrow();
            await messageService.SendInfoAsync(
                subjectVideoGroupAssignment.Owner.Id,
                "Subject video group assignment deleted successfully");
            await subjectVideoGroupAssignmentRepository.DeleteSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        }
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId);
    }

    public async Task<Optional<IEnumerable<User>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id)
    {
        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsLabelersAsync(id);
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, string labelerId)
    {
        // return await subjectVideoGroupAssignmentRepository.AssignLabelerToAssignmentAsync(assignmentId, labelerId);
        var assignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId);
        if (assignmentOpt.IsFailure)
        {
            return assignmentOpt;
        }

        var assignmentProjectOwnerId = assignmentOpt.GetValueOrThrow().Owner.Id;

        var result = await subjectVideoGroupAssignmentRepository.AssignLabelerToAssignmentAsync(assignmentId, labelerId);
        if (result.IsFailure)
        {
            await messageService.SendWarningAsync(
                assignmentProjectOwnerId,
                $"Failed to assign labeler to assignment: {result.GetErrorOrThrow()}");
            return result;
        }
        await messageService.SendSuccessAsync(
            assignmentProjectOwnerId,
            "Labeler assigned to assignment successfully");
        return result;
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, string labelerId)
    {
        return await subjectVideoGroupAssignmentRepository.UnassignLabelerFromAssignmentAsync(assignmentId, labelerId);
    }
}