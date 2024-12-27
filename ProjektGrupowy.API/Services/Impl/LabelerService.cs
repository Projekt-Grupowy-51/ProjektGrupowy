using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class LabelerService(ILabelerRepository labelerRepository) : ILabelerService
{
    public async Task<Optional<IEnumerable<Labeler>>> GetLabelersAsync()
    {
        return await labelerRepository.GetLabelersAsync();
    }

    public async Task<Optional<Labeler>> GetLabelerAsync(int id)
    {
        return await labelerRepository.GetLabelerAsync(id);
    }

    public async Task<Optional<Labeler>> AddLabelerAsync(Labeler labeler)
    {
        return await labelerRepository.AddLabelerAsync(labeler);
    }

    public async Task<Optional<Labeler>> UpdateLabelerAsync(Labeler labeler)
    {
        return await labelerRepository.UpdateLabelerAsync(labeler);
    }

    public async Task DeleteLabelerAsync(int id)
    {
        var labeler = await labelerRepository.GetLabelerAsync(id);
        if (labeler.IsSuccess)
            await labelerRepository.DeleteLabelerAsync(labeler.GetValueOrThrow());
    }
}