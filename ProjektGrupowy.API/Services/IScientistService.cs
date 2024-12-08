using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Scientist;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IScientistService
{
    Task<Optional<IEnumerable<Scientist>>> GetScientistsAsync();
    Task<Optional<Scientist>> GetScientistAsync(int id);
    Task<Optional<Scientist>> AddScientistAsync(Scientist scientist);
    Task<Optional<Scientist>> UpdateScientistAsync(Scientist scientist);
    Task DeleteScientistAsync(int id);
    Task<Optional<Scientist>> AddProjectToScientist(int scientistId, Project project);
}