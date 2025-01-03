using ProjektGrupowy.API.DTOs.Label;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class LabelService(ILabelRepository labelRepository, ISubjectRepository subjectRepository) : ILabelService
{
    public async Task<Optional<IEnumerable<Label>>> GetLabelsAsync()
    {
        return await labelRepository.GetLabelsAsync();
    }

    public async Task<Optional<Label>> GetLabelAsync(int id)
    {
        return await labelRepository.GetLabelAsync(id);
    }

    public async Task<Optional<Label>> AddLabelAsync(LabelRequest labelRequest)
    {
        var subjectOptional = await subjectRepository.GetSubjectAsync(labelRequest.SubjectId);

        if (subjectOptional.IsFailure)
        {
            return Optional<Label>.Failure("No subject found");
        }

        var label = new Label
        {
            Name = labelRequest.Name,
            Description = labelRequest.Description,
            Subject = subjectOptional.GetValueOrThrow()
        };

        return await labelRepository.AddLabelAsync(label);
    }

    public async Task<Optional<Label>> UpdateLabelAsync(int labelId, LabelRequest labelRequest)
    {
        var labelOptional = await labelRepository.GetLabelAsync(labelId);

        if (labelOptional.IsFailure)
        {
            return labelOptional;
        }

        var label = labelOptional.GetValueOrThrow();

        var subjectOptional = await subjectRepository.GetSubjectAsync(labelRequest.SubjectId);
        if (subjectOptional.IsFailure)
        {
            return Optional<Label>.Failure("No subject found!");
        }

        label.Name = labelRequest.Name;
        label.Description = labelRequest.Description;
        label.Subject = subjectOptional.GetValueOrThrow();

        return await labelRepository.UpdateLabelAsync(label);
    }

    public async Task DeleteLabelAsync(int id)
    {
        var label = await labelRepository.GetLabelAsync(id);
        if (label.IsSuccess)
            await labelRepository.DeleteLabelAsync(label.GetValueOrThrow());
    }
}