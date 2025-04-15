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

        if (!Char.IsLetterOrDigit(labelRequest.Shortcut))
        {
            return Optional<Label>.Failure("Shortcut has to be a letter or a number");
        }

        var label = new Label
        {
            Name = labelRequest.Name,
            Subject = subjectOptional.GetValueOrThrow(),
            ColorHex = labelRequest.ColorHex,
            Type = labelRequest.Type,
            Shortcut = labelRequest.Shortcut
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
        label.Subject = subjectOptional.GetValueOrThrow();
        label.ColorHex = labelRequest.ColorHex;
        label.Type = labelRequest.Type;
        label.Shortcut = labelRequest.Shortcut;

        return await labelRepository.UpdateLabelAsync(label);
    }

    public async Task DeleteLabelAsync(int id)
    {
        var label = await labelRepository.GetLabelAsync(id);
        if (label.IsSuccess)
            await labelRepository.DeleteLabelAsync(label.GetValueOrThrow());
    }

    public async Task<Optional<IEnumerable<Label>>> GetLabelsBySubjectIdAsync(int subjectId)
    {
        return await labelRepository.GetLabelsBySubjectIdAsync(subjectId);
    }

    public async Task<Optional<IEnumerable<Label>>> GetLabelsByScientistIdAsync(int scientistId)
    {
        return await labelRepository.GetLabelsByScientistIdAsync(scientistId);
    }
}