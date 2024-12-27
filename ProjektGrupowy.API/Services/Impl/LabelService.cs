using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class LabelService(ILabelRepository labelRepository) : ILabelService
{
    public async Task<Optional<IEnumerable<Label>>> GetLabelsAsync()
    {
        return await labelRepository.GetLabelsAsync();
    }

    public async Task<Optional<Label>> GetLabelAsync(int id)
    {
        return await labelRepository.GetLabelAsync(id);
    }

    public async Task<Optional<Label>> AddLabelAsync(Label label)
    {
        return await labelRepository.AddLabelAsync(label);
    }

    public async Task<Optional<Label>> UpdateLabelAsync(Label label)
    {
        return await labelRepository.UpdateLabelAsync(label);
    }

    public async Task DeleteLabelAsync(int id)
    {
        var label = await labelRepository.GetLabelAsync(id);
        if (label.IsSuccess)
            await labelRepository.DeleteLabelAsync(label.GetValueOrThrow());
    }
}