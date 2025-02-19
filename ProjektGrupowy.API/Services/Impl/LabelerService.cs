using ProjektGrupowy.API.DTOs.Labeler;
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

    public async Task<Optional<Labeler>> AddLabelerAsync(LabelerRequest labelerRequest)
    {
        var labeler = new Labeler
        {
            Name = labelerRequest.Name,
        };

        return await labelerRepository.AddLabelerAsync(labeler);
    }

    public async Task<Optional<Labeler>> AddLabelerWithUser(LabelerRequest labelerRequest, User user)
    {
        var labeler = new Labeler
        {
            Name = labelerRequest.Name,
            User = user
        };

        return await labelerRepository.AddLabelerAsync(labeler);
    }

    public async Task<Optional<Labeler>> UpdateLabelerAsync(int labelerId, LabelerRequest labelerRequest)
    {
        var labelerOptional = await labelerRepository.GetLabelerAsync(labelerId);

        if (labelerOptional.IsFailure)
        {
            return labelerOptional;
        }

        var labeler = labelerOptional.GetValueOrThrow();
        labeler.Name = labelerRequest.Name;

        return await labelerRepository.UpdateLabelerAsync(labeler);
    }

    public async Task DeleteLabelerAsync(int id)
    {
        var labeler = await labelerRepository.GetLabelerAsync(id);
        if (labeler.IsSuccess)
            await labelerRepository.DeleteLabelerAsync(labeler.GetValueOrThrow());
    }
}