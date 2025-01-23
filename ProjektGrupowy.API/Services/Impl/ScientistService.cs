using ProjektGrupowy.API.DTOs.Scientist;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ScientistService(IScientistRepository scientistRepository) : IScientistService
{
    public async Task<Optional<Scientist>> AddScientistAsync(ScientistRequest scientistRequest)
    {
        var scientist = new Scientist
        {
            FirstName = scientistRequest.FirstName,
            LastName = scientistRequest.LastName
        };

        return await scientistRepository.AddScientistAsync(scientist);
    }

    public async Task DeleteScientistAsync(int id)
    {
        var scientist = await scientistRepository.GetScientistAsync(id);
        if (scientist.IsSuccess)
        {
            await scientistRepository.DeleteScientistAsync(scientist.GetValueOrThrow());
        }
    }

    public async Task<Optional<Scientist>> GetScientistAsync(int id)
    {
        return await scientistRepository.GetScientistAsync(id);
    }

    public async Task<Optional<IEnumerable<Scientist>>> GetScientistsAsync()
    {
        return await scientistRepository.GetScientistsAsync();
    }

    public async Task<Optional<Scientist>> UpdateScientistAsync(int scientistId, ScientistRequest scientistRequest)
    {
        var scientistOptional = await scientistRepository.GetScientistAsync(scientistId);

        if (scientistOptional.IsFailure)
        {
            return scientistOptional;
        }

        var scientist = scientistOptional.GetValueOrThrow();
        scientist.FirstName = scientistRequest.FirstName;
        scientist.LastName = scientistRequest.LastName;

        return await scientistRepository.UpdateScientistAsync(scientist);
    }
}
