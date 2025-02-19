using ProjektGrupowy.API.DTOs.Project;
using ProjektGrupowy.API.DTOs.Scientist;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IScientistService
{
    Task<Optional<IEnumerable<Scientist>>> GetScientistsAsync();
    Task<Optional<Scientist>> GetScientistAsync(int id);
    Task<Optional<Scientist>> AddScientistAsync(ScientistRequest scientistRequest);
    Task<Optional<Scientist>> UpdateScientistAsync(int scientistId, ScientistRequest scientistRequest);
    Task DeleteScientistAsync(int id);
    Task<Optional<Scientist>> AddScientistWithUser(ScientistRequest scientistRequest, User user);
}