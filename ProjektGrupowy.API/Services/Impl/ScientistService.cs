using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Scientist;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ScientistService(IScientistRepository scientistRepository) : IScientistService
{
    public async Task<Optional<Scientist>> AddProjectToScientist(int scientistId, Project project)
    {
        using var transaction = await scientistRepository.BeginTransactionAsync();
        try
        {
            var scientist = await scientistRepository.GetScientistAsync(scientistId);

            if (scientist.IsFailure)
                return Optional<Scientist>.Failure(scientist.GetErrorOrThrow());

            if (project == null)
            {
                return Optional<Scientist>.Failure("Project cannot be null");
            }

            scientist.GetValueOrThrow().Projects.Add(project);

            var updatedScientist = await scientistRepository.UpdateScientistAsync(scientist.GetValueOrThrow());

            await transaction.CommitAsync();

            return updatedScientist;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Optional<Scientist>.Failure(e.Message);
        }
    }

    public async Task<Optional<Scientist>> AddScientistAsync(Scientist scientist)
    {
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

    public async Task<Optional<Scientist>> UpdateScientistAsync(Scientist scientist)
    {
        return await scientistRepository.UpdateScientistAsync(scientist);
    }
}
