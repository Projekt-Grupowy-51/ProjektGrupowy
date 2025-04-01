using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class AssignedLabelService(
    IAssignedLabelRepository assignedLabelRepository,
    ILabelRepository labelRepository,
    ILabelerRepository labelerRepository,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
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

        var labelerOptional = await labelerRepository.GetLabelerAsync(assignedLabelRequest.LabelerId);
        if (labelerOptional.IsFailure)
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
            Labeler = labelerOptional.GetValueOrThrow(),
            Video = subjectVideoGroupAssignmentOptional.GetValueOrThrow(),
            Start = assignedLabelRequest.Start,
            End = assignedLabelRequest.End
        };

        return await assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
    }

    public async Task<Optional<AssignedLabel>> UpdateAssignedLabelAsync(int assignedLabelId, AssignedLabelRequest assignedLabelRequest)
    {
        var assignedLabelOptional = await assignedLabelRepository.GetAssignedLabelAsync(assignedLabelId);
        if (assignedLabelOptional.IsFailure) {
            return assignedLabelOptional;
        }

        var assignedLabel = assignedLabelOptional.GetValueOrThrow();

        var labelOptional = await labelRepository.GetLabelAsync(assignedLabelRequest.LabelId);
        if (labelOptional.IsFailure)
        {
            return Optional<AssignedLabel>.Failure("No label found");
        }

        var labelerOptional = await labelerRepository.GetLabelerAsync(assignedLabelRequest.LabelerId);
        if (labelerOptional.IsFailure)
        {
            return Optional<AssignedLabel>.Failure("No labeler found");
        }

        var videoOptional = await videoRepository.GetVideoAsync(assignedLabelRequest.VideoId);
        if (videoOptional.IsFailure)
        {
            return Optional<AssignedLabel>.Failure("No subject video group assignment found");
        }

        var label = labelOptional.GetValueOrThrow();
        var labeler = labelerOptional.GetValueOrThrow();
        var video = videoOptional.GetValueOrThrow();

        assignedLabel.Label = label;
        assignedLabel.Labeler = labeler;
        assignedLabel.Video = video;
        assignedLabel.Start = assignedLabelRequest.Start;
        assignedLabel.End = assignedLabelRequest.End;

        return await assignedLabelRepository.UpdateAssignedLabelAsync(assignedLabel);
    }

    public async Task DeleteAssignedLabelAsync(int id)
    {
        var assignedLabel = await assignedLabelRepository.GetAssignedLabelAsync(id);
        if (assignedLabel.IsSuccess)
            await assignedLabelRepository.DeleteAssignedLabelAsync(assignedLabel.GetValueOrThrow());
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByScientistIdAsync(int scientistId)
    {
        return await assignedLabelRepository.GetAssignedLabelsByScientistIdAsync(scientistId);
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByLabelerIdAsync(int labelerId)
    {
        return await assignedLabelRepository.GetAssignedLabelsByLabelerIdAsync(labelerId);
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId)
    {
        return await assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(videoId);
    }
}