using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class AssignedLabelService(
    IAssignedLabelRepository assignedLabelRepository,
    ILabelRepository labelRepository,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    UserManager<User> userManager,
    IMessageService messageService,
    IVideoRepository videoRepository) : IAssignedLabelService

{
    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync()
    {
        return await assignedLabelRepository.GetAssignedLabelsAsync();
    }

    public async Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id)
    {
        return await assignedLabelRepository.GetAssignedLabelAsync(id);
    }

    public async Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        var owner = await userManager.FindByIdAsync(assignedLabelRequest.LabelerId);
        if (owner == null)
        {
            return Optional<AssignedLabel>.Failure("No labeler found");
        }

        var labelOptional = await labelRepository.GetLabelAsync(assignedLabelRequest.LabelId);

        if (labelOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                assignedLabelRequest.LabelerId,
                "No label found");
            return Optional<AssignedLabel>.Failure("No label found");
        }


        var subjectVideoGroupAssignmentOptional = await videoRepository.GetVideoAsync(assignedLabelRequest.VideoId);
        if (subjectVideoGroupAssignmentOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                assignedLabelRequest.LabelerId,
                "No subject video group assignment found");
            return Optional<AssignedLabel>.Failure("No subject video group assignment found");
        }

        var assignedLabel = new AssignedLabel
        {
            Label = labelOptional.GetValueOrThrow(),
            Owner = owner,
            Video = subjectVideoGroupAssignmentOptional.GetValueOrThrow(),
            Start = assignedLabelRequest.Start,
            End = assignedLabelRequest.End
        };

        // return await assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
        var result = await assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                assignedLabelRequest.LabelerId,
                "Failed to add assigned label");
            return result;
        }
        await messageService.SendSuccessAsync(
            assignedLabelRequest.LabelerId,
            "Assigned label added successfully");
        return result;
    }

    public async Task DeleteAssignedLabelAsync(int id)
    {
        var assignedLabelOpt = await assignedLabelRepository.GetAssignedLabelAsync(id);
        if (assignedLabelOpt.IsSuccess)
        {
            var assignedLabel = assignedLabelOpt.GetValueOrThrow();
            await messageService.SendInfoAsync(
                assignedLabel.Owner.Id,
                "Assigned label deleted successfully");
            await assignedLabelRepository.DeleteAssignedLabelAsync(assignedLabel);
        }
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId)
    {
        return await assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(videoId);
    }
}