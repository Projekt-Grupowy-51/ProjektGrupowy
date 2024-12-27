using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface ILabelerService
{
    Task<Optional<IEnumerable<Labeler>>> GetLabelersAsync();
    Task<Optional<Labeler>> GetLabelerAsync(int id);
    Task<Optional<Labeler>> AddLabelerAsync(Labeler labeler);
    Task<Optional<Labeler>> UpdateLabelerAsync(Labeler labeler);
    Task DeleteLabelerAsync(int id);
}