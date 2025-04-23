using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class AssignedLabelService(
    IAssignedLabelRepository assignedLabelRepository,
    ILabelRepository labelRepository,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    UserManager<User> userManager,
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
        var labelOptional = await labelRepository.GetLabelAsync(assignedLabelRequest.LabelId);

        if (labelOptional.IsFailure)
        {
            return Optional<AssignedLabel>.Failure("No label found");
        }

        var owner = await userManager.FindByIdAsync(assignedLabelRequest.LabelerId);
        if (owner == null)
        {
            return Optional<AssignedLabel>.Failure("No labeler found");
        }

        var subjectVideoGroupAssignmentOptional = await videoRepository.GetVideoAsync(assignedLabelRequest.VideoId);
        if (subjectVideoGroupAssignmentOptional.IsFailure)
        {
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

        return await assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
    }

    public async Task DeleteAssignedLabelAsync(int id)
    {
        var assignedLabel = await assignedLabelRepository.GetAssignedLabelAsync(id);
        if (assignedLabel.IsSuccess)
            await assignedLabelRepository.DeleteAssignedLabelAsync(assignedLabel.GetValueOrThrow());
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId)
    {
        return await assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(videoId);
    }
}