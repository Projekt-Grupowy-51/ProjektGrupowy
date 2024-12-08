using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IScientistRepository
{
    Task<Optional<IEnumerable<Scientist>>> GetScientistsAsync();
    Task<Optional<Scientist>> GetScientistAsync(int id);
    Task<Optional<Scientist>> AddScientistAsync(Scientist scientist);
    Task<Optional<Scientist>> UpdateScientistAsync(Scientist scientist);
    Task DeleteScientistAsync(Scientist scientist);

    Task<IDbContextTransaction> BeginTransactionAsync();
}
